using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Enemies;
using UnityEditor;
using static Structs;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class SpearAndShieldEnemy : MeleeEnemy
{
    private static readonly int X = Animator.StringToHash("x");
    private static readonly int Y = Animator.StringToHash("y");
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private static readonly int OnAttack = Animator.StringToHash("onAttack");
    private static readonly int AttackDirection = Animator.StringToHash("AttackDirection");
    private static readonly int AttackSmallDirection = Animator.StringToHash("AttackSmallDirection");
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Defense = Animator.StringToHash("Defense");
    private static readonly int DefenseBreak = Animator.StringToHash("DefenseBreak");
    private static readonly int BlockDirection = Animator.StringToHash("BlockDirection");
    private static readonly int OnSmallAttackEnd = Animator.StringToHash("onSmallAttackEnd");
    private static readonly int OnAttackEnd = Animator.StringToHash("onAttackEnd");


    private void Start()
    {
        base.StartUp();
        StartTargeting();
        
    }

    private void Update()
    {
        NextMove();
        if (currentEnemyState == EnemyState.Moving || currentEnemyState == EnemyState.Idle)
        {
             SetAnimator(GetDirection(),rb.velocity.magnitude > 0.05f);
        }else
            rb.velocity = Vector3.zero;
    }

    protected override IEnumerator Attack(Direction direction,Action<bool> callback)
    {
        StopTargeting();
        rb.velocity = Vector2.zero;
        ChangeState(EnemyState.ChargingAttack);
        int[] attackdirection = new int[3];
        for (int i = 0; i < 3; ++i)
            attackdirection[i] = UnityEngine.Random.Range(0, 3);
        yield return new WaitForSeconds(chargeAttackTime);
        animator.SetTrigger(OnAttack);
        animator.SetInteger(AttackDirection,(int)direction);
        // animation will change state to attacking
        foreach (int i in attackdirection)
        {
            //play animation 1..3
            animator.SetInteger(AttackSmallDirection,i);
            yield return new WaitUntil(() => currentEnemyState == EnemyState.Attacking);
            //set enemy not attacking on the end of the animation 
            yield return new WaitUntil(() => currentEnemyState != EnemyState.Attacking);

        }
        //animator.SetBool(IsAttacking,false);
        //play animation
        StartTargeting();
        ChangeState(EnemyState.Idle);
        animator.Play("Idle");
        //yield return new WaitForSeconds(rechargingTime);
        callback(true);
    }

    protected override void SetAnimator(Vector2 dir, bool isWalking)
    {
        float distance = Vector2.Distance(origin.position, target.position - new Vector3(0f, 0.5f, 0f));
        if (distance < attackRange)
        {
            Vector2 direction = (target.position - origin.position).normalized;
            animator.SetFloat(X, direction.x);
            animator.SetFloat(Y,direction.y);
        }
        else
        {
            animator.SetFloat(X, dir.x);
            animator.SetFloat(Y, dir.y);
        }
        animator.SetBool(IsWalking, isWalking);
    }

    public void EndSmallAttack()
    {
        ChangeState(EnemyState.ChargingAttack);
        animator.SetTrigger(OnSmallAttackEnd);
    }
    public void StartSmallAttack()
    {
        ChangeState(EnemyState.Attacking);
    }

    public void PlayBlockAnimation(Vector2 damageSourcePosition)
    {
        var dir = GetDirectionFromDamageSource(damageSourcePosition);
        animator.SetInteger(BlockDirection,(int)dir);
        animator.SetTrigger(Defense);
    }
    public void BlockBreak(Vector2 damageSourcePosition)
    {   
        //Stun(1f); 
        animator.SetTrigger(DefenseBreak);
    }

    protected Direction GetDirectionFromDamageSource(Vector2 damageSourcePosition)
    {
        Vector2 direction = (damageSourcePosition - (Vector2)origin.position).normalized;
        return GetDirection(direction);
    }

    public EnemyState GetState()
    {
        return currentEnemyState;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("PlayerBody"))
            other.GetComponentInParent<HitablePlayer>().GetHit(damage,origin.position,knockback,gameObject,false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position,attackRange);
        Gizmos.DrawLine(transform.position,transform.position);
    }
}

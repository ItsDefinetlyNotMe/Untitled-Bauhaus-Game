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
    private static readonly int onAttack = Animator.StringToHash("onAttack");
    private static readonly int AttackDirection = Animator.StringToHash("AttackDirection");
    private static readonly int AttackSmallDirection = Animator.StringToHash("AttackSmallDirection");
    
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
            SetAnimator(GetDirection(),rb.velocity.magnitude > 0.1f);
        }
    }

    protected override IEnumerator Attack(Direction direction,Action<bool> callback)
    {
        rb.velocity = Vector2.zero;
        ChangeState(EnemyState.ChargingAttack);
        int[] attackdirection = new int[3];
        for (int i = 0; i < 3; ++i)
            attackdirection[i] = UnityEngine.Random.Range(0, 3);
        yield return new WaitForSeconds(chargeAttackTime);
        animator.SetTrigger(onAttack);
        animator.SetInteger(AttackDirection,(int)direction);
        // animation will change state to attacking
        foreach (int i in attackdirection)
        {
            Debug.Log(i);
            //play animation 1..3
            animator.SetInteger(AttackSmallDirection,i);
            yield return new WaitUntil(() => currentEnemyState == EnemyState.Attacking);
            //set enemy not attacking on the end of the animation 
            
            yield return new WaitUntil(() => currentEnemyState == EnemyState.ChargingAttack);
        }
        //animator.SetBool(IsAttacking,false);
        //play animation
        yield return new WaitForSeconds(rechargingTime);
        
        callback(true);
    }

    private void SetAnimator(Vector2 dir,bool isWalking)
    {
        animator.SetFloat(X,dir.x);
        animator.SetFloat(Y,dir.y);
        animator.SetBool(IsWalking,isWalking);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position,attackRange);
        Gizmos.DrawLine(transform.position,transform.position);
    }

    public void EndSmallAttack()
    {
        ChangeState(EnemyState.ChargingAttack);
    }
}

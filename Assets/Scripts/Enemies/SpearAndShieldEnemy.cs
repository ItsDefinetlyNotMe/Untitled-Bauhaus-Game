using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEditor;
using static Structs;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class SpearAndShieldEnemy : MeleeEnemy
{
    private void Start()
    {
        StartTargeting();
        base.StartUp();
    }

    private void Update()
    {
        if (currentEnemyState == EnemyState.Moving || currentEnemyState == EnemyState.Idle)
        {
            animator.SetBool("isWalking",rb.velocity.magnitude > 0.1f);
            //For some reason the rb stops every now and then
            animator.SetFloat("x",direction.x);
            animator.SetFloat("y",direction.y);
        }
        NextMove();
    }

    protected override IEnumerator Attack(Direction direction)
    {

        ChangeState(EnemyState.ChargingAttack);
        int[] attackdirection = new int[3];
        for (int i = 0; i < 3; ++i)
            attackdirection[i] = UnityEngine.Random.Range(1, 4);
        yield return new WaitForSeconds(chargeAttackTime);
        animator.SetBool("isAttacking", true);
        animator.SetInteger("AttackDirection",(int)direction);
        // animation will change state to attacking
        foreach (int i in attackdirection)
        {
            //play animation 1..3
            animator.SetInteger("AttackSmallDirection",i);
            yield return new WaitWhile(() => currentEnemyState == EnemyState.Attacking);
        }
        animator.SetBool("isAttacking",false);
        //play animation
        yield return new WaitForSeconds(rechargingTime);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position,attackRange);
        Gizmos.DrawLine(transform.position,transform.position);
    }
}

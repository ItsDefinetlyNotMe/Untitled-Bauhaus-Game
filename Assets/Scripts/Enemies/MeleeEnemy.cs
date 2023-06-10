using System;
using System.Collections;
using System.Collections.Generic;
using static Structs;
using Enemies;
using Unity.VisualScripting;
using UnityEngine;

abstract public class MeleeEnemy : EnemyMovement
{
       [Header("Movement")]
    
        [Header("States")]
        protected EnemyState currentEnemyState;
    
        [Header("Layer")]
        [SerializeField] protected LayerMask attackLayer;
        [SerializeField] protected LayerMask enemyObstacleLayer;

        [Header("Animation")]
        protected Animator animator; 

        [Header("AttackRanges")]
        [SerializeField] protected float attackRange = 1f;
        [SerializeField] protected float aggroRange = 50f;
    
        [Header("AttackTime")]
        [SerializeField] protected float chargeAttackTime = .6f;
        [SerializeField] protected float rechargingTime = 1f;
        protected bool readyToAttack = true;
        
        protected override void StartUp() 
        {
            base.StartUp();
            currentEnemyState = EnemyState.Moving;
            animator = GetComponent<Animator>();
        }
        /// <summary> Figuring out what to do next based on The state Enemy is in</summary>
        protected void NextMove()
        {//figuring out what to do next called in update
            float distance = Vector2.Distance(transform.position, target.position - new Vector3(0f,0.5f,0f));
            if (currentEnemyState != EnemyState.Attacking)
            {
                SetAnimator(target.position - transform.position,true);
                if (distance <= attackRange * 0.8f)
                {
                    StartAttack();
                }
            }

            /*if (distance > aggroRange)
            {
                StopTargeting();
                ChangeState(EnemyState.Idle);
            }
            else if(currentEnemyState == EnemyState.Idle)
            {
                StartTargeting();
            }*/
        }
        private void StartAttack()
        {
            if(!readyToAttack)
                return;
            Direction attackDirection = Direction.Up;
            //Ray to look wether there is an obstacle between u and player
            Vector2 raydirection = (target.position - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, raydirection, attackRange);
            if(hit)
                if(!hit.transform.CompareTag("Player"))
                    return;
            if (raydirection.y > 0 && Mathf.Abs(raydirection.x) <= raydirection.y)
                attackDirection = Direction.Up;
            else if(Mathf.Abs(raydirection.x) <= Mathf.Abs(raydirection.y))
                attackDirection = Direction.Down;
            else if (raydirection.x > 0)
                attackDirection = Direction.Right;
            else
                attackDirection = Direction.Left;
            bool notused = true;
            readyToAttack = false;
            StartCoroutine(Attack(attackDirection,(notused =>
            {
                readyToAttack = true;
            } )));
            

            
        }

        protected virtual void ChangeState(EnemyState nextState)
        {
            currentEnemyState = nextState;
        }
        /// <summary> Needs to be overriden, implements The Attack  </summary>
        protected abstract IEnumerator Attack(Direction direction,Action<bool> callback);

        protected abstract void SetAnimator(Vector2 dir,bool isWalking);

}

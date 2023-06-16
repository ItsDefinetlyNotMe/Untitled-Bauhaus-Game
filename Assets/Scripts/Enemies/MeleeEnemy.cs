using System;
using System.Collections;
using System.Collections.Generic;
using static Structs;
using Enemies;
using Unity.VisualScripting;
using UnityEngine;

abstract public class MeleeEnemy : EnemyMovement
{
        //[Header("Movement")]
    
        //[Header("States")]
        public EnemyState currentEnemyState { get; private set; }
    
        [Header("Layer")]
        [SerializeField] protected LayerMask attackLayer;
        [SerializeField] protected LayerMask enemyObstacleLayer;

        [Header("Animation")]
        protected Animator animator;

        [Header("Attack")] 
        [SerializeField] protected int damage;

        [SerializeField] protected float knockback;
        
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
            if(isStunned)return;
            float distance = Vector2.Distance(transform.position, target.position - new Vector3(0f,0.5f,0f));
            if (currentEnemyState != EnemyState.Attacking)
            {
                SetAnimator(target.position - transform.position,true);
                if (distance <= attackRange * 0.8f)
                {
                    StartAttack();
                }
            }
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
            attackDirection = GetDirection(raydirection);
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

        protected Direction GetDirection(Vector2 dir)
        {
            if (dir.y > 0 && Mathf.Abs(dir.x) <= dir.y)
                return Direction.Up;
            else if(Mathf.Abs(dir.x) <= Mathf.Abs(dir.y))
                return Direction.Down;
            else if (dir.x > 0)
                return Direction.Right;
            else
                return Direction.Left;
        }
}

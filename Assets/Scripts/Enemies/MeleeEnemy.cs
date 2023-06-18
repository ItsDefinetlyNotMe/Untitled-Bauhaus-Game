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

        protected Transform origin;
        protected override void StartUp() 
        {
            base.StartUp();
            currentEnemyState = EnemyState.Moving;
            animator = GetComponent<Animator>();
            origin = GameObject.Find("OrderLayer").transform;
        }
        /// <summary> Figuring out what to do next based on The state Enemy is in</summary>
        protected void NextMove()
        {//figuring out what to do next called in update
            if(isStunned)return;
            float distance = Vector2.Distance(origin.position, target.position);
            if (currentEnemyState != EnemyState.Attacking)
            {
                SetAnimator(target.position - origin.position,true);
                if (distance <= attackRange * 0.8f)
                {
                    print("I should attack");
                    StartAttack();
                }
            }
        }
        private void StartAttack()
        {
            Time.timeScale = 0.5f;
            if(!readyToAttack)
                return;
            Direction attackDirection = Direction.Up;
            //Ray to look wether there is an obstacle between u and player
            Vector2 offset = new Vector2(0f, -1f) * 0.5f; 
            Vector2 raydirection = ((Vector2)target.position + offset - (Vector2)origin.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(origin.position, raydirection, attackRange);
            print(!hit.transform.CompareTag("Player"));
            /*if(hit)
                if(!hit.transform.CompareTag("Player"))
                    return;*/
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

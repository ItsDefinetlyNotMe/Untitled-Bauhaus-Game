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

        [Header("Directionoffset")] 
        [SerializeField] protected float directionOffset;

        [Header("Layer")]
        [SerializeField] protected LayerMask attackLayer;
        [SerializeField] protected LayerMask enemyObstacleLayer;

        [Header("Animation")]
        protected Animator animator;

        [Header("Attack")] 
        [SerializeField] protected int damage;

        //[SerializeField] protected float knockback;
        
        [Header("AttackRanges")]
        [SerializeField] protected float attackRange = 1f;
        //[SerializeField] protected float aggroRange = 50f;
    
        [Header("AttackTime")]
        [SerializeField] protected float chargeAttackTime = .6f;
        [SerializeField] protected float rechargingTime = 1f;
        public bool readyToAttack = true;

        protected Transform origin;
        protected override void StartUp() 
        {
            base.StartUp();
            currentEnemyState = EnemyState.Moving;
            animator = GetComponent<Animator>();
            origin = transform.GetChild(2).transform;
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
            //Vector2 offset = new Vector2(0f, -1f) * 0.5f; 
            //Vector2 raydirection = ((Vector2)target.position + offset - (Vector2)origin.position).normalized;
            //RaycastHit2D hit = Physics2D.Raycast(origin.position, raydirection, attackRange);
            //print(!hit.transform.CompareTag("PlayerBody"));
            /*if(hit)
                if(!hit.transform.CompareTag("Player"))
                    return;*/
            attackDirection = GetDirection(target.position);
            bool notused = true;
            readyToAttack = false;
            StartCoroutine(Attack(attackDirection,(notused =>
            {
                readyToAttack = true;
            } )));
        }

        public virtual void ChangeState(EnemyState nextState)
        {
            currentEnemyState = nextState;
        }
        /// <summary> Needs to be overriden, implements The Attack  </summary>
        protected abstract IEnumerator Attack(Direction direction,Action<bool> callback);

        protected abstract void SetAnimator(Vector2 dir,bool isWalking);

        protected Direction GetDirection(Vector2 pos)
        {
            Vector2 position = transform.position;
            Vector2 left = position + directionOffset * Vector2.left;
            Vector2 up = position + directionOffset * Vector2.up;
            Vector2 right = position + directionOffset * Vector2.right;
            Vector2 down = position + directionOffset * Vector2.down;

            Direction smallest_direction = Direction.Left;
            float min_dist = float.PositiveInfinity;
            
            if (Vector2.Distance(left, pos) < min_dist)
            {
                min_dist = Vector2.Distance(left, pos);
                smallest_direction = Direction.Left;
            }

            if (Vector2.Distance(up, pos) < min_dist)
            {
                min_dist = Vector2.Distance(up, pos);
                smallest_direction = Direction.Up;
            }
            if (Vector2.Distance(right, pos) < min_dist)
            {
                min_dist = Vector2.Distance(right, pos);
                smallest_direction = Direction.Right;
            }
            if(Vector2.Distance(down, pos) < min_dist)
            {
                min_dist = Vector2.Distance(down, pos);
                smallest_direction = Direction.Down;
            }
                
            return smallest_direction;
        }

        protected override bool ShouldTarget()
        {
            return (currentEnemyState == EnemyState.Idle || currentEnemyState == EnemyState.Moving);
        }
}

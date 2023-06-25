using System;
using System.Collections;
using static Structs;
using Enemies;
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
            Direction attackDirection = GetDirection(target.position);
            //bool notused = true;
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

            Direction smallestDirection = Direction.Left;
            float minDist = float.PositiveInfinity;
            
            if (Vector2.Distance(left, pos) < minDist)
            {
                minDist = Vector2.Distance(left, pos);
                smallestDirection = Direction.Left;
            }

            if (Vector2.Distance(up, pos) < minDist)
            {
                minDist = Vector2.Distance(up, pos);
                smallestDirection = Direction.Up;
            }
            if (Vector2.Distance(right, pos) < minDist)
            {
                minDist = Vector2.Distance(right, pos);
                smallestDirection = Direction.Right;
            }
            if(Vector2.Distance(down, pos) < minDist)
            {
                smallestDirection = Direction.Down;
            }
                
            return smallestDirection;
        }

        protected override bool ShouldTarget()
        {
            return (currentEnemyState == EnemyState.Idle || currentEnemyState == EnemyState.Moving);
        }
}

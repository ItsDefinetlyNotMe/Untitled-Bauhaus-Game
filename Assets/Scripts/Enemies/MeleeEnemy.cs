using System.Collections;
using System.Collections.Generic;
using static Structs;
using Enemies;
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
        [SerializeField]protected float attackRange = 10f;
    
        [Header("AttackTime")]
        [SerializeField] protected float chargeAttackTime = .6f;
        [SerializeField] protected float rechargingTime = 1f;
        
        protected override void StartUp() 
        {
            base.StartUp();
            currentEnemyState = EnemyState.Moving;
            animator = GetComponent<Animator>();
        }
        /// <summary> Figuring out what to do next based on The state Enemy is in</summary>
        protected void NextMove()
        {//figuring out what to do next called in update
            if (currentEnemyState == EnemyState.Moving)
            {
                if (Vector3.Distance(transform.position, target.position) <= attackRange * 0.8f)
                    StartAttack();
            }
        }
        private void StartAttack()
        {
            //figure out direction
            Attack(Direction.Down);

        }

        protected virtual void ChangeState(EnemyState nextState)
        {
            currentEnemyState = nextState;
        }
        /// <summary> Needs to be overriden, implements The Attack  </summary>
        protected abstract IEnumerator Attack(Direction direction);
}

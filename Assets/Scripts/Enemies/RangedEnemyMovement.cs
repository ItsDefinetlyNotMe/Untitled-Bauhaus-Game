using System.Collections;
using UnityEngine;
using static Structs;

namespace Enemies
{
    public abstract class RangedEnemyMovement : EnemyMovement
    {
        [Header("Movement")]
    
        [Header("States")]
        protected EnemyState currentEnemyState;
    
        [Header("Layer")]
        [SerializeField] protected LayerMask projectileLayer;//all layers that are relevant / hitable
        [SerializeField] protected LayerMask projectileObstacleLayer;//layers that are obstacles for the Objectile
        [SerializeField] protected LayerMask enemyObstacleLayer;//Layers that are obstacles for the enemy
        //fleeing

        [Header("Fleeing")]
        private readonly float fleeingOffset = 1f;
        private readonly float fleeingMovementSpeedMultiplier = 3f;

    
        [Header("Animation")]
        protected Animator animator; 

        [Header("AttackRanges")]
        [SerializeField] float minimumRange;
        //[SerializeField] private float stopFleeingOffset;
        [SerializeField]protected float maximumRange = 50f;
    
        [Header("AttackTime")]
        [SerializeField] protected float chargeAttackTime = .6f;
        [SerializeField] protected float rechargingTime = 1f;
    
        [Header("Debug",order = 2)]
        public bool debug;
        public bool debugRanges;
        public bool debugRaycast;
        public bool debugState;
        Vector3 debugdirection;
        Vector3 debugDirection;
        RaycastHit2D[] debugObjecthit;
        /// <summary>To call on Start, gets basic Components </summary>
        protected override void StartUp() 
        {
            base.StartUp();
            currentEnemyState = EnemyState.Moving;
            animator = GetComponent<Animator>();
        }
        /// <summary> Figuring out what to do next based on The state Enemy is in</summary>
        protected void NextMove()
        {//figuring out what to do next called in update
            float distance = Vector2.Distance(transform.position, target.position);
            if(currentEnemyState == EnemyState.Fleeing)
            {
                if(distance > minimumRange + fleeingOffset)
                {
                    ChangeState(EnemyState.Moving);
                }else
                {
                    CheckFleeing();
                }
            }
            if(currentEnemyState == EnemyState.Moving){
                if(distance <= maximumRange && distance > minimumRange)
                    StartAttack();
                else if(distance <= minimumRange)
                {
                    Debug.Log("Fleee you fool");
                    CheckFleeing();
                }
            }
        }
        /// <summary> Raycasting to check wether PLayer is in LOS, if true attacks </summary>
        private void StartAttack()
        {
            int arraysize = 10;
            ContactFilter2D rayCastFilter = new ContactFilter2D
            {
                layerMask = projectileLayer
            };
            RaycastHit2D[]  results = new RaycastHit2D[arraysize];
            Vector2 direction = target.position - new Vector3(0f,0.2f,0) - transform.position;//TODO
            arraysize = Physics2D.Raycast(transform.position, direction.normalized, rayCastFilter, results, maximumRange);//Raycast to check wether player is behind an Object
            debugDirection = direction.normalized;
            for(int i = 0; i < arraysize; ++i)
            {
                debugObjecthit = results;            
                int layer = results[i].collider.gameObject.layer;
                if(projectileObstacleLayer == (projectileObstacleLayer | (1 << layer))){//if the player is not hittable in a straight line, reposition
                    break;
                }
                else if(results[i].transform.CompareTag("Player"))//Attack if player is hittable
                {
                    StartCoroutine(Attack());
                }
            }
        }

        ///<summary> Enemy Raycasting to look wether he can flee, if he has no way out he will just attack </summary>
        private void CheckFleeing()
        {//flee when Player to close
       
            ChangeState(EnemyState.Fleeing);
            Vector3 direction = (transform.position - target.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position,direction,minimumRange,enemyObstacleLayer); 
            debugDirection = direction;
            float distance = hit.distance;
            if(distance <= minimumRange/2 && distance > 0)
            {
                //if in a corner just Attack in desperation
                StartAttack();
            }else
            {//flee 
                rb.velocity = direction * (movementSpeed * fleeingMovementSpeedMultiplier);
                //enemyMovement.SwapTargetBriefly( transform.position + fleeingDirection.normalized * minimumRange,fleeingTime);
            }
        }

        protected virtual void ChangeState(EnemyState nextState)
        {
            currentEnemyState = nextState;
        }
        /// <summary> Needs to be overriden, implements The Attack  </summary>
        protected abstract IEnumerator Attack();
        private void OnDrawGizmosSelected() {
            if(debug)
            {
                if(debugState)
                {
                    if(currentEnemyState == EnemyState.Moving)
                    {
                        Gizmos.color = Color.green;
                    }else if(currentEnemyState == EnemyState.Recharging)
                    {
                        Gizmos.color = Color.red;
                    }
                    else if(currentEnemyState == EnemyState.ChargingAttack) {
                        Gizmos.color = Color.blue;
                    } else if(currentEnemyState == EnemyState.Fleeing){
                        Gizmos.color = Color.yellow;
                    }
                    Gizmos.DrawWireSphere(transform.position,transform.localScale.x*1.2f);
                }
                if(debugRaycast)
                {
                    //Raycast
                    Gizmos.DrawRay(transform.position,debugDirection.normalized * maximumRange);

                    //hitthingy
                    int i = 0;
                    Gizmos.color = Color.green;
                    while(debugObjecthit[i])
                    {
                        if(i == 1)
                            Gizmos.color = Color.white;
                        Gizmos.DrawWireCube(debugObjecthit[i].transform.position,debugObjecthit[i].transform.localScale);
                        ++i;
                    }
                    //
                }
                if(debugRanges){
                    if(currentEnemyState == EnemyState.Moving)
                    {
                        Gizmos.color = Color.green;
                    }else if(currentEnemyState == EnemyState.Recharging)
                    {
                        Gizmos.color = Color.red;
                    }
                    else if(currentEnemyState == EnemyState.ChargingAttack) {
                        Gizmos.color = Color.blue;
                    } else {
                        return;
                    }
                    Vector2 direction = target.position - transform.position;
                    Gizmos.DrawRay(transform.position,direction.normalized * maximumRange);
        
                    if(Vector3.Distance(transform.position,target.position) <= minimumRange + 1.5){
                        Gizmos.DrawWireSphere(transform.position,minimumRange);
                    }
                    Gizmos.DrawWireSphere(transform.position,maximumRange);
                }
            }
        }
    }
}
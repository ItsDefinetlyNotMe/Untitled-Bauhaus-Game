using System.Collections;
using UnityEngine;

namespace Enemies
{
    public class RavenEnemy : RangedEnemyMovement
    {
        [Header("AttackParameters")]
        [SerializeField] private float dashingPower = 7f;
        [SerializeField] private float chargeDamage = 10f;
        [SerializeField] private float dashingTime = 1f;
    
        [Header("DashVisual")]
        RavenDrawPath ravenDrawPath; 
        private float dashRange;
        private static readonly int IsDashing = Animator.StringToHash("isDashing");

        protected void Start(){
            base.StartUp();
            StartTargeting();
            dashRange = dashingPower*dashingTime;
            maximumRange *= (dashRange*2)/3; 
            ravenDrawPath = GetComponentInChildren<RavenDrawPath>();
        }
        private void Update() {
            if(currentState != State.Attacking)
            {   
                float invert = 1;
                if(currentState == State.Fleeing){
                    invert = -1;
                }
                transform.localScale = target.position.x < transform.position.x ? new Vector3(1f*invert, 1f, 1f) : new Vector3(-1f*invert, 1f, 1f);
            }
            NextMove();
        }
        void OnTriggerEnter2D(Collider2D other)
        {//if Attacking detect collisions
        
            if(currentState == State.Attacking){
                if(other.CompareTag("Player"))
                    other.GetComponent<HitablePlayer>().GetHit((int)chargeDamage,transform.position,5);
                else if(projectileLayer == (projectileLayer | (1 << other.gameObject.layer)))
                {
                    ChangeState(State.Recharging);
                    rb.velocity = new Vector2(0f,0f);
                    animator.SetBool(IsDashing,false);
                }
            }
        }
        /// <summary> Raven Attacks Dashing into the enemy, Pattern: charging attack, dashing as attack, Recovering from attack</summary>
        protected override IEnumerator Attack()
        {//channeling Dash & Dashing & recovering 
            //charging Attack
            //set state to charging
            ChangeState(State.ChargingAttack);
            rb.velocity = new Vector3(0,0,0);
            //Choose point to charge to
            Vector3 chargePoint = target.position;
            //Draw Path where he flies
            var position = transform.position;
            ravenDrawPath.DrawPath(position,chargePoint,dashRange,projectileObstacleLayer);
            yield return new WaitForSeconds(chargeAttackTime);
        
            //Dash
            ChangeState(State.Attacking);
            //start animation
            animator.SetBool(IsDashing,true);
            //disable rb collisions and 
            rb.velocity = (chargePoint - position).normalized * dashingPower;
            yield return new WaitForSeconds(dashingTime);
        
            //End dash / recharging
            // ReSharper disable once Unity.InefficientPropertyAccess
            rb.velocity = new Vector2(0,0);
            animator.SetBool(IsDashing,false);
            ravenDrawPath.HidePath();
            ChangeState(State.Recharging);
            yield return new WaitForSeconds(rechargingTime);
        
            //move 
            ChangeState(State.Moving);
        }
        /// <summary>Changing State into parameter </summary> <param name="nextState"></param>
        protected override void ChangeState(State nextState)
        {
            //Changing state and if necessary change some other parameters
            switch(nextState){
                case State.Attacking:
                    rb.isKinematic = true;
                    break;
                case State.ChargingAttack:
                    StopTargeting();
                    break;
                case State.Moving:
                    rb.isKinematic = false;
                    StartTargeting();
                    break;
                case State.Fleeing:
                    StopTargeting();
                    break;
            }
            if(nextState != State.Moving)
            {    
                rb.isKinematic = false;
            }
            currentState = nextState;
        }   
    }
}
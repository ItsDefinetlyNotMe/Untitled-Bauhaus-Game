using System.Collections;
using UnityEngine;

public class RavenEnemy : RangedEnemyMovement
{
    [Header("AttackParameters")]
    [SerializeField] private float dashingPower = 7f;
    [SerializeField] private float chargeDamage = 10f;
    [SerializeField] private float dashingTime = 1f;
    
    [Header("DashVisual")]
    RavenDrawPath ravenDrawPath; 
    private float dashRange;

    protected void Start(){
        base.StartUp();
        base.StartTargeting();
        dashRange = dashingPower*dashingTime;
        maximumRange *= (dashRange*2)/3; 
        ravenDrawPath = GetComponentInChildren<RavenDrawPath>();
    }
    private void Update() {
        if(currentState != state.ATTACKING)
        {   
            float invert = 1;
            if(currentState == state.FLEEING){
                invert = -1;
            }
            if (target.position.x < transform.position.x)
                transform.localScale = new Vector3(1f*invert, 1f, 1f);
            else
                transform.localScale = new Vector3(-1f*invert, 1f, 1f);
        }
        base.NextMove();
    }
    void OnTriggerEnter2D(Collider2D other)
    {//if Attacking detect collisions
        
        if(currentState == state.ATTACKING){
            if(other.CompareTag("Player"))
                other.GetComponent<HitablePlayer>().GetHit((int)chargeDamage,transform.position,5);
            else if(projectileLayer == (projectileLayer | (1 << other.gameObject.layer)))
            {
                ChangeState(state.RECHARGING);
                rb.velocity = new Vector2(0f,0f);
                animator.SetBool("isDashing",false);
            }
        }
    }
    /// <summary> Raven Attacks Dashing into the enemy, Pattern: charging attack, dashing as attack, Recovering from attack</summary>
    override public IEnumerator Attack()
    {//channeling Dash & Dashing & recovering 
        //charging Attack
        //set state to charging
        ChangeState(state.CHARGING_ATTACK);
        rb.velocity = new Vector3(0,0,0);
        //Choose point to charge to
        Vector3 chargePoint = target.position;
        //Draw Path where he flies
        ravenDrawPath.DrawPath(transform.position,chargePoint,dashRange,projectileObstacleLayer);
        yield return new WaitForSeconds(chargeAttackTime);
        
        //Dash
        ChangeState(state.ATTACKING);
        //start animation
        animator.SetBool("isDashing",true);
        //disable rb collisions and 
        rb.velocity = (chargePoint - transform.position).normalized * dashingPower;
        yield return new WaitForSeconds(dashingTime);
        
        //End dash / recharging
        rb.velocity = new Vector2(0,0);
        animator.SetBool("isDashing",false);
        ravenDrawPath.HidePath();
        ChangeState(state.RECHARGING);
        yield return new WaitForSeconds(rechargingTime);
        
        //move 
        ChangeState(state.MOVING);
    }
    /// <summary>Changing State into parameter </summary> <param name="nextState"></param>
    override public void ChangeState(state nextState)
    {
        //Changing state and if necessary change some other parameters
        switch(nextState){
            case state.ATTACKING:
                rb.isKinematic = true;
                break;
            case state.CHARGING_ATTACK:
                base.StopTargeting();
                break;
            case state.MOVING:
                rb.isKinematic = false;
                base.StartTargeting();
                break;
            case state.FLEEING:
                base.StopTargeting();
                break;
            default:
                break;        
        }
        if(nextState != state.MOVING)
        {    
            rb.isKinematic = false;
        }
        currentState = nextState;
    }   
}
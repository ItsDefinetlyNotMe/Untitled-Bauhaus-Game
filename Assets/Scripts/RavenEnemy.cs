using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum state{
    CHARGING_ATTACK,DASHING,RECHARGING,MOVING,FLEEING
}

public class RavenEnemy : MonoBehaviour
{
    //Movement
    private Rigidbody2D rb;
    [SerializeField] private LayerMask airLayer;//all layers that are relevant / hitable
    [SerializeField] private LayerMask enviromentLayer;//layers that are obstacles
    state currentState;
    [SerializeField] private float dashingPower = 5f;
    [SerializeField] private float dashingTime = 1f;
    [SerializeField] private float recharginTime = 1f;
    //Animation
    Animator animator;
    RavenDrawPath ravenDrawPath; 
    //combat
    [SerializeField]private float chargeDamage = 10f;
    private Transform target;
    [SerializeField] private float chargeAttackTime = .6f;
    Collider2D collider;
    [SerializeField] float minimumRange;
    [SerializeField] float maximumRange = 50f;
    //Debug
    bool debug = false; 

   private void Start() 
    {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        target = (GameObject.FindGameObjectsWithTag("Player"))[0].transform;
        currentState = state.MOVING;
        //rb.isKinematic = true;
        animator = GetComponent<Animator>();
        ravenDrawPath = GetComponentInChildren<RavenDrawPath>();
        debug = true;
    }
   void Update()
    {
        if(currentState != state.DASHING)
        {
            if (target.position.x < transform.position.x)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }   
        }

        float distance = Vector2.Distance(transform.position,target.position);
        if(currentState == state.MOVING &&  distance < maximumRange)
            StartAttack();
    }
   void OnTriggerEnter2D(Collider2D other)
    {
        if(currentState == state.DASHING){
            if(other.CompareTag("Player"))
                other.GetComponent<HitablePlayer>().GetHit((int)chargeDamage,transform.position,5);
            else if(airLayer == (airLayer | (1 << other.gameObject.layer)))
            {
                rb.isKinematic = false;
                rb.velocity = new Vector2(0f,0f);
                animator.SetBool("isDashing",false);
            }
        }
    }
   private void StartAttack()
    {
        int arraysize = 10;
        ContactFilter2D rayCastFilter = new ContactFilter2D();
        rayCastFilter.layerMask = airLayer;
        RaycastHit2D[]  results = new RaycastHit2D[arraysize];
        Vector2 direction = target.position - new Vector3(0f,0.2f,0) - transform.position;
        arraysize = Physics2D.Raycast(transform.position,direction.normalized,rayCastFilter,results,maximumRange);//Raycast to check wether player is behind an Object
        
        for(int i = 0; i < arraysize; ++i)
        {
            Debug.Log(results[i].collider.gameObject.name);
            int layer = results[i].collider.gameObject.layer;
            if(enviromentLayer == (enviromentLayer | (1 << layer))){
                Reposition();
                break;
            }
            else if(results[i].transform.CompareTag("Player"))
                StartCoroutine(Attack());
        }
    }
   public IEnumerator Attack()
    {
        //charging Attack
        //set state to charging
        currentState = state.CHARGING_ATTACK;
        //Choose point to charge to
        Vector3 chargePoint = target.position;
        //Draw Path where he flies
        ravenDrawPath.DrawPath(chargePoint,transform.position);
        yield return new WaitForSeconds(chargeAttackTime);
        
        //Dash
        currentState = state.DASHING;
        //start animation
        animator.SetBool("isDashing",true);
        //disable rb collisions and 
        rb.isKinematic = true;
        rb.velocity = (chargePoint - transform.position).normalized * dashingPower;
        yield return new WaitForSeconds(dashingTime);
        
        //End dash / recharging
        rb.velocity = new Vector2(0,0);
        animator.SetBool("isDashing",false);
        ravenDrawPath.HidePath();
        currentState = state.RECHARGING;
        rb.isKinematic = false;
        yield return new WaitForSeconds(recharginTime);
        
        //move 
        currentState = state.MOVING;
    }
   private void Reposition()
   {
    	//repositioning when unable to hit Player

   }

   private void flee()
   {
        //flee when Player to close
   }
   void OnDrawGizmos()
   {
    if(debug)
    {
        if(currentState == state.MOVING)
        {
            Gizmos.color = Color.green;
        }else if(currentState == state.RECHARGING)
        {
            Gizmos.color = Color.red;
        }
        else if(currentState == state.CHARGING_ATTACK) {
            Gizmos.color = Color.blue;
        } else {
            return;
        }
        Vector2 direction = target.position - transform.position;
        Gizmos.DrawRay(transform.position,direction.normalized * maximumRange);
        
        if(Vector3.Distance(transform.position,target.position) <= minimumRange + 1.5){
            Gizmos.DrawWireSphere(transform.position,minimumRange);
        }
    }
   }
}
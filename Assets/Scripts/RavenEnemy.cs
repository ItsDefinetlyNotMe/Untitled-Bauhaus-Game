using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum state{
    CHARGING_ATTACK,DASHING,RECHARGING ,MOVING
}

public class RavenEnemy : MonoBehaviour
{
    //Movement
    private Rigidbody2D rb;
    [SerializeField] private LayerMask airLayer;
    state currentState;
    [SerializeField] private float dashingPower = 5f;
    [SerializeField] private float dashingTime = 1f;
    [SerializeField] private float recharginTime = 1f;
    //combat
    [SerializeField]private float chargeDamage = 10f;
    private Transform target;
    [SerializeField] private float chargeAttackTime = .6f;
    Collider2D collider;
    [SerializeField] float minimumRange;
    [SerializeField] float maximumRange = 50f;
    private void Start() 
    {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        target = (GameObject.FindGameObjectsWithTag("Player"))[0].transform;
        currentState = state.MOVING;
        rb.isKinematic = true;
        
    }
    void Update()
    {
        float distance = Vector2.Distance(transform.position,target.position);
        Debug.Log("isMoving: " + (currentState == state.MOVING));
        Debug.Log("inRange: " + (distance < maximumRange));
        if(currentState == state.MOVING &&  distance < maximumRange)
            StartAttack();
    }
    private void StartAttack()
    {
        ContactFilter2D rayCastFilter = new ContactFilter2D();
        rayCastFilter.layerMask = airLayer;
        RaycastHit2D[]  results = new RaycastHit2D[1];
        Vector2 direction = target.position -new Vector3(0f,0.2f,0) - transform.position;
        Physics2D.Raycast(transform.position,direction.normalized,rayCastFilter,results,maximumRange);//Raycast to check wether player is behind an Object

        if(results[0])
        {
            Debug.Log(results[0].transform.CompareTag("Player"));
            if(results[0].transform.CompareTag("Player"))
                StartCoroutine(Attack());
                
        }
    }
    public IEnumerator Attack()
    {
        currentState = state.CHARGING_ATTACK;
        //charge Attack
        Vector3 chargePoint = target.position; 
        //Draw Path where he flies
        yield return new WaitForSeconds(chargeAttackTime);
        //start animation
        
        currentState = state.DASHING;
        rb.velocity = (chargePoint - transform.position).normalized * dashingPower;
        yield return new WaitForSeconds(dashingTime);
        rb.velocity = new Vector2(0,0);
        currentState = state.RECHARGING;
        yield return new WaitForSeconds(recharginTime);
        currentState = state.MOVING;

        
        //charges in said direction until it hits a relevant collider or maximum range
    }
  
    void OnTriggerEnter2D(Collider2D other)
   {
        if(currentState == state.DASHING){
            if(other.CompareTag("Player"))
                other.GetComponent<HitablePlayer>().GetHit((int)chargeDamage,transform.position,5);
        }

   }
   void OnDrawGizmos()
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
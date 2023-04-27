using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponScript : MonoBehaviour
{
    Vector2 playerDirection = new Vector2(-1,0);
    private float attackRange = 1f;
    private float attackSpeed = 2f;
    private int attackDamage = 20;
    private float knockbackAmplifier = 10f;
    [SerializeField] float weaponOffset = 0.5f;
    [SerializeField] LayerMask enemyLayers;
    float nextAttack = 0f;
    PlayerMovement movementScript;
    PlayerAnimator playerAnimator;

    Animator animator;

    Collider2D weaponHitBox;

    private void Start()
    {
        movementScript = GetComponentInParent<PlayerMovement>();
        playerAnimator = GetComponentInParent<PlayerAnimator>();
        animator = GetComponentInParent<Animator>();
        weaponHitBox = GetComponent<Collider2D>();
        transform.localPosition = weaponOffset * playerDirection;
    }

    public void Attack()
    {
        //tracking the attacktimer and detecting enemys in attackradius if possible to attack
        if(Time.time >= nextAttack){
            //call animation
            DetermineAttackDirection();

            //Cooldown
            nextAttack = Time.time + 1f/attackSpeed;

            //locating enemies
            transform.localPosition = weaponOffset * playerDirection;

            List<Collider2D> enemiesHit = new List<Collider2D>();
            ContactFilter2D enemyFilter = new ContactFilter2D();
            enemyFilter.SetLayerMask(enemyLayers);
            weaponHitBox.OverlapCollider(enemyFilter, enemiesHit);

            foreach (Collider2D enemy in enemiesHit)
                enemy.GetComponent<EnemyScript>().TakeDamage(attackDamage,playerDirection,knockbackAmplifier);
        }
    }

    public abstract void LeftTriggerAttack();//special Attack based on weapon

    private void Update()
    {
        //getting the last direction player looked in
        if(movementScript.movementDirection.magnitude > 0)
            playerDirection = movementScript.movementDirection;

    }

    private void DetermineAttackDirection()
    {
        string attackDirection = "";

        //if the absolute value of y is bigger than the absolute value of x you attack up
        //same thing with the down direction
        if (playerDirection.y > 0)
        {
            if (Mathf.Abs(playerDirection.x) <= playerDirection.y)
                attackDirection = "AttackUp";
            else if (playerDirection.x > 0)
                attackDirection = "AttackRight";                
            else
                attackDirection = "AttackLeft";
        }
        else
        {
            if (Mathf.Abs(playerDirection.x) <= Mathf.Abs(playerDirection.y))
                attackDirection = "AttackDown";
            else if (playerDirection.x > 0)
                attackDirection = "AttackRight";
            else
                attackDirection = "AttackLeft";
        }
        
        //call actual animation in PlayerAnimator.cs
        playerAnimator.PlayAttackAnimation(attackDirection);
    }
    
    void OnDrawGizmosSelected()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawCube(transform.position, 1);
    }
}

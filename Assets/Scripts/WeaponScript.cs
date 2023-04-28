using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponScript : MonoBehaviour
{
    Vector2 playerDirection = new Vector2(-1,0);
    private float attackSpeed = 2f;
    public int attackDamage { get; private set; } = 20; 
    public float knockbackAmplifier { get; private set; } = 10f;
    [SerializeField] LayerMask hittableLayers;
    float nextAttack = 0f;
    PlayerMovement movementScript;
    PlayerAnimator playerAnimator;

    Animator animator;
    //Hitbox
    Collider2D[] weaponHitBoxes;

    private void Start()
    {
        movementScript = GetComponentInParent<PlayerMovement>();
        playerAnimator = GetComponentInParent<PlayerAnimator>();
        animator = GetComponentInParent<Animator>();
        //maybe temporary
        weaponHitBoxes = new Collider2D[4];
        for(int i = 0; i < 4;++i ){
            //Left, Up, Right, Down
            weaponHitBoxes[i] = transform.GetChild(i).GetComponent<Collider2D>();
        }
    }

    public int Attack(ref List<Collider2D> enemiesHit )
    {
        //tracking the attacktimer and detecting enemys in attackradius if possible to attack
        if(Time.time >= nextAttack){
            //call animation
            DetermineAttackDirection();

            //Cooldown
            nextAttack = Time.time + 1f/attackSpeed;

            //locating enemies
            ContactFilter2D enemyFilter = new ContactFilter2D();
            enemyFilter.SetLayerMask(hittableLayers);
            foreach(Collider2D weaponHitBox in weaponHitBoxes)
            {
                if(!weaponHitBox.enabled)
                    continue;
                weaponHitBox.OverlapCollider(enemyFilter, enemiesHit);
            }
            return attackDamage;
        }
        return 0;
    }
    public abstract void LeftTriggerAttack();//special Attack based on weapon
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
}

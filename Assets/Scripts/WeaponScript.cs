using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponScript : MonoBehaviour
{
    Vector2 playerDirection = new Vector2(-1,0);
    private float attackSpeed = 2f;
    [SerializeField] protected int attackDamage = 20;
    protected float knockbackAmplifier = 10f;
    [SerializeField] LayerMask hittableLayers;
    float nextAttack = 0f;
    PlayerMovement movementScript;
    PlayerAnimator playerAnimator;
    Animator animator;
    //Hitbox
    Collider2D[] weaponHitBoxes;
    public bool atttttttack=false;

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
    void Update()
    {
        if(movementScript.movementDirection.magnitude > 0)
            playerDirection = movementScript.movementDirection;
        Debug.Log(weaponHitBoxes[0].enabled);
    }
    public int Attack(ref List<Collider2D> enemiesHit )
    {
        //tracking the attacktimer and detecting enemys in attackradius if possible to attack
        if(Time.time >= nextAttack){
            //call animation
            DetermineAttackDirection();
            //wait for 
            StartCoroutine(WaitForAttackAnimation(enemiesHit));
            //Cooldown
            nextAttack = Time.time + 1f/attackSpeed;

            //locating enemies
            
            return attackDamage;
        }
        return 0;
    }
    private void TrackEnemiesInAttackHitbox(List<Collider2D> enemiesHit)
    {
        Debug.Log("Attacking");
        ContactFilter2D enemyFilter = new ContactFilter2D();
        //enemyFilter.SetLayerMask(hittableLayers);
        foreach(Collider2D weaponHitBox in weaponHitBoxes)
        {
            if(!weaponHitBox.gameObject.activeSelf){
                continue;
            }
            weaponHitBox.OverlapCollider(enemyFilter.NoFilter(), enemiesHit);
        }
        foreach(Collider2D enemy in enemiesHit){
            Debug.Log(enemy.name);
        }
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
    private IEnumerator WaitForAttackAnimation(List<Collider2D> enemiesHit)
    {
        yield return new WaitWhile(() => atttttttack == false);
        TrackEnemiesInAttackHitbox(enemiesHit);
    }
}

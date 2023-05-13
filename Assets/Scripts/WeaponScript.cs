using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Direction;
public abstract class WeaponScript : MonoBehaviour
{
    
    [Header("Animation")]
    Animator animator;
    PlayerAnimator playerAnimator;

    [Header("Stats")]
    [SerializeField] protected int attackDamage = 20;
    private float attackSpeed = 2f;
    protected float knockbackAmplifier = 10f;
    
    [Header("Layer")]
    [SerializeField] LayerMask hittableLayers;

    [Header("Player")]
    PlayerMovement movementScript;
    Vector2 playerDirection = new Vector2(-1,0);

    [Header("Physics")]
    Collider2D[] weaponHitBoxes;
    
    [Header("Attack")]
    private float nextAttack = 0f;
    public bool isAttacking = false;
    private int attackNumber = 0;
    [SerializeField] private float attackNumberCooldown = 5.0f;
    private float attackNumberTimeStamp = 0f;
    private Direction attackDirection;

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
    }
    public IEnumerator Attack(Action<List<Collider2D>,int> callback )
    {
        //tracking the attacktimer and detecting enemys in attackradius if possible to attack
        if(Time.time >= nextAttack){
            //call animation
            DetermineAttackDirection();
            
            //Cooldown
            nextAttack = Time.time + 1f/attackSpeed;
            
            //Wait for animation to start
            yield return new WaitWhile(() => isAttacking == false);
            
            //locating enemies
            List<Collider2D> enemiesHit = new List<Collider2D>();
            ContactFilter2D enemyFilter = new ContactFilter2D();
            enemyFilter.SetLayerMask(hittableLayers);
            foreach(Collider2D weaponHitBox in weaponHitBoxes)
            {
                if(!weaponHitBox.gameObject.activeSelf){
                    continue;
                }
                weaponHitBox.OverlapCollider(enemyFilter, enemiesHit);
            }
            //TODO maybe wait until attack is over ? will probably lead to delayed dmg from attacks might look weird
            //yield return new WaitWhile(() => isAttacking == true);
        
            //giving back enemies and the attackdamage as soon as they are calculated 
            callback(enemiesHit,attackDamage);
        }

    }
    public abstract void LeftTriggerAttack();//special Attack based on weapon
    private void DetermineAttackDirection()
    {
        Direction newAttackDirection;
        
        //if the absolute value of y is bigger than the absolute value of x you attack up
        //same thing with the down direction
        //TODO fix this
        if (playerDirection.y > 0 && Mathf.Abs(playerDirection.x) <= playerDirection.y)
            newAttackDirection = UP;
        else if(Mathf.Abs(playerDirection.x) <= Mathf.Abs(playerDirection.y))
            newAttackDirection = DOWN;
        else if (playerDirection.x > 0)
            newAttackDirection = RIGHT;
        else
            newAttackDirection = LEFT;

        //determine what Attack we are at
        if(attackDirection != newAttackDirection)
            attackNumber = 0;
        if(Time.time > attackNumberTimeStamp)
            attackNumber = 0;
        
        attackDirection = newAttackDirection; 
        
        //call actual animation in PlayerAnimator.cs
        playerAnimator.PlayAttackAnimation(attackDirection,attackNumber);
        attackNumber = (attackNumber + 1) % 3;
        attackNumberTimeStamp = attackNumberCooldown + Time.time;
    }
}

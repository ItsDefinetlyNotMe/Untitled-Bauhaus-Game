using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static Structs.Direction;
using static Structs.PlayerState;
public abstract class WeaponScript : MonoBehaviour
{
    
    [Header("Animation")]
    PlayerAnimator playerAnimator;

    [Header("Stats")]
    [SerializeField] protected int attackDamage = 20;
    private readonly float attackSpeed = 2f;
    protected float knockbackAmplifier = 10f;
    
    [Header("Layer")]
    [SerializeField] LayerMask hittableLayers;

    [Header("Player")]
    PlayerMovement movementScript;
    Vector2 playerDirection = new Vector2(-1,0);

    [Header("Physics")]
    Collider2D[] weaponHitBoxes;
    
    
    [Header("Attack")]
    private float nextAttack;
    private bool isAttacking = false;
    private float nextHeavyAttack;
    //public bool isAttacking = false;
    private int attackNumber;
    [SerializeField] private float attackNumberCooldown = 5.0f;
    private float attackNumberTimeStamp;
    private Structs.Direction attackDirection;

    private void Start()
    {
        movementScript = GetComponentInParent<PlayerMovement>();
        playerAnimator = GetComponentInParent<PlayerAnimator>();
        //maybe temporary
        weaponHitBoxes = new Collider2D[4];
        for(int i = 0; i < 4;++i ){
            //Left, Up, Right, Down
            weaponHitBoxes[i] = transform.GetChild(i).GetComponent<Collider2D>();
        }
    }
    private void Update()
    {
        if(movementScript.movementDirection.magnitude > 0)
            playerDirection = movementScript.movementDirection;
    }

    public IEnumerator Attack(Action<List<Collider2D>,int> callback )
    {
        //tracking the attacktimer and detecting enemys in attackradius if possible to attack
        if(/*Time.time >= nextAttack*/ !isAttacking && movementScript.currentState == Moving){
            //movementScript.ChangeState(ATTACKING);

            //Cooldown
            isAttacking = true;
            nextAttack = Time.time + 1f/attackSpeed;

            //call animation
            DetermineAttackNumber(DetermineAttackDirection());
            
            //Wait for animation to start
            //yield return new WaitWhile(() => isAttacking == false);
            yield return new WaitWhile(() => movementScript.currentState != Attacking);
            
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
            //Debug.Log(enemiesHit.Count);
            callback(enemiesHit,attackDamage);
            //yield return new WaitWhile(()=> isAttacking == true);
            yield return new WaitWhile(()=> movementScript.currentState == Attacking);
            //movementScript.ChangeState(MOVING);
        }
    }

    public IEnumerator HeavyAttack(Action<List<Collider2D>,int> callback)
    {
        //call animation
            DetermineAttackDirection();
            playerAnimator.SetDirection(attackDirection);
            playerAnimator.PlayHeavyAttackAnimation(attackDirection);

            
            //Cooldown
            nextHeavyAttack = Time.time + 1f/attackSpeed;
            
            //Wait for Hitbox to be ready
            yield return new WaitUntil(() => IsAnyHitboxEnabled());
            
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

            //giving back enemies and the attackdamage as soon as they are calculated
            callback(enemiesHit,attackDamage);
            //yield return new WaitWhile(()=> isAttacking == true);
            yield return new WaitWhile(()=> movementScript.currentState == Attacking);
            //movementScript.ChangeState(MOVING);
        //}
        /*else
        {
            Debug.Log(":c");
        }*/
    }

    private void AttackFinished()
    {
        isAttacking = false;
    }

    private bool IsAnyHitboxEnabled()
    {
        foreach (Collider2D hitbox in weaponHitBoxes)
        {
            if (hitbox.enabled)
                return true; 
        }
        return false;
    }
    //public abstract void HeavyAttack();//special Attack based on weapon
    public Structs.Direction DetermineAttackDirection()
    {
        Structs.Direction newAttackDirection;
        
        //if the absolute value of y is bigger than the absolute value of x you attack up
        //same thing with the down direction
        //TODO fix this
        if (playerDirection.y > 0 && Mathf.Abs(playerDirection.x) <= playerDirection.y)
            newAttackDirection = Up;
        else if(Mathf.Abs(playerDirection.x) <= Mathf.Abs(playerDirection.y))
            newAttackDirection = Down;
        else if (playerDirection.x > 0)
            newAttackDirection = Right;
        else
            newAttackDirection = Left;

        //determine what Attack we are at//TODO unschön dass es sich um die nummer kümmert
        if(attackDirection != newAttackDirection)
            attackNumber = 0;
        attackDirection = newAttackDirection; 

        return newAttackDirection; 
    }

    private void DetermineAttackNumber(Structs.Direction direction)
    {
        if(Time.time > attackNumberTimeStamp)
            attackNumber = 0;
        
        //call actual animation in PlayerAnimator.cs
        playerAnimator.PlayAttackAnimation(attackDirection,attackNumber);
        attackNumber = (attackNumber + 1) % 3;
        attackNumberTimeStamp = attackNumberCooldown + Time.time;
    }
    
}

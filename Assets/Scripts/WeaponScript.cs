using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Direction;
using static PlayerState;
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
    //public bool isAttacking = false;
    private int attackNumber;
    [SerializeField] private float attackNumberCooldown = 5.0f;
    private float attackNumberTimeStamp;
    private Direction attackDirection;

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
    void Update()
    {
        if(movementScript.movementDirection.magnitude > 0)
            playerDirection = movementScript.movementDirection;
    }
    public IEnumerator Attack(Action<List<Collider2D>,int> callback )
    {
        //tracking the attacktimer and detecting enemys in attackradius if possible to attack
        if(Time.time >= nextAttack && movementScript.currentState == Moving){
            //movementScript.ChangeState(ATTACKING);
            //call animation
            DetermineAttackDirection();
            
            //Cooldown
            nextAttack = Time.time + 1f/attackSpeed;
            
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
            callback(enemiesHit,attackDamage);
            //yield return new WaitWhile(()=> isAttacking == true);
            yield return new WaitWhile(()=> movementScript.currentState == Attacking);
            //movementScript.ChangeState(MOVING);
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
            newAttackDirection = Up;
        else if(Mathf.Abs(playerDirection.x) <= Mathf.Abs(playerDirection.y))
            newAttackDirection = Down;
        else if (playerDirection.x > 0)
            newAttackDirection = Right;
        else
            newAttackDirection = Left;

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

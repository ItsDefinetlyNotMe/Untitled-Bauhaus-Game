using UnityEngine.InputSystem;
using UnityEngine;


public abstract class WeaponScript : MonoBehaviour
{
    Vector2 playerDirection = new Vector2(-1,0);
    private float attackRange = 1f;
    private float attackSpeed = 2f;
    private int attackDamage = 20;
    [SerializeField] float weaponOffset = 0.5f;
    [SerializeField] LayerMask enemyLayers;
    float nextAttack = 0f;
    PlayerMovement movementScript;
    PlayerAnimator playerAnimator;

    private void Start()
    {
        movementScript = GetComponentInParent<PlayerMovement>();
        playerAnimator = GetComponentInParent<PlayerAnimator>();
        Debug.Log(playerAnimator);
        transform.localPosition = weaponOffset * playerDirection;
    }
    public void Attack(){
        //tracking the attacktimer and detecting enemys in attackradius if possible to attack
        if(Time.time >= nextAttack){
            //call animation
            playerAnimator.PlayAttackAnimation();
            //Cooldown
            nextAttack = Time.time + 1f/attackSpeed;
            //locating enemies
            transform.localPosition = weaponOffset * playerDirection;
            Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(transform.position,attackRange,enemyLayers);
            foreach (Collider2D enemy in enemiesHit)
            {
               enemy.GetComponent<EnemyScript>().TakeDamage(attackDamage);   
            }
        }
    }

    public abstract void LeftTriggerAttack();//special Attack based on weapon

    private void Update()//TODO dont use the getter try using the actionmap
    {
        //getting the last direction player looked in
        if(movementScript.GetMovementDirection().magnitude > 0)
        {
            playerDirection = movementScript.GetMovementDirection();
            playerDirection.Normalize();
        }
    }
    
    void OnDrawGizmosSelected(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,attackRange);
    }
}

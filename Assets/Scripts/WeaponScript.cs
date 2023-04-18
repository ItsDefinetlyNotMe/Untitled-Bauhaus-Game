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
            playerAnimator.PlayAttackAnimation();

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
        {
            playerDirection = movementScript.movementDirection;
            playerDirection.Normalize();
        }

        Debug.Log(animator.GetCurrentAnimatorClipInfo(0)[0].clip);
    }
    
    void OnDrawGizmosSelected()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawCube(transform.position, 1);
    }
}

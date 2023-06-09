using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HittableObject : MonoBehaviour
{
    public delegate void ObjectDeathDelegate();
    public static ObjectDeathDelegate onObjectDeath;

    protected bool isDead;

    protected Collider2D objectCollider;
    [SerializeField] protected int maxHealth;
    public int currentHealth;
    [SerializeField] protected float size;
    public bool isDying;

    //protected virtual void Awake()
    //{
    //}

    protected virtual void Start()
    {
        objectCollider = GetComponent<Collider2D>();
        currentHealth = maxHealth;
    }

    protected virtual void TakeDamage(int damage,GameObject damageSource)
    {
        //apply damage
        currentHealth -= damage;
        if (currentHealth <= 0 && !isDead && !isDying)
        {
            Die(damageSource);
        }
    }
    public virtual void GetHit(int damage, Vector2 damageSourcePosition, float knockbackMultiplier,GameObject damageSource,bool heavy)
    {
        TakeDamage(damage,damageSource);
    }

    protected virtual void Die(GameObject damageSource)
    {
        if(damageSource.CompareTag("Player"))
            onObjectDeath?.Invoke(); //Invoke event for controller vibration

        isDead = true;
        Bounds myBounds = new Bounds(transform.position,new Vector3(1f,1f,1f) * 2f);
        AstarPath.active.UpdateGraphs (myBounds);
        objectCollider.enabled = false;
    }
    
}

using UnityEngine;

public abstract class HittableObject : MonoBehaviour
{
    public delegate void ObjectDeathDelegate();
    public static ObjectDeathDelegate onObjectDeath;

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
        if (currentHealth <= 0)
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

        objectCollider.enabled = false;
        //disable enemy
        //gameObject.SetActive(false);
    }
    
}

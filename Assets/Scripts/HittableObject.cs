using UnityEngine;

public abstract class HittableObject : MonoBehaviour
{
    private Collider2D objectCollider;
    [SerializeField] protected int maxHealth;
    protected int currentHealth;
    [SerializeField] protected float size;
    protected virtual void Start()
    {
        objectCollider = GetComponent<Collider2D>();
        currentHealth = maxHealth;
    }
    protected virtual void TakeDamage(int damage)
    {
        //apply damage
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    public virtual void GetHit(int damage, Vector2 damageSourcePosition, float knockbackMultiplier)
    {
        TakeDamage(damage);
    }

    protected virtual void Die()
    {
        objectCollider.enabled = false;
        //disable enemy
        //gameObject.SetActive(false);
    }
    
}

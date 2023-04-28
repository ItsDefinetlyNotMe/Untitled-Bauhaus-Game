using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class HittableObject : MonoBehaviour
{
    [SerializeField] protected int maxHealth;
    protected int currentHealth;
    [SerializeField] protected float size;
    virtual protected void Start()
    {
        currentHealth = maxHealth;
    }
    virtual protected void TakeDamage(int damage)
    {
        //apply damage
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    virtual public void GetHit(int Damage, Vector2 damageSourcePosition, float knockbackMultiplier)
    {
        TakeDamage(Damage);
    }
    virtual public void Die()
    {
        //disable enemy
        gameObject.SetActive(false);
    }
    
}

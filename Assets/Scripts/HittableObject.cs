using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class HittableObject : MonoBehaviour
{
    [SerializeField] int maxHealth;
    int currentHealth;
    float size;
    private void Start()
    {
        currentHealth = maxHealth;
    }
    private void TakeDamage(int damage)
    {
        //apply damage
        currentHealth -= damage;
        if (currentHealth < 0)
        {
            Die();
        }
    }
    public void GetHit(int Damage, Vector2 damageSourcePosition, float knockbackMultiplier)
    {
        TakeDamage(Damage);
        //
        /*
        float sizeMultiplier;
        //knockback enemy
        if(size == 0){//imoveable object
            sizeMultiplier = 0;
        }else{
            sizeMultiplier = 1 / size;
        }
        //stun(knockbackMultiplier);
        //rb.velocity = knockbackDirection * sizeAmplifier * knockbackAmplifier;*/

        //TODO visual feedback
    }
    virtual public void Die()
    {
        //disable enemy
        gameObject.SetActive(false);
    }
    
}

using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int maxHealth;
    int currentHealth;


    public void TakeDamage(int damage, Vector2 knockbackDirection, float knockbackAmplifier)
    {
        //apply damage
        currentHealth -= damage;
        if (currentHealth < 0)
        {
            //Die();
            
        }

        //knockbackDirection enemy
        //float sizeAmplifier = 1 / size;//TODO find a fitting konstant for knockbackDirection
        //stun(knockbackAmplifier);
        //rb.velocity = knockbackDirection * sizeAmplifier * knockbackAmplifier;

        //TODO visual feedback


    }
    private void Start()
    {
        currentHealth = maxHealth;
    }
}

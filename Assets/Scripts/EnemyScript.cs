using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    int maxHealth = 100;
    int currentHealth;
    float size = 1f; 
    Rigidbody2D rb;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int dmg,Vector2 knockback)
    {
        //knockback enemy
        float sizeAmplifier = 1 / size;//TODO find a fitting konstant for knockback
        rb.velocity = knockback * sizeAmplifier;
        
        //TODO visual feedback


        //apply damage
        currentHealth -= dmg;
        if(currentHealth < 0)
        {
            Death();
        }
    }
    private void Death(){
        //TODO play death animation

        //disable enemy
        gameObject.SetActive(false);
    }
}

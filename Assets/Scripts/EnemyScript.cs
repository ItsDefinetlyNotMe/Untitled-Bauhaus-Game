using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    int maxHealth = 100;
    int currentHealth;
    float size = 1f;
    Rigidbody2D rb;
    GameObject player;
    WeaponScript weaponScript;


    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Mario"); //TODO Mario won't be Mario forever
        weaponScript = GameObject.Find("Mario/Weapon").GetComponent<WeaponScript>(); //TODO Don't have damage number in weapon script please (Maybe in GameManager?)
        
    }

    public void TakeDamage(int dmg, Vector2 knockbackDirection, float knockbackAmplifier)
    {
        //knockbackDirection enemy
        float sizeAmplifier = 1 / size;//TODO find a fitting konstant for knockbackDirection
        stun(knockbackAmplifier);
        rb.velocity = knockbackDirection * sizeAmplifier * knockbackAmplifier;

        //TODO visual feedback


        //apply damage
        currentHealth -= dmg;
        if (currentHealth < 0)
        {
            Death();
        }
    }
    private void Death()
    {
        //TODO play death animation

        //disable enemy
        gameObject.SetActive(false);
    }
    private void stun(float amplifier)
    {
        //stunning for a certain time
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Attack")
            TakeDamage(weaponScript.attackDamage, player.transform.position - transform.position, weaponScript.knockbackAmplifier);
    }
}

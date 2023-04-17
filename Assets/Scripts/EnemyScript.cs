using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    int maxHealth = 100;
    int currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        if(currentHealth < 0)
        {
            Debug.Log("Dead");
        }
    }
}

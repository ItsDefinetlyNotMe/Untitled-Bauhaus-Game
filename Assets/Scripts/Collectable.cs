using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [Header("Item Bonuses")]
    [SerializeField] private int maxHealthValue;
    [SerializeField] private float healPercentage;
    [SerializeField] private float damageMultiplierValue;
    [SerializeField] private int moneyValue;

    Rigidbody2D rb;
    PlayerStats playerStats;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerStats = FindObjectOfType<PlayerStats>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector2 playerPos = collision.transform.position;
            Vector2 direction = new Vector2(playerPos.x - transform.position.x, playerPos.y - transform.position.y);
            rb.AddForce((6 - direction.magnitude) * direction.normalized);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (name)
        {
            case "CollectableMaxHealthUp":
                print("HealthUp");
                playerStats.SetMaxHealthRunBonus(maxHealthValue);
                collision.gameObject.GetComponent<HitablePlayer>().currentHealth += maxHealthValue;
                break;

            case "CollectableHeal":
                print("Heal");
                collision.gameObject.GetComponent<HitablePlayer>().HealByPercentage(healPercentage);
                break;

            case "CollectableMynt":
                print("GetRich");
                playerStats.AddMoney(moneyValue);
                break;

            case "CollectableDamageUp":
                print("DamageUp");
                playerStats.SetdamageMultiplierRunBonus(damageMultiplierValue);
                break;
        }
        Destroy(gameObject);
    }
}

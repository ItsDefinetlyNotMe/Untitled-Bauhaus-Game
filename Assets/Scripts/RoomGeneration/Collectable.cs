using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TestRandomWorldGeneration;
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
        if (collision.gameObject.CompareTag("Player"))
        {
            HitablePlayer hitablePlayer = collision.gameObject.GetComponent<HitablePlayer>();
            PlayerAttack playerAttack = collision.gameObject.GetComponent<PlayerAttack>();

            GameObject inGameCanvas = GameObject.Find("/InGameCanvas");

            switch (Regex.Replace(gameObject.name, "\\(Clone\\)", ""))
            {
                case "CollectableMaxHealthUp":
                    inGameCanvas.transform.GetChild(4).gameObject.SetActive(true);
                    playerStats.SetMaxHealthRunBonus(maxHealthValue);                    
                    hitablePlayer.currentHealth += maxHealthValue;
                    hitablePlayer.LoadStats();
                    break;

                case "CollectableHeal":
                    inGameCanvas.transform.GetChild(5).gameObject.SetActive(true);
                    hitablePlayer.HealByPercentage(healPercentage);
                    hitablePlayer.LoadStats();
                    break;

                case "CollectableMynt":
                    inGameCanvas.transform.GetChild(2).gameObject.SetActive(true);
                    playerStats.AddMoney(moneyValue);
                    break;

                case "CollectableDamageUp":
                    inGameCanvas.transform.GetChild(3).gameObject.SetActive(true);
                    playerStats.SetdamageMultiplierRunBonus(damageMultiplierValue);
                    playerAttack.LoadStats();
                    break;
            }
            FindObjectOfType<CreateRandomRoomLayout>().OpenDoors();

            Destroy(gameObject);
        }
    }
}

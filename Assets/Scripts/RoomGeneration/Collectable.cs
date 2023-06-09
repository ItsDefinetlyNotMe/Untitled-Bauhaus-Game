using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TestRandomWorldGeneration;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Update = UnityEngine.PlayerLoop.Update;

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

    private void Update()
    {
        Vector2 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        Vector2 direction = new Vector2(playerPos.x - transform.position.x, playerPos.y - transform.position.y);
        rb.velocity = direction.normalized;
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
                    if (SceneManager.GetActiveScene().name != "ThorBossFight")
                        inGameCanvas.transform.GetChild(2).gameObject.SetActive(true);

                    else
                        inGameCanvas.transform.GetChild(7).gameObject.SetActive(true);

                    playerStats.AddMoney(moneyValue);
                    break;

                case "CollectableDamageUp":
                    inGameCanvas.transform.GetChild(3).gameObject.SetActive(true);
                    playerStats.SetdamageMultiplierRunBonus(damageMultiplierValue);
                    playerAttack.LoadStats();
                    break;
            }

            if (SceneManager.GetActiveScene().name != "ThorBossFight")
                FindObjectOfType<CreateRandomRoomLayout>().OpenDoors();            

            Destroy(gameObject);
        }
    }
}

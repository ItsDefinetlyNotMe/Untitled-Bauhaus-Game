using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private int baseMaxHealth;
    public float damageMultiplier;

    private int maxHealthRunBonus;

    private HitablePlayer hitablePlayer;
    private GameManager gameManager;

    private void Start()
    {
        hitablePlayer = GetComponent<HitablePlayer>();
        gameManager = FindObjectOfType<GameManager>();
    }

    public void SetMaxHealthRunBonus(int bonus)
    {
        maxHealthRunBonus += bonus;

        hitablePlayer.LoadStats();
    }

    public int getMaxHealth()
    {
        return baseMaxHealth + PlayerPrefs.GetInt("maxHealth" + gameManager.saveSlot) + maxHealthRunBonus;
    }
}

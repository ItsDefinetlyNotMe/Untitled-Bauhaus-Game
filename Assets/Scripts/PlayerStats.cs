using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private int baseMaxHealth;

    public int money { get; private set; }
    private int maxHealthRunBonus;
    private float damageMultiplierRunBonus = 1;

    private HitablePlayer hitablePlayer;
    private PlayerAttack playerAttack;
    private GameManager gameManager;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        hitablePlayer = GetComponent<HitablePlayer>();
        playerAttack = GetComponent<PlayerAttack>();
        gameManager = FindObjectOfType<GameManager>();
    }

    public void AddMoney(int amount)
    {
        int oldMoney = PlayerPrefs.GetInt("money" + gameManager.saveSlot);
        PlayerPrefs.SetInt("money" + gameManager.saveSlot, oldMoney + amount);

        PlayerPrefs.Save(); //Save changes in playerPrefs


        //TODO: call money UI refresh
    }

    public void SetMaxHealthRunBonus(int bonus)
    {
        maxHealthRunBonus += bonus;
    }

    public int getMaxHealth()
    {
        return baseMaxHealth + PlayerPrefs.GetInt("maxHealth" + gameManager.saveSlot) + maxHealthRunBonus;
    }

    public void SetdamageMultiplierRunBonus(float bonus)
    {
        damageMultiplierRunBonus += bonus;
    }

    public float getDamageMultiplier()
    {
        return PlayerPrefs.GetInt("damageMultiplier" + gameManager.saveSlot) + damageMultiplierRunBonus;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "HUB")
        {
            maxHealthRunBonus = 0;
            damageMultiplierRunBonus = 1;
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

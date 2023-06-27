using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private int baseMaxHealth;

    //money
    public int money { get; private set; }
    public int runMoney { get; private set; } = 0;

    //maxHealth
    private int maxHealthRunBonus;
    private float damageMultiplierRunBonus = 0;

    //crit
    private float critMultiplier = 1.5f;

    private GameManager gameManager;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public bool AddMoney(int amount)
    {
        gameManager = FindObjectsByType<GameManager>(FindObjectsSortMode.InstanceID)[0];

        if (SceneManager.GetActiveScene().name != "HUB")
            runMoney += amount;

        int oldMoney = PlayerPrefs.GetInt("money" + gameManager.saveSlot);

        if (-oldMoney > amount)
        {
            return false;
        }

        PlayerPrefs.SetInt("money" + gameManager.saveSlot, oldMoney + amount);

        PlayerPrefs.Save(); //Save changes in playerPrefs


        MoneyUI moneyUI = FindObjectOfType<MoneyUI>();
        moneyUI.LoadMoney();

        return true;
    }

    public void SetMaxHealthRunBonus(int bonus)
    {
        maxHealthRunBonus += bonus;
    }

    public int getMaxHealth()
    {
        gameManager = FindObjectsByType<GameManager>(FindObjectsSortMode.InstanceID)[0];
        return baseMaxHealth + PlayerPrefs.GetInt("maxHealth" + gameManager.saveSlot) + maxHealthRunBonus;
    }

    public void SetdamageMultiplierRunBonus(float bonus)
    {
        damageMultiplierRunBonus += bonus;
    }

    public float getDamageMultiplier()
    {
        gameManager = FindObjectsByType<GameManager>(FindObjectsSortMode.InstanceID)[0];
        return PlayerPrefs.GetInt("damageMultiplier" + gameManager.saveSlot) / 100.0f + damageMultiplierRunBonus;
    }

    public float GetCritMultiplier()
    {
        gameManager = FindObjectsByType<GameManager>(FindObjectsSortMode.InstanceID)[0];
        float critChance = PlayerPrefs.GetInt("critChance" + gameManager.saveSlot) / 100.0f;

        if (Random.Range(0f, 1f) <= critChance)
            return critMultiplier;
        return 1;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "HUB")
        {
            maxHealthRunBonus = 0;
            damageMultiplierRunBonus = 0;
            runMoney = 0;
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

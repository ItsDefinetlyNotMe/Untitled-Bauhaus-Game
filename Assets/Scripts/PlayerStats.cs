using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private int baseMaxHealth;

    public int money { get; private set; }
    private int maxHealthRunBonus;
    private float damageMultiplierRunBonus = 0;
    public int runMoney { get; private set; } = 0;

    private HitablePlayer hitablePlayer;
    private PlayerAttack playerAttack;
    private GameManager gameManager;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        hitablePlayer = GetComponent<HitablePlayer>();
        playerAttack = GetComponent<PlayerAttack>();
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UpgradeWindow : MonoBehaviour
{
    private GameManager gameManager;
    private GameObject upgradeWindow;

    private int maxHealthUpgrade = 10;
    private int maxHealthBonus = 0;
    private int maxHealthBasePrice = 0;

    private int damageMultiplierUpgrade = 25;
    private int damageMultiplierBonus = 0;
    private int damageMultiplierBasePrice = 0;

    private int critChanceUpgrade = 2;
    private int critChancePercentage;
    private int critChanceBasePrice = 0;

    private int critDamageUpgrade = 25;
    private int critDamagePercentage;

    public AudioSource audioSource;
    public AudioClip clickSound;
    public AudioClip locked;

    private bool firstOnClick;

    public void Interact()
    {
        PlayerInput playerInput = FindObjectOfType<PlayerInput>();
        playerInput.actions.FindActionMap("Movement").Disable();

        upgradeWindow.SetActive(true);

        firstOnClick = true;

        UpdateMaxHealth();
        UpdateDamageMultiplier();
        UpdateCritChance();
        UpdateCritDamage();
        UpdateCloneAbility();
    }

    public void Back()
    {
        upgradeWindow.SetActive(false);

        PlayerInput playerInput = FindObjectOfType<PlayerInput>();
        playerInput.actions.FindActionMap("Movement").Enable();
    }

    public void IncreaseMaxHealth()
    {
        if (FindObjectsByType<GameManager>(FindObjectsSortMode.InstanceID).Length > 1)
        {
            print("ERROR: Too many gameManagers");
            return;
        }
        
        PlayerInput playerInput = FindObjectOfType<PlayerInput>();
        if (playerInput.currentControlScheme == "Gamepad")
        {
            if (firstOnClick)
            {
                firstOnClick = false;
                return;
            }
        }

        gameManager = FindObjectsByType<GameManager>(FindObjectsSortMode.InstanceID)[0];

        int price = int.Parse(transform.GetChild(0).GetChild(1).GetChild(2).GetChild(0).GetComponent<TMP_Text>().text);
        PlayerStats stats = FindObjectOfType<PlayerStats>();

        if (!stats.AddMoney(-price))
        {
            audioSource.PlayOneShot(locked);
            return;
        }

        maxHealthBonus = PlayerPrefs.GetInt("maxHealth" + gameManager.saveSlot) + maxHealthUpgrade; //Get value and add new bonus
        PlayerPrefs.SetInt("maxHealth" + gameManager.saveSlot, maxHealthBonus); //Set new value

        PlayerPrefs.Save(); //Save changes in playerPrefs

        UpdateMaxHealth();
    }

    private void UpdateMaxHealth()
    {
        if (FindObjectsByType<GameManager>(FindObjectsSortMode.InstanceID).Length > 1)
        {
            print("ERROR: Too many gameManagers");
            return;
        }

        gameManager = FindObjectsByType<GameManager>(FindObjectsSortMode.InstanceID)[0];

        Transform maxHealthDisplay = transform.GetChild(0).GetChild(1);
        maxHealthDisplay.GetChild(1).GetComponent<TMP_Text>().text = PlayerPrefs.GetInt("maxHealth" + gameManager.saveSlot).ToString();
        TMP_Text text = maxHealthDisplay.GetChild(2).GetChild(0).GetComponent<TMP_Text>();

        int newPrice = maxHealthBasePrice;
        for (int i = 0; i < PlayerPrefs.GetInt("maxHealth" + gameManager.saveSlot) / 10; i++)
        {
            newPrice = (int)(newPrice * 1.1);
            audioSource.PlayOneShot(clickSound);
        }

        text.text = newPrice.ToString();
    }

    public void IncreaseDamageMultiplier()
    {
        if (FindObjectsByType<GameManager>(FindObjectsSortMode.InstanceID).Length > 1)
        {
            print("ERROR: Too many gameManagers");
            return;
        }

        gameManager = FindObjectsByType<GameManager>(FindObjectsSortMode.InstanceID)[0];

        int price = int.Parse(transform.GetChild(0).GetChild(2).GetChild(2).GetChild(0).GetComponent<TMP_Text>().text);
        PlayerStats stats = FindObjectOfType<PlayerStats>();

        if (!stats.AddMoney(-price))
        {
            audioSource.PlayOneShot(locked);
            return;
        }

        damageMultiplierBonus = PlayerPrefs.GetInt("damageMultiplier" + gameManager.saveSlot) + damageMultiplierUpgrade; //Get value and add new bonus
        PlayerPrefs.SetInt("damageMultiplier" + gameManager.saveSlot, damageMultiplierBonus); //Set new value

        PlayerPrefs.Save(); //Save changes in playerPrefs

        UpdateDamageMultiplier();
    }

    public void UpdateDamageMultiplier()
    {
        if (FindObjectsByType<GameManager>(FindObjectsSortMode.InstanceID).Length > 1)
        {
            print("ERROR: Too many gameManagers");
            return;
        }

        gameManager = FindObjectsByType<GameManager>(FindObjectsSortMode.InstanceID)[0];

        Transform damageMultiplierDisplay = transform.GetChild(0).GetChild(2);
        damageMultiplierDisplay.GetChild(1).GetComponent<TMP_Text>().text = PlayerPrefs.GetInt("damageMultiplier" + gameManager.saveSlot).ToString() + "%";
        TMP_Text text = damageMultiplierDisplay.GetChild(2).GetChild(0).GetComponent<TMP_Text>();

        int newPrice = damageMultiplierBasePrice;
        for (int i = 0; i < (PlayerPrefs.GetInt("damageMultiplier" + gameManager.saveSlot) - 100) / 25; i++)
        {
            newPrice = (int)(newPrice * 1.1);
            audioSource.PlayOneShot(clickSound);
        }

        text.text = newPrice.ToString();
    }

    public void IncreaseCritDamage()
    {
        if (FindObjectsByType<GameManager>(FindObjectsSortMode.InstanceID).Length > 1)
        {
            print("ERROR: Too many gameManagers");
            return;
        }

        gameManager = FindObjectsByType<GameManager>(FindObjectsSortMode.InstanceID)[0];

        int price = int.Parse(transform.GetChild(0).GetChild(6).GetChild(2).GetChild(0).GetComponent<TMP_Text>().text);
        PlayerStats stats = FindObjectOfType<PlayerStats>();

        if (!stats.AddMoney(-price))
        {
            audioSource.PlayOneShot(locked);
            return;
        }

        critDamagePercentage = PlayerPrefs.GetInt("critDamage" + gameManager.saveSlot) + critDamageUpgrade; //Get value and add new bonus
        PlayerPrefs.SetInt("critDamage" + gameManager.saveSlot, critDamagePercentage); //Set new value

        PlayerPrefs.Save(); //Save changes in playerPrefs

        UpdateCritDamage();
    }

    private void UpdateCritDamage()
    {
        if (FindObjectsByType<GameManager>(FindObjectsSortMode.InstanceID).Length > 1)
        {
            print("ERROR: Too many gameManagers");
            return;
        }

        gameManager = FindObjectsByType<GameManager>(FindObjectsSortMode.InstanceID)[0];

        Transform critDamageDisplay = transform.GetChild(0).GetChild(6);

        // round it to 2 decimal places
        int critDamage = PlayerPrefs.GetInt("critDamage" + gameManager.saveSlot);
        float critDamageFloat = critDamage / 100f;
        float roundedCritDamage = (float)Math.Round(critDamageFloat, 2);
        critDamageDisplay.GetChild(1).GetComponent<TMP_Text>().text = "x" + roundedCritDamage.ToString();
        TMP_Text text = critDamageDisplay.GetChild(2).GetChild(0).GetComponent<TMP_Text>();

        int newPrice = critChanceBasePrice;
        for (int i = 0; i < (PlayerPrefs.GetInt("critDamage" + gameManager.saveSlot) - 100) / 25; i++)
        {
            newPrice = (int)(newPrice * 1.3);
            audioSource.PlayOneShot(clickSound);
        }

        text.text = newPrice.ToString();
    }

    public void IncreaseCritChance()
    {
        if (FindObjectsByType<GameManager>(FindObjectsSortMode.InstanceID).Length > 1)
        {
            print("ERROR: Too many gameManagers");
            return;
        }

        gameManager = FindObjectsByType<GameManager>(FindObjectsSortMode.InstanceID)[0];

        int price = int.Parse(transform.GetChild(0).GetChild(5).GetChild(2).GetChild(0).GetComponent<TMP_Text>().text);
        PlayerStats stats = FindObjectOfType<PlayerStats>();

        if (!stats.AddMoney(-price))
        {
            audioSource.PlayOneShot(locked);
            return;
        }

        critChancePercentage = PlayerPrefs.GetInt("critChance" + gameManager.saveSlot) + critChanceUpgrade; //Get value and add new bonus
        PlayerPrefs.SetInt("critChance" + gameManager.saveSlot, critChancePercentage); //Set new value

        PlayerPrefs.Save(); //Save changes in playerPrefs

        UpdateCritChance();
    }

    private void UpdateCritChance()
    {
        if (FindObjectsByType<GameManager>(FindObjectsSortMode.InstanceID).Length > 1)
        {
            print("ERROR: Too many gameManagers");
            return;
        }

        gameManager = FindObjectsByType<GameManager>(FindObjectsSortMode.InstanceID)[0];

        Transform critChanceDisplay = transform.GetChild(0).GetChild(5);
        critChanceDisplay.GetChild(1).GetComponent<TMP_Text>().text = PlayerPrefs.GetInt("critChance" + gameManager.saveSlot).ToString() + "%";

        if (PlayerPrefs.GetInt("critChance" + gameManager.saveSlot) >= 100)
        {
            critChanceDisplay.GetChild(2).gameObject.SetActive(false);
            return;
        }

        TMP_Text text = critChanceDisplay.GetChild(2).GetChild(0).GetComponent<TMP_Text>();
        int newPrice = critChanceBasePrice;
        for (int i = 0; i < PlayerPrefs.GetInt("critChance" + gameManager.saveSlot) / 2; i++)
        {
            newPrice = (int)(newPrice * 1.1);
            audioSource.PlayOneShot(clickSound);
        }

        text.text = newPrice.ToString();
    }

    public void BuyCloneAbility()
    {
        if (FindObjectsByType<GameManager>(FindObjectsSortMode.InstanceID).Length > 1)
        {
            print("ERROR: Too many gameManagers");
            return;
        }

        gameManager = FindObjectsByType<GameManager>(FindObjectsSortMode.InstanceID)[0];

        int price = int.Parse(transform.GetChild(0).GetChild(7).GetChild(2).GetChild(0).GetComponent<TMP_Text>().text);
        PlayerStats stats = FindObjectOfType<PlayerStats>();

        if (!stats.AddMoney(-price))
        {
            audioSource.PlayOneShot(locked);
            return;
        }

        PlayerPrefs.SetInt("cloneAbility" + gameManager.saveSlot, 1); //Set to true

        PlayerPrefs.Save(); //Save changes in playerPrefs

        UpdateCloneAbility();
    }

    private void UpdateCloneAbility()
    {
        if (FindObjectsByType<GameManager>(FindObjectsSortMode.InstanceID).Length > 1)
        {
            print("ERROR: Too many gameManagers");
            return;
        }

        gameManager = FindObjectsByType<GameManager>(FindObjectsSortMode.InstanceID)[0];

        if (PlayerPrefs.GetInt("cloneAbility" + gameManager.saveSlot) == 1)
        {
            //activate toggle
            transform.GetChild(0).GetChild(7).GetChild(1).GetComponent<Toggle>().isOn = true;

            //deactivate purchase button
            transform.GetChild(0).GetChild(7).GetChild(2).gameObject.SetActive(false);
        }
        
        else
        {
            //deactivate toggle
            transform.GetChild(0).GetChild(7).GetChild(1).GetComponent<Toggle>().isOn = false;

            //activate purchase button
            transform.GetChild(0).GetChild(7).GetChild(2).gameObject.SetActive(true);
        }
        
    }


    private void Awake()
    {
        maxHealthBasePrice = int.Parse(transform.GetChild(0).GetChild(1).GetChild(2).GetChild(0).GetComponent<TMP_Text>().text);
        damageMultiplierBasePrice = int.Parse(transform.GetChild(0).GetChild(2).GetChild(2).GetChild(0).GetComponent<TMP_Text>().text);
        critChanceBasePrice = int.Parse(transform.GetChild(0).GetChild(5).GetChild(2).GetChild(0).GetComponent<TMP_Text>().text);

        if (PlayerPrefs.GetInt("damageMultiplier" + gameManager.saveSlot) == 0)
            PlayerPrefs.SetInt("damageMultiplier" + gameManager.saveSlot, 100);

        if (PlayerPrefs.GetInt("critDamage" + gameManager.saveSlot) == 0)
            PlayerPrefs.SetInt("critDamage" + gameManager.saveSlot, 150);
    }

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        upgradeWindow = transform.GetChild(0).gameObject;

        if (PlayerPrefs.GetInt("damageMultiplier" + gameManager.saveSlot) < 100)
            PlayerPrefs.SetInt("damageMultiplier" + gameManager.saveSlot, 100);

        PlayerPrefs.Save();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerInput playerInput = FindObjectOfType<PlayerInput>();
            playerInput.actions.FindActionMap("Fighting").Disable();
            playerInput.actions.FindActionMap("UI").Enable();

            InputHandler inputHandler = FindObjectOfType<InputHandler>();
            inputHandler.isOnUpgrade = true;
            
            if (playerInput.currentControlScheme == "Keyboard + Mouse")
            {
                transform.GetChild(9).GetChild(1).gameObject.SetActive(true);
            }

            if (playerInput.currentControlScheme == "Gamepad")
            {
                transform.GetChild(9).GetChild(0).gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerInput playerInput = FindObjectOfType<PlayerInput>();
            playerInput.actions.FindActionMap("Fighting").Enable();
            playerInput.actions.FindActionMap("UI").Disable();

            InputHandler inputHandler = FindObjectOfType<InputHandler>();
            inputHandler.isOnUpgrade = false;
            
            transform.GetChild(9).GetChild(0).gameObject.SetActive(false);
            transform.GetChild(9).GetChild(1).gameObject.SetActive(false);


            upgradeWindow.SetActive(false);
        }
    }
}
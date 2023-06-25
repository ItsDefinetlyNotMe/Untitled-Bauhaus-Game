using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UpgradeWindow : MonoBehaviour
{
    private GameManager gameManager;
    private GameObject upgradeWindow;

    private int maxHealthUpgrade = 10;
    private int maxHealthBonus = 0;
    private int maxHealthBasePrice = 0;
    public AudioSource audioSource;
    public AudioClip clickSound;
    public AudioClip locked;

    public void Interact()
    {
        PlayerInput playerInput = FindObjectOfType<PlayerInput>();
        playerInput.actions.FindActionMap("Movement").Disable();

        upgradeWindow.SetActive(true);

        UpdateMaxHealth();
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
        int price = int.Parse(transform.GetChild(0).GetChild(2).GetChild(2).GetChild(0).GetComponent<TMP_Text>().text);
        PlayerStats stats = FindObjectOfType<PlayerStats>();

        if (!stats.AddMoney(-price))
        {
            audioSource.PlayOneShot(locked);
            return;
        }

        print("Increase Damage Multiplier");
    }

    private void Awake()
    {
        maxHealthBasePrice = int.Parse(transform.GetChild(0).GetChild(1).GetChild(2).GetChild(0).GetComponent<TMP_Text>().text);
    }

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        upgradeWindow = transform.GetChild(0).gameObject;

        //PlayerPrefs.SetInt("maxHealth" + gameManager.saveSlot, 0);
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
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIButton : MonoBehaviour, ISelectHandler
{
    private UpgradeWindow upgradeWindow;
    private GameManager gameManager;

    public void SelectThisButton()
    {
        GetComponent<Button>().Select();
    }

    public void IncreaseMaxHealth()
    {
        upgradeWindow.IncreaseMaxHealth();
    }

    public void IncreaseDamageMultiplier()
    {
        upgradeWindow.IncreaseDamageMultiplier();
    }

    public void IncreaseCritChance()
    {
        upgradeWindow.IncreaseCritChance();
    }

    public void IncreaseCritDamage()
    {
        upgradeWindow.IncreaseCritDamage();
    }

    public void LoadSave(int slotNumber)
    {
        gameManager.saveSlot = slotNumber;
        SceneManager.LoadScene("HUB");
    }

    public void BackToMainMenu()
    {
        PlayerInput playerInput = FindObjectOfType<PlayerInput>();
        playerInput.actions.FindActionMap("Fighting").Enable();
        playerInput.actions.FindActionMap("Movement").Enable();
        playerInput.actions.FindActionMap("UI").Disable();

        FindObjectOfType<InputHandler>().isInPauseMenu = false;

        gameObject.transform.parent.gameObject.SetActive(false);
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void BackToHUB()
    {
        PlayerInput playerInput = FindObjectOfType<PlayerInput>();
        playerInput.actions.FindActionMap("Fighting").Enable();
        playerInput.actions.FindActionMap("Movement").Enable();
        playerInput.actions.FindActionMap("UI").Disable();

        FindObjectOfType<InputHandler>().isInPauseMenu = false;

        gameObject.transform.parent.gameObject.SetActive(false);
        Time.timeScale = 1;
        SceneManager.LoadScene("HUB");
    }

    public void BackToPauseMenu()
    {
        GameObject pauseCanvas = GameObject.Find("PauseMenuCanvas");
        pauseCanvas.transform.GetChild(0).gameObject.SetActive(true);
        pauseCanvas.transform.GetChild(1).gameObject.SetActive(false);
    }

    public void PauseSettings()
    {
        GameObject pauseCanvas = GameObject.Find("PauseMenuCanvas");
        pauseCanvas.transform.GetChild(0).gameObject.SetActive(false);
        pauseCanvas.transform.GetChild(1).gameObject.SetActive(true);
    }

    public void ResumeGame()
    {
        PlayerInput playerInput = FindObjectOfType<PlayerInput>();
        playerInput.actions.FindActionMap("Fighting").Enable();
        playerInput.actions.FindActionMap("Movement").Enable();
        playerInput.actions.FindActionMap("UI").Disable();

        FindObjectOfType<InputHandler>().isInPauseMenu = false;

        GameObject pauseCanvas = GameObject.Find("PauseMenuCanvas");
        pauseCanvas.transform.GetChild(0).gameObject.SetActive(false);
        pauseCanvas.transform.GetChild(1).gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    private void OnEnable()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "MainMenu")
        {
            gameManager = GameObject.Find("/GameManager").GetComponent<GameManager>();
        }

        else if (sceneName == "HUB")
        {
            upgradeWindow = FindObjectOfType<UpgradeWindow>();
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
            FindObjectOfType<MainMenu>().selectedButton = name;
    }
}

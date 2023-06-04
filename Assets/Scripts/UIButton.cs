using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIButton : MonoBehaviour
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

    public void LoadSave(int slotNumber)
    {
        gameManager.saveSlot = slotNumber;
        SceneManager.LoadScene("HUB");
    }

    public void BackToMainMenu()
    {
        gameObject.SetActive(false);
        SceneManager.LoadScene("MainMenu");
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
}

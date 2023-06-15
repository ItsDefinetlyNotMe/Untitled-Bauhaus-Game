using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    GameObject startButtons;
    GameObject slotButtons;
    GameObject settingsButtons;
    GameObject slotDeletionConfirmationButtons;

    private string slotNumber;

    public string selectedButton { private get; set; }

    private void Start()
    {
        startButtons = transform.GetChild(1).gameObject;
        slotButtons = transform.GetChild(2).gameObject;
        settingsButtons = transform.GetChild(3).gameObject;
        slotDeletionConfirmationButtons = transform.GetChild(4).gameObject;
    }

    public void Play()
    {
        startButtons.SetActive(false);
        slotButtons.SetActive(true);
        settingsButtons.SetActive(false);
    }

    public void Settings()
    {
        startButtons.SetActive(false);
        slotButtons.SetActive(false);
        settingsButtons.SetActive(true);
    }

    public void BackToMainMenu()
    {
        startButtons.SetActive(true);
        slotButtons.SetActive(false);
        settingsButtons.SetActive(false);
    }

    public void SlotDeletionConfirmatiion()
    {
        if (slotButtons.activeSelf)
        {
            slotNumber = Regex.Replace(Regex.Replace(selectedButton, "Slot", ""), "Button", "");
            slotButtons.SetActive(false);
            slotDeletionConfirmationButtons.SetActive(true);
        }
    }

    public void BackToSlotMenu()
    {
        slotButtons.SetActive(true);
        slotDeletionConfirmationButtons.SetActive(false);
    }

    public void DeleteSlot()
    {
        FindObjectOfType<GameManager>().DeleteSaveOfSlot(slotNumber);
        BackToSlotMenu();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UpgradeWindow : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputHandler inputHandler;
    private GameObject upgradeWindow;

    public void Interact()
    {
        upgradeWindow.SetActive(true);

        playerInput.actions.FindActionMap("Movement").Disable();
    }

    public void Back()
    {
        upgradeWindow.SetActive(false);

        playerInput.actions.FindActionMap("Movement").Enable();
    }

    private void Start()
    {
        inputHandler = FindObjectOfType<InputHandler>();
        playerInput = FindObjectOfType<PlayerInput>();

        upgradeWindow = transform.GetChild(0).gameObject;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInput.actions.FindActionMap("Fighting").Disable();
            playerInput.actions.FindActionMap("UI").Enable();

            inputHandler.isOnUpgrade = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInput.actions.FindActionMap("Fighting").Enable();
            playerInput.actions.FindActionMap("UI").Disable();

            inputHandler.isOnUpgrade = false;
        }
    }
}

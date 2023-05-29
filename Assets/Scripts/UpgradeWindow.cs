using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UpgradeWindow : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputHandler inputHandler;

    public void Interact()
    {
        print("Interact");
    }

    private void Start()
    {
        inputHandler = FindObjectOfType<InputHandler>();
        playerInput = FindObjectOfType<PlayerInput>();
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
            playerInput.actions.FindActionMap("UI").Disable();
            playerInput.actions.FindActionMap("Fighting").Enable();

            inputHandler.isOnUpgrade = false;
        }
    }
}

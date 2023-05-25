using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UpgradeWindow : MonoBehaviour
{
    private PlayerInput playerInput;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInput = collision.gameObject.GetComponent<PlayerInput>();
            playerInput.actions.FindActionMap("Fighting").Disable();
            playerInput.actions.FindActionMap("UI").Enable();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInput = collision.gameObject.GetComponent<PlayerInput>();
            playerInput.actions.FindActionMap("UI").Disable();
            playerInput.actions.FindActionMap("Fighting").Enable();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerInput playerInput = FindObjectOfType<PlayerInput>();
        playerInput.actions.FindActionMap("Fighting").Disable();
        playerInput.actions.FindActionMap("UI").Enable();
        
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        PlayerInput playerInput = FindObjectOfType<PlayerInput>();
        playerInput.actions.FindActionMap("Fighting").Enable();
        playerInput.actions.FindActionMap("UI").Disable();
        
        FindObjectOfType<DialogueManager>().EndDialogue();
    }
}


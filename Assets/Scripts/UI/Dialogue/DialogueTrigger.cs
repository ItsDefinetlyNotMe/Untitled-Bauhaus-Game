using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private Dialogue dialogue;

    private int saveslot;
    
    private void Start()
    {
        saveslot = FindObjectOfType<GameManager>().saveSlot;
        if (PlayerPrefs.GetInt("boolFirstTimeTalk" + dialogue.name + saveslot) == 0)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerInput playerInput = FindObjectOfType<PlayerInput>();
        playerInput.actions.FindActionMap("Fighting").Disable();
        playerInput.actions.FindActionMap("UI").Enable();
                
        transform.GetChild(0).gameObject.SetActive(false);
        
        if (PlayerPrefs.GetInt("boolFirstTimeTalk" + dialogue.name + saveslot) == 0)
        {
            FindObjectOfType<DialogueManager>().StartDialogue(dialogue.firstTimeSentences, dialogue.name);
                    
            PlayerPrefs.SetInt("boolFirstTimeTalk" + dialogue.name + saveslot, 1);
            PlayerPrefs.Save();
        }
        else
        {
            FindObjectOfType<DialogueManager>().StartDialogue(dialogue.loopSentences, dialogue.name);
        }  
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerInput playerInput = FindObjectOfType<PlayerInput>();
        playerInput.actions.FindActionMap("Fighting").Enable();
        playerInput.actions.FindActionMap("UI").Disable();
                
        FindObjectOfType<DialogueManager>().EndDialogue();
    }
}


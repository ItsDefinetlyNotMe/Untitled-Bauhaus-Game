using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text dialogueText;
    
    private Queue<string> sentences;
    private DialogueTrigger dialogueTrigger;

    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue(string[] dialogueSentences, DialogueTrigger newDialogueTrigger)
    {
        dialogueTrigger = newDialogueTrigger;
        GameObject dialogueSystem = GameObject.Find("/DialogueSystem");
        dialogueSystem.transform.GetChild(0).gameObject.SetActive(true);
        
        nameText.text = dialogueTrigger.dialogue.name;
        
        sentences.Clear();

        foreach (string sentence in dialogueSentences)
        {
            sentences.Enqueue(sentence);
        }
        
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            dialogueTrigger.transform.GetChild(0).gameObject.SetActive(false);
            PlayerPrefs.SetInt("boolFirstTimeTalk" + dialogueTrigger.dialogue.name + dialogueTrigger.saveslot, 1);
            PlayerPrefs.Save();
            
            EndDialogue();
            return;
        }
        
        string sentence = sentences.Dequeue();
        
        //StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence)
        {
            dialogueText.text += letter;
        }
        yield return null;
    }

    public void EndDialogue()
    {
        GameObject dialogueSystem = GameObject.Find("/DialogueSystem");
        dialogueSystem.transform.GetChild(0).gameObject.SetActive(false);
    }
}

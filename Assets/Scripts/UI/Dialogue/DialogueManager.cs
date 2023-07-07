using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text dialogueText;
    
    private Queue<string> sentences;
    
    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue(string[] dialogueSentences, string dialogueName)
    {
        GameObject dialogueSystem = GameObject.Find("/DialogueSystem");
        dialogueSystem.transform.GetChild(0).gameObject.SetActive(true);
        
        nameText.text = dialogueName;
        
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
            yield return null;
        }
    }

    public void EndDialogue()
    {
        GameObject dialogueSystem = GameObject.Find("/DialogueSystem");
        dialogueSystem.transform.GetChild(0).gameObject.SetActive(false);
    }
}

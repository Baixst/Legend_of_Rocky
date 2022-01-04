using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialogueWindow;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    private Queue<string> sentences;
    private DialogueTrigger nextDialogueTrigger;
    private bool dialogueActive = false;

    public PlayerInput playerMovementInputs;
    public PlayerInput dialogueInputs;
    public PlayerMovement playerMovement;

    private bool typingText;
    private string currentSentence;

    void Awake()
    {
        dialogueWindow.SetActive(false);
        sentences = new Queue<string>();
        dialogueInputs.enabled = false;
    }

    public void StartDialogue(Dialogue dialogue, DialogueTrigger nextPartToTrigger)
    {
        if (!dialogueActive)
        {
            OpenDialogueWindow();
        }

        nextDialogueTrigger = nextPartToTrigger;
        nameText.text = dialogue.charName;
        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
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
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        currentSentence = sentence;
        typingText = true;
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.02f);
        }
        typingText = false;
    }

    private void EndDialogue()
    {
        Debug.Log("End of dialogue");
        if (nextDialogueTrigger != null)
        {
            StartDialogue(nextDialogueTrigger.dialogue, nextDialogueTrigger.nextPartToTrigger);
        }
        else
        {
            CloseDialogueWindow();
        }
    }

    private void OpenDialogueWindow()
    {
        dialogueWindow.SetActive(true);
        dialogueActive = true;
        if (playerMovementInputs != null)   playerMovementInputs.enabled = false;
        if (playerMovement != null)         playerMovement.allowMovement = false;
        dialogueInputs.enabled = true;
    }

    private void CloseDialogueWindow()
    {
        dialogueWindow.SetActive(false);
        dialogueActive = false;
        if (playerMovementInputs != null)   playerMovementInputs.enabled = true;
        if (playerMovement != null)         playerMovement.allowMovement = true;
        dialogueInputs.enabled = false;
    }

    public void SubmitPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (typingText)
            {
                StopAllCoroutines();
                dialogueText.text = currentSentence;
                typingText = false;
            }
            else
            {
                DisplayNextSentence();
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialogueWindow;
    public GameObject dialogueArrow;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    private Queue<string> sentences;
    private DialogueTrigger nextDialogueTrigger;

    private bool dialogueActive = false;
    
    [HideInInspector]
    public int dialoguesFinished = 0;

    public PlayerInput playerMovementInputs;
    public PlayerInput dialogueInputs;
    public PlayerMovement playerMovement;

    public CutsceneManager cutsceneManager;

    private bool typingText;
    private string currentSentence;

    void Awake()
    {
        dialogueWindow.SetActive(false);
        dialogueArrow.SetActive(false);
        sentences = new Queue<string>();
        dialogueInputs.enabled = false;
    }

    public void StartDialogue(Dialogue dialogue, DialogueTrigger nextPartToTrigger)
    {
        StartCoroutine(StartDialogueCoroutine(dialogue, nextPartToTrigger));
    }

    private IEnumerator StartDialogueCoroutine(Dialogue dialogue, DialogueTrigger nextPartToTrigger)
    {
        if (!dialogueActive)
        {
            OpenDialogueWindow();
            yield return new WaitForSeconds(0.75f);
            dialogueText.gameObject.SetActive(true);
            nameText.gameObject.SetActive(true);
            dialogueInputs.enabled = true;
        }

        nextDialogueTrigger = nextPartToTrigger;
        nameText.text = dialogue.charName;
        nameText.color = dialogue.charNameFontColor;
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
        dialogueArrow.SetActive(false);
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.035f);
        }
        typingText = false;
        dialogueArrow.SetActive(true);
    }

    private void EndDialogue()
    {
        if (nextDialogueTrigger != null)
        {
            StartDialogue(nextDialogueTrigger.dialogue, nextDialogueTrigger.nextPartToTrigger);
        }
        else
        {
            StartCoroutine(CloseDialogueWindow());
        }
    }

    private void OpenDialogueWindow()
    {
        dialogueWindow.SetActive(true);
        dialogueActive = true;
        if (playerMovementInputs != null)   playerMovementInputs.enabled = false;
        if (playerMovement != null)
        {
            playerMovement.allowMovement = false;
            playerMovement.horizontalMove = 0f;
            playerMovement.animator.SetFloat("Speed", Mathf.Abs(0f));
        }
        dialogueWindow.GetComponent<Animator>().SetTrigger("open");
    }

    private IEnumerator CloseDialogueWindow()
    {
        dialogueText.gameObject.SetActive(false);
        nameText.gameObject.SetActive(false);
        dialogueWindow.GetComponent<Animator>().SetTrigger("close");
        yield return new WaitForSeconds(0.75f);

        dialogueWindow.SetActive(false);
        dialogueActive = false;
        if (playerMovementInputs != null)   playerMovementInputs.enabled = true;
        if (playerMovement != null)         playerMovement.allowMovement = true;
        dialogueInputs.enabled = false;

        dialoguesFinished++;
        if(cutsceneManager != null) cutsceneManager.UpdateAfterDialogue();
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
                dialogueArrow.SetActive(true);
            }
            else
            {
                DisplayNextSentence();
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueNPC : MonoBehaviour
{
    public GameObject tooltip;
    public DialogueTrigger dialogueTrigger;
    private PlayerInput playerInput;

    void Awake()
    {
        tooltip.SetActive(false);
        playerInput = gameObject.GetComponent<PlayerInput>();
        playerInput.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        tooltip.SetActive(true);
        playerInput.enabled = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        tooltip.SetActive(false);
        playerInput.enabled = false;
    }

    public void StartDialogue(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            dialogueTrigger.TriggerDialogue();
            tooltip.SetActive(false);
            playerInput.enabled = false;
        }
    }
}

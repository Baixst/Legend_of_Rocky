using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroSceneManager : MonoBehaviour
{
    public float waitUntilDialogueStart;
    public float waitUntilNextScene;
    public DialogueTrigger firstDialoguePart;

    void Start()
    {
        StartCoroutine(WaitAndTriggerDialogue());
    }

    private IEnumerator WaitAndTriggerDialogue()
    {
        yield return new WaitForSeconds(waitUntilDialogueStart);
        firstDialoguePart.TriggerDialogue();
    }
}

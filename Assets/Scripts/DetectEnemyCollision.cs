using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DetectEnemyCollision : MonoBehaviour
{
    public bool loadFightScene;
    public bool triggerDialogue;
    public DialogueTrigger dialogueTrigger;

    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject otherObject = collision.gameObject;
        if (otherObject.CompareTag("Enemy"))
        {
            Debug.Log("Collision with enemy happend.");

            if (loadFightScene)
            {
                SceneManager.LoadScene("Fight");
            }

            if (triggerDialogue)
            {
                dialogueTrigger.TriggerDialogue();
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DetectEnemyCollision : MonoBehaviour
{
    public bool loadFightScene;
    public bool triggerDialogue;
    public DialogueTrigger dialogueTrigger;
    private bool forceWalkRight = false;
    private SceneLoader sceneLoader;
    private Camera camera;
    private PlayerMovement playerMovement;

    void Start()
    {
        sceneLoader = FindObjectOfType<SceneLoader>();
        camera = FindObjectOfType<Camera>();
        playerMovement = FindObjectOfType<PlayerMovement>();
    }

    void Update()
    {
        if (forceWalkRight)
        {
            playerMovement.horizontalMove = playerMovement.runSpeed;
        }
    }

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

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject otherObject = other.gameObject;
        if (otherObject.CompareTag("SceneEnd"))
        {
            sceneLoader.LoadNextScene();
            camera.transform.SetParent(null);
            forceWalkRight = true;
        }
    }
}

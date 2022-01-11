using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public bool loadFightScene;
    public bool triggerDialogue;
    public DialogueTrigger dialogueTrigger;
    public bool loadPositionFromSaveData;
    private bool forceWalkRight = false;
    private Camera camera;
    private PlayerMovement playerMovement;
    private RockyGameManager gameManager;

    void Awake()
    {
        camera = FindObjectOfType<Camera>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        gameManager = FindObjectOfType<RockyGameManager>();
        
        // set spawn position
        if (loadPositionFromSaveData && gameManager.playerStartPositionSet) 
        {
            gameObject.transform.position = gameManager.playerStartPosition;
        }
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
                gameManager.sceneLoader.LoadSceneByName("Fight");
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
            gameManager.sceneLoader.LoadNextScene();
            camera.transform.SetParent(null);
            forceWalkRight = true;
        }

        if (otherObject.CompareTag("Checkpoint"))
        {
            otherObject.GetComponent<Checkpoint>().SaveGame(this);
        }
    }
}

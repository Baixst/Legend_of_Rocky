using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public bool triggerDialogue;
    public DialogueTrigger dialogueTrigger;
    public bool loadPositionFromSaveData;
    private bool forceWalkRight = false;
    private Camera camera;
    private PlayerMovement playerMovement;
    private RockyGameManager gameManager;
    private Animator animator;

    void Awake()
    {
        camera = FindObjectOfType<Camera>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        gameManager = FindObjectOfType<RockyGameManager>();
        animator = gameObject.GetComponent<Animator>();
        
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
        if (otherObject.CompareTag("DialogueTrigger"))
        {
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
            camera.GetComponent<CameraFollow>().followObject = false;
            forceWalkRight = true;
            return;
        }

        if (otherObject.CompareTag("Checkpoint"))
        {
            otherObject.GetComponent<Checkpoint>().SaveGame(this);
            return;
        }

        if (otherObject.CompareTag("Spikes"))
        {
            animator.SetBool("IsJumping", false);
            animator.SetTrigger("TakeDamage");
            StartCoroutine(Respawn());
        }
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(1f);
        camera.GetComponent<CameraFollow>().followObject = false;
        camera.transform.parent = gameObject.transform;
        gameObject.transform.position = gameManager.GetPlayerSpawnPosition();
        camera.transform.parent = null;
        camera.GetComponent<CameraFollow>().followObject = true;
        animator.SetTrigger("Idle");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public bool triggerDialogue;
    public DialogueTrigger dialogueTrigger;
    public bool loadPositionFromSaveData;
    public CharacterController2D controller2D;
    public bool forceWalkRight = false;
    private Camera camera;
    public PlayerMovement playerMovement;
    private RockyGameManager gameManager;
    private Animator animator;

    void Awake()
    {
        camera = FindObjectOfType<Camera>();
        gameManager = FindObjectOfType<RockyGameManager>();
        animator = gameObject.GetComponent<Animator>();
        if (playerMovement == null)
        {
            playerMovement = FindObjectOfType<PlayerMovement>();
        }
        
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
                triggerDialogue = false;
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
            controller2D.allowJump = false;
            animator.SetBool("IsJumping", false);
            animator.SetTrigger("TakeDamage");
            StartCoroutine(Respawn());
        }

        if (otherObject.CompareTag("DialogueTrigger"))
        {
            if (triggerDialogue)
            {
                triggerDialogue = false;
                dialogueTrigger.TriggerDialogue();
            }
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
        animator.SetBool("IsJumping", false);
        animator.SetTrigger("Idle");
        controller2D.allowJump = true;
    }
}

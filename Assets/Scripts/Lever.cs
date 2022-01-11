using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lever : MonoBehaviour
{
    public List<MoveableByLever> objectsToMove;
    public float changeX;
    public float changeY;
    public float changeDurationsSec;
    public GameObject tooltip;
    public Sprite spriteOn;
    public Sprite spriteOff;
    private SpriteRenderer spriteRenderer;
    private bool isOn;
    private PlayerInput playerInput;
    private bool interactable = true;

    void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        isOn = true;
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

    public void SwitchState()
    {
        if (!interactable)  return;
        
        interactable = false;
        if (isOn)
        {
            spriteRenderer.sprite = spriteOff;
        }
        else
        {
            spriteRenderer.sprite = spriteOn;
        }
        MoveLinkedObjects();
        isOn = !isOn;
    }

    private void MoveLinkedObjects()
    {
        foreach (MoveableByLever wall in objectsToMove)
        {
            if (changeX != 0 || changeY != 0)
            {
                float newX = wall.gameObject.transform.position.x;
                float newY = wall.gameObject.transform.position.y;
                if (wall.state)
                {
                    newX += changeX;
                    newY += changeY;
                }
                else
                {
                    newX -= changeX;
                    newY -= changeY; 
                }
                wall.state = !wall.state;
                
                // would be better to trigger a animation
                StartCoroutine(MoveOverSeconds(wall.gameObject, new Vector3 (newX, newY, wall.gameObject.transform.position.z), changeDurationsSec));
            }
        }
    }

    public IEnumerator MoveOverSeconds (GameObject objectToMove, Vector3 endPosition, float seconds)
    {
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.transform.position;
        while (elapsedTime < seconds)
        {
            objectToMove.transform.position = Vector3.Lerp(startingPos, endPosition, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        objectToMove.transform.position = endPosition;
        interactable = true;
    }
}

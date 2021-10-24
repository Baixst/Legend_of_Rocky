using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D body;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpHight;
    private float scaleX, scaleY, scaleZ;
    private bool grounded;
    private bool moveButtonPressed = false;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        scaleX = transform.localScale.x;
        scaleY = transform.localScale.y;
        scaleZ = transform.localScale.z;
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown("a") || Input.GetKeyDown("d"))
        {
            moveButtonPressed = true;
        }

        if (Input.GetKeyUp("a") || Input.GetKeyUp("d"))
        {
            moveButtonPressed = false;
            body.velocity = new Vector2(0, body.velocity.y);
        }

        //allow horizontal movement
        if (moveButtonPressed)
        {
            body.velocity = new Vector2(horizontalInput * movementSpeed, body.velocity.y);
        }

        // Flip sprite in movement direction
        if (horizontalInput > 0.01f)
        {
            transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
        }
        else if (horizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(-scaleX, scaleY, scaleZ);
        }

        // Allow jumping
        if (Input.GetKey(KeyCode.Space) && grounded)
        {
            Jump();

        }
    }

    private void Jump()
    {
        body.velocity = new Vector2(body.velocity.x, jumpHight);
        grounded = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            grounded = true;
        }
    }
}

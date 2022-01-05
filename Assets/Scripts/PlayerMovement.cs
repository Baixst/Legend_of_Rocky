using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public float runSpeed = 40f;

    public Animator animator;

    public float horizontalMove = 0f;
    private bool jump = false;
    private bool crouch = false;
    public bool allowMovement = true;

    // Update is called once per frame
    void Update()
    {
        if (allowMovement)
        {
            horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
            animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
        }
    }

    public void OnLanding()
    {
        animator.SetBool("IsJumping", false);
    }

    public void OnCrouching(bool isCrouching)
    {
        animator.SetBool("IsCrouching", isCrouching);
    }

    void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
        jump = false;
    }

    public void Crouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            crouch = true;
        }
        if (context.canceled)
        {
            crouch = false;
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jump = true;
            animator.SetBool("IsJumping", true);
        }
    }
}

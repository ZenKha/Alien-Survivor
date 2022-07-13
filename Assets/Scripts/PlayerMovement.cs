using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private CharacterController2D controller;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    public float runSpeed = 40f;

    float horizontalMove = 0;
    bool jump = false;


    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        if (Input.GetButton("Jump"))
        {
            jump = true;
            animator.SetBool("IsJumping", true);
        }

        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
    }

    public void OnLanding()
    {
        animator.SetBool("IsJumping", false);
    }
    void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, jump);
        jump = false;
    }
}

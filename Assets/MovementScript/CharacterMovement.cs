using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    // Start is called before the first frame update
    public CharacterController2D controller;
    public Animator animator;

    public float runSpeed = 20f;
    public float jumpModifier;
    public float sprintValue = 4f;
    public float horizontalMove = 0f;
    public float ySpeed;
    public float yOld = 0;
    public Transform tranform;
    public float downTime, upTime, pressTime;
    public bool jump = false;
    public bool sprint = false;
    public bool isReady = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        if (Input.GetButtonDown("Jump") && isReady == false)
        {
            downTime = Time.time;
        }

        // test of charge jump
        /* if (Input.GetButtonUp("Jump"))
         {
             upTime = Time.time;
             jumpModifier = (upTime - downTime)*4;
             jump = true;
             animator.SetBool("isJumping", true);
         }*/

        //jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump = true;
            animator.SetBool("isJumping", true);
        }
        
        //calculate ySpeed
        float yPos = tranform.position.y;
        ySpeed = yOld - yPos;
        animator.SetFloat("relativeYSpeed", ySpeed);
        yOld = yPos;

        //sprint and change of speed to show sprint in animation
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            sprint = !sprint;
        }
        if (sprint && this.animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
        {
            this.animator.speed= 1.5f;
        } else if (!sprint || !this.animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
        {
            this.animator.speed = 1f;
        }
        

   

    }

    public void onLanding()
    {
        animator.SetBool("isJumping", false);
        controller.countJump = 0;
    }
    private void FixedUpdate()
    {
        if (sprint)
        {
            controller.Move(horizontalMove * Time.fixedDeltaTime * sprintValue, false, jump);
        }
        else
        {
            controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump);
        }
        jump = false;
    }
}

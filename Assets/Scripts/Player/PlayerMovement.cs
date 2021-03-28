using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    CharacterController2D characterController;
    PlayerController playerController;
    Animator animator;
    Rigidbody2D rb;

	Vector3 velocity = Vector3.zero;
	bool isFacingRight = true;
    bool wasGrounded = false;
    float xInput = 0;

    // Timers
    Timer dashTimer, coyoteTimer;


    [Header("Ground")]
    [SerializeField] float speed = 10f;
    [SerializeField] float walkAcceleration = 2f;
    [SerializeField] float walkDeceleration = 2f;


    [Header("Air")]
    [SerializeField] float airAcceleration = 2f;
    [SerializeField] float jumpHeight = 5f;
    [SerializeField] float fallMultiplier = 2.5f;


    [Header("Dash")]
    [SerializeField] float dashSpeed;
    [SerializeField] float maxDashTime = .2f;
    bool dashReset = false;


    [Header("Better game")]
    [SerializeField] float maxCoyoteTime = .2f;


    [Header("Particle Effect")]
    [SerializeField] ParticleSystem footstepsPS;
    ParticleSystem.EmissionModule footstepsEmission;

    [SerializeField] GameObject jumpImpactPrefab;
    [SerializeField] GameObject dashEffectPrefab;


    bool isGrounded = false;
    bool isJumping = false;
    bool isDashing = false;

    #region getters

    public bool IsGrounded { get { return isGrounded; } }
    public bool IsJumping { get { return isJumping; } }
    public bool IsDashing { get { return isDashing; } }
    public bool DashHasReset { get { return dashReset; } }

    #endregion

    void Start()
    {
        characterController = this.GetComponent<CharacterController2D>();
        playerController = this.GetComponent<PlayerController>();
        animator = this.GetComponent<Animator>();
        rb = this.GetComponent<Rigidbody2D>();

        footstepsEmission = footstepsPS.emission;

        dashTimer = new Timer(maxDashTime);
        coyoteTimer = new Timer(maxCoyoteTime);
    }

	void Update()
	{
        isGrounded = characterController.isGrounded;

        // Update timers;
        coyoteTimer.Decrease();
        dashTimer.Decrease();

        if (isGrounded) 
        {
            velocity.y = 0;
            isJumping = false;
            animator.SetBool("isGrounded", true);

            if ( !wasGrounded ) {
                Land();
            } 
        } 
        else {
            animator.SetBool("isGrounded", false);
            if( wasGrounded && !isJumping ) {
                coyoteTimer.Start();
            }
        }

        float acceleration = isGrounded ? walkAcceleration : airAcceleration;
        float deceleration = isGrounded ? walkDeceleration : 0;

        if (xInput != 0) {
            velocity.x = Mathf.MoveTowards(velocity.x, speed * xInput, acceleration * Time.deltaTime);
        }
        else {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, deceleration * Time.deltaTime);
        }

		// if not dashing apply gravity before moving
        if(isDashing)
        {
            if(dashTimer.IsOn) {
                Dash();
            } else {
                StopDash();
            }
        } 
        else {
            ApplyGravity();
        }

        // move
		characterController.move(velocity * Time.deltaTime);

		// If the input is moving the player right and the player is facing left...
		if ( (xInput > 0 && !isFacingRight) || (xInput < 0 && isFacingRight) ) 
        {
			Flip();
		} 

        // update animator
        animator.SetFloat("xSpeed", Mathf.Abs(velocity.x));
        animator.SetFloat("ySpeed", -velocity.y);

        // show foosteps
        if(velocity.x != 0 && isGrounded) {
            footstepsEmission.rateOverTime = 35f;
        } else {
            footstepsEmission.rateOverTime = 0f;
        }

        // grab our isGrounded component
        wasGrounded = isGrounded;

		// grab our current _velocity to use as a base for all calculations
		velocity = characterController.velocity;
	}

    #region inputs

    public void MoveInput(InputAction.CallbackContext context)
    {
        xInput = context.ReadValue<Vector2>().x;
    }

    public void JumpInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            StartCoroutine( 
                playerController.InputBuffer(() => playerController.CanJump(), Jump) 
            );
        }
        else if (context.canceled && velocity.y > 0) {
            velocity.y *= 0.5f;
            characterController.move( velocity * Time.deltaTime );
        }
    }

    public void DashInput(InputAction.CallbackContext context)
    {
        if(context.performed) 
        {
            StartCoroutine( 
                playerController.InputBuffer(() => playerController.CanDash(), StartDash) 
            );
        }
    }

    #endregion

    void Jump()
	{
        velocity.y = Mathf.Sqrt( 2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y) );
        isJumping = true;
        characterController.move(velocity * Time.deltaTime);
	}

    void StartDash()
	{
        isDashing = true;
        animator.SetBool("isDashing", true);
        dashReset = false;
        dashTimer.Start();
        Instantiate(dashEffectPrefab, transform.position, Quaternion.identity);
	}

    void Dash()
	{
        velocity.y = 0;
        if(isFacingRight) {
            velocity.x = dashSpeed;
        }  else {
            velocity.x = -dashSpeed;
        }
	}

    void StopDash()
	{
        isDashing = false;
        animator.SetBool("isDashing", false);
        velocity = Vector3.zero; 
	}



    void ApplyGravity()
	{
        if( velocity.y < 0) {
		    velocity.y += Physics2D.gravity.y * fallMultiplier * Time.deltaTime;
        } else {
		    velocity.y += Physics2D.gravity.y * Time.deltaTime;
        }
	}

    void Flip()
    {
        // Switch the way the player is labelled as facing.
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    void Land()
	{
        //show jump impact
        Instantiate(jumpImpactPrefab, footstepsPS.transform.position, Quaternion.identity);
        coyoteTimer.Stop();
        dashReset = true;
	}

}

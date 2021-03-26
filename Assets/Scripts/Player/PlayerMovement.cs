using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    CharacterController2D characterController;
    PlayerAttack playerAttack;
    Animator animator;
    Rigidbody2D rb;

	Vector3 velocity = Vector3.zero;
	bool isFacingRight = true;
    bool wasGrounded = false;
    bool isGrounded = false;
    bool isJumping = false;
    float xInput = 0;

    // Timers
    Timer jumpBufferTimer, dashTimer, coyoteTimer;


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
    bool isDashing = false;
    bool canDash = false;


    [Header("Better game")]
    [SerializeField] float maxCoyoteTime = .2f;
    [SerializeField] float maxJumpBufferTime = .2f;


    [Header("Particle Effect")]
    [SerializeField] ParticleSystem footstepsPS;
    ParticleSystem.EmissionModule footstepsEmission;

    [SerializeField] GameObject jumpImpactPrefab;
    [SerializeField] GameObject dashEffectPrefab;


    void Start()
    {
        characterController = this.GetComponent<CharacterController2D>();
        animator = this.GetComponent<Animator>();
        rb = this.GetComponent<Rigidbody2D>();
        playerAttack = this.GetComponent<PlayerAttack>();

        footstepsEmission = footstepsPS.emission;

        jumpBufferTimer = new Timer(maxJumpBufferTime);
        dashTimer = new Timer(maxDashTime);
        coyoteTimer = new Timer(maxCoyoteTime);
    }

	void Update()
	{
        isGrounded = characterController.isGrounded;

        // Update timers;
        coyoteTimer.Decrease(Time.deltaTime);
        jumpBufferTimer.Decrease(Time.deltaTime);
        dashTimer.Decrease(Time.deltaTime);

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
                ApplyDash();
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

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if ( !isJumping ) {
                if ( characterController.isGrounded || coyoteTimer.IsOn ) {
                    ApplyJump();
                    characterController.move( velocity * Time.deltaTime );
                }
            }
            else {
                jumpBufferTimer.Start();
            }
        }
        else if (context.canceled && velocity.y > 0) {
            velocity.y *= 0.5f;
            characterController.move( velocity * Time.deltaTime );
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        xInput = context.ReadValue<Vector2>().x;
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if(context.performed && canDash) 
        {
            isDashing = true;
            canDash = false;
            animator.SetBool("isDashing", isDashing);
            dashTimer.Start();
            Instantiate(dashEffectPrefab, transform.position, Quaternion.identity);
        }
    }


    void ApplyJump()
	{
        velocity.y = Mathf.Sqrt( 2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y) );
        isJumping = true;
	}

    void ApplyGravity()
	{
        if( velocity.y < 0) {
		    velocity.y += Physics2D.gravity.y * fallMultiplier * Time.deltaTime;
        } else {
		    velocity.y += Physics2D.gravity.y * Time.deltaTime;
        }
	}

    void ApplyDash()
	{
        velocity.y = 0;
        if(isFacingRight) {
            velocity.x = dashSpeed;
        }  else {
            velocity.x = -dashSpeed;
        }
	}



    void Flip()
    {
        // Switch the way the player is labelled as facing.
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    void StopDash()
	{
        isDashing = false;
        animator.SetBool("isDashing", isDashing);
        velocity = Vector3.zero; 
	}

    void Land()
	{
        //show jump impact
        Instantiate(jumpImpactPrefab, footstepsPS.transform.position, Quaternion.identity);
        coyoteTimer.Reset();
        canDash = true;
        if (jumpBufferTimer.IsOn) {
            ApplyJump();
        }
	}

}

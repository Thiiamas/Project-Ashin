using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    CharacterController2D characterController;
    PlayerController playerController;
    SpriteRenderer spriteRenderer;
    Animator animator;
    Rigidbody2D rb;

	Vector3 velocity = Vector3.zero;
	bool isFacingRight = true;
    bool wasGrounded = false;
    float xInput = 0;
    bool collidingWithWall = false;

    // Timers
    Timer dashTimer;
    Timer coyoteTimer;
    Timer dashCooldownTimer;


    [Header("Ground")]
    [SerializeField] float speed = 10f;
    [SerializeField] float walkAcceleration = 2f;
    [SerializeField] float walkDeceleration = 2f;


    [Header("Air")]
    [SerializeField] float airAcceleration = 2f;


    [Header("Jump")]
    [SerializeField] float jumpHeight = 5f;
    [SerializeField] float fallMultiplier = 2.5f;
    [SerializeField] float coyoteTime = .2f;
    [SerializeField] float wallJumpForce;


    [Header("Wall")]
    [SerializeField] Transform wallCheck;
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] float wallSlideSpeed;


    [Header("Dash")]
    [SerializeField] float dashSpeed;
    [SerializeField] float dashTime = .2f;
    [SerializeField] float dashCooldownTime = .2f;
    bool dashHasReset = false;
    bool dashHasCooldown = true;


    [Header("Particle Effect")]
    [SerializeField] ParticleSystem footstepsPS;
    [SerializeField] GameObject jumpImpactPrefab;
    [SerializeField] GameObject dashEffectPrefab;
    ParticleSystem.EmissionModule footstepsEmission;


    bool isGrounded = false;
    bool isWallSliding = false;
    bool isJumping = false;
    bool isDashing = false;

    #region getters

    public bool IsGrounded { get { return isGrounded; } }
    public bool IsJumping { get { return isJumping; } }
    public bool IsDashing { get { return isDashing; } }
    public bool DashHasReset { get { return dashHasReset; } }
    public bool DashHasCooldown { get { return dashHasCooldown; } }
    public bool IsWallSliding { get { return isWallSliding; } }
    public bool IsCoyoteTimerOn { get { return coyoteTimer.IsOn; } }

    #endregion

    void Start()
    {
        characterController = this.GetComponent<CharacterController2D>();
        playerController = this.GetComponent<PlayerController>();
        animator = this.GetComponent<Animator>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        rb = this.GetComponent<Rigidbody2D>();

        footstepsEmission = footstepsPS.emission;

        dashTimer = new Timer(dashTime);
        coyoteTimer = new Timer(coyoteTime);
        dashCooldownTimer = new Timer(dashCooldownTime);
    }

	void Update()
	{
        isGrounded = characterController.isGrounded;

        if (isGrounded) {
            velocity.y = 0;
            isJumping = false;
            if ( !wasGrounded ) {
                //show jump impact
                Instantiate(jumpImpactPrefab, footstepsPS.transform.position, Quaternion.identity);
                dashHasReset = true;
            } 
        } 
        else if( wasGrounded && !isJumping ) {
            StartCoroutine(CoyoteTime());
        }
        animator.SetBool("isGrounded", isGrounded);


        // Wall Slide
        collidingWithWall = Physics2D.OverlapCircle(wallCheck.position, .2f, whatIsGround);
        if( collidingWithWall && !isGrounded ) {
            dashHasReset = isDashing ? false : true;
            isWallSliding = true;
        } else {
            isWallSliding = false;
        }
        animator.SetBool("isWallSliding", isWallSliding);


        float acceleration = isGrounded ? walkAcceleration : airAcceleration;
        float deceleration = isGrounded ? walkDeceleration : 0;

        if (xInput != 0) {
            velocity.x = Mathf.MoveTowards(velocity.x, speed * xInput, acceleration * Time.deltaTime);
        } else {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, deceleration * Time.deltaTime);
        }  

		// if not dashing apply gravity before moving
        if(!isDashing) {
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
        if(isWallSliding) {
            velocity.x = isFacingRight ? -wallJumpForce : wallJumpForce;
            Flip();
        } 
        velocity.y = Mathf.Sqrt( 2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y) );
        isJumping = true;
        characterController.move(velocity * Time.deltaTime);
	}

    void StartDash()
	{
        Instantiate(dashEffectPrefab, transform.position, Quaternion.identity);
        isDashing = true;
        animator.SetBool("isDashing", true);
        dashHasReset = false;
        float dSpeed = isFacingRight ? dashSpeed : -dashSpeed;
        if(isWallSliding){
            dSpeed = -dSpeed;
            Flip();
        }
        StartCoroutine( Dash(dSpeed) );
	}

    public IEnumerator Dash(float dSpeed) 
    {
        dashTimer.Start();
        while ( dashTimer.IsOn ) 
        {
            velocity.y = 0;
            velocity.x = dSpeed; 
            dashTimer.Decrease();            
            yield return new WaitForEndOfFrame();
        }
        StopDash();
    }

    void StopDash()
	{
        isDashing = false;
        animator.SetBool("isDashing", false);
        velocity = Vector3.zero; 
        dashHasCooldown = false;
        StartCoroutine(CooldDownDash());
	}

    public IEnumerator CooldDownDash()
	{
        dashCooldownTimer.Start();
        while ( dashCooldownTimer.IsOn ) 
        {
            //spriteRenderer.color = Color.h;
            dashCooldownTimer.Decrease();            
            yield return new WaitForEndOfFrame();
        }
        dashHasCooldown = true;
	}



    public IEnumerator CoyoteTime () 
    {
        coyoteTimer.Start();
        while ( coyoteTimer.IsOn ) 
        {
            coyoteTimer.Decrease();            
            yield return new WaitForEndOfFrame();
        }
    }


    void ApplyGravity()
	{
        if(isWallSliding && velocity.y < -wallSlideSpeed) {
		    velocity.y = -wallSlideSpeed;
        } 
        else if( velocity.y < 0) {
		    velocity.y += Physics2D.gravity.y * fallMultiplier * Time.deltaTime;
        } 
        else {
		    velocity.y += Physics2D.gravity.y * Time.deltaTime;
        }
	}

    void Flip()
    {
        // Switch the way the player is labelled as facing.
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }

}

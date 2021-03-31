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
    float xInput = 0;
	bool isFacingRight = true;
    bool isGrounded = false;
    bool wasGrounded = false;
    bool isWallSliding = false;
    bool isJumping = false;
    bool isDashing = false;


    // Timers
    Timer dashTimer, coyoteTimer;


    [Header("Speed")]
    [SerializeField] float speed = 10f;
    [SerializeField] float walkAcceleration = 200f;
    [SerializeField] float walkDeceleration = 200f;
    [SerializeField] float airAcceleration = 20f;
    [SerializeField] Vector2 knockBack = new Vector2(5, 5);

    
    [Header("Gravity")]
    [SerializeField] float fallMultiplier = 2.5f;


    [Header("Jump")]
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float xWallJumpForce = 6f;
    [SerializeField] float jumpFloatFeel = 0.5f;
    [SerializeField] float coyoteTime = .2f;


    [Header("Wall")]
    [SerializeField] Transform wallCheck;
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] float wallSlideSpeed;
    bool isCollidingWithWall = false;


    [Header("Dash")]
    [SerializeField] float dashSpeed;
    [SerializeField] float dashTime = .2f;
    [SerializeField] float dashCooldown = 2f;
    bool dashHasReset = false;
    bool dashHasCooldown = true;


    [Header("Color")]
    [SerializeField] Color basicColor;
    [SerializeField] Color dashCooldownColor;


    [Header("Particle Effect")]
    [SerializeField] ParticleSystem footstepsPS;
    [SerializeField] GameObject jumpImpactPrefab;
    [SerializeField] GameObject dashEffectPrefab;
    ParticleSystem.EmissionModule footstepsEmission;


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
        animator = playerController.GFX.GetComponent<Animator>();
        spriteRenderer = playerController.GFX.GetComponent<SpriteRenderer>();
        rb = this.GetComponent<Rigidbody2D>();

        footstepsEmission = footstepsPS.emission;

        dashTimer = new Timer(dashTime);
        coyoteTimer = new Timer(coyoteTime);
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
        isCollidingWithWall = Physics2D.OverlapCircle(wallCheck.position, .2f, whatIsGround);
        if( isCollidingWithWall && !isGrounded ) {
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

    public void KnockBack(Transform damageDealer) 
    {
        Vector3 direction = (transform.position - damageDealer.position).normalized;
        velocity.x = direction.x * knockBack.x;
        velocity.y = knockBack.y;
        characterController.move(velocity * Time.deltaTime);
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
            velocity.y *= jumpFloatFeel;
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


    #region Jump

    void Jump()
	{       
        if(isWallSliding) {
            velocity.x = isFacingRight ? -xWallJumpForce : xWallJumpForce;
            Flip();
        } 
        velocity.y = Mathf.Sqrt( 2 * jumpForce * Mathf.Abs(Physics2D.gravity.y) );
        isJumping = true;
        characterController.move(velocity * Time.deltaTime);
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

    #endregion


    #region Dash

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
        spriteRenderer.color = dashCooldownColor;
        yield return new WaitForSeconds(dashCooldown);
        spriteRenderer.color = basicColor;
        dashHasCooldown = true;
	}

    #endregion

}

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    CharacterController2D characterController;
    PlayerController playerController;

	Vector3 velocity = Vector3.zero;
    Vector2 directionInput = Vector2.zero;
	bool isFacingRight = true;
    bool isGrounded = false;
    bool wasGrounded = false;
    bool isWallSliding = false;
    bool isJumping = false;
    bool isDashing = false;


    // Timers
    Timer dashTimer, coyoteTimer;


    [Header("Speed")]
    [SerializeField] float speed;
    [SerializeField] float walkAcceleration;
    [SerializeField] float walkDeceleration;
    [SerializeField] float airAcceleration;

    
    [Header("Gravity")]
    [SerializeField] float fallMultiplier;


    [Header("Jump")]
    [SerializeField] float jumpForce;
    [SerializeField] [Range(0,1)] float jumpFloatFeel;
    [SerializeField] float maxCoyoteTime;


    [Header("Wall Jump")]
    [SerializeField] Transform wallCheck;
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] float wallJumpForce;
    [SerializeField] float wallSlideSpeed;
    bool isCollidingWithWall = false;


    [Header("Dash")]
    [SerializeField] float dashSpeed;
    [SerializeField] float maxDashTime = .2f;
    [SerializeField] float dashCooldown = 2f;
    bool dashHasReset = false;
    bool dashHasCooldown = true;

    
    [Header("KnockBack")]
    [SerializeField] Vector2 knockBackForce = new Vector2(5, 5);
    [SerializeField] float knockBackDeceleration = 2f;
    bool isKnockbacked = false;


    [Header("Particle Effect")]
    [SerializeField] ParticleSystem footstepsPS;
    [SerializeField] ParticleSystem dashPS;
    [SerializeField] GameObject jumpImpactPrefab;
    ParticleSystem.EmissionModule footstepsEmission;

    BoxCollider2D box;
    #region getters

    public bool IsGrounded { get { return isGrounded; } }
    public bool IsFacingRight { get { return isFacingRight; } }
    public bool IsJumping { get { return isJumping; } }
    public bool IsDashing { get { return isDashing; } }
    public bool DashHasReset { get { return dashHasReset; } }
    public bool DashHasCooldown { get { return dashHasCooldown; } }
    public bool IsWallSliding { get { return isWallSliding; } }
    public bool IsCoyoteTimerOn { get { return coyoteTimer.IsOn; } }
    public Vector3 Velocity { get { return velocity; } }
    public Vector2 DirectionInput { get { return directionInput; } }

    #endregion

    void Start()
    {
        characterController = this.GetComponent<CharacterController2D>();
        playerController = this.GetComponent<PlayerController>();

        footstepsEmission = footstepsPS.emission;

        dashTimer = new Timer(maxDashTime);
        coyoteTimer = new Timer(maxCoyoteTime);
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


        // Wall Slide
        isCollidingWithWall = Physics2D.OverlapCircle(wallCheck.position, .2f, whatIsGround);
        if( isCollidingWithWall && !isGrounded ) {
            dashHasReset = isDashing ? false : true;
            isWallSliding = true;
        } else {
            isWallSliding = false;
        }

        if( !isKnockbacked && !isDashing ) 
        {
            float acceleration = isGrounded ? walkAcceleration : airAcceleration;
            float deceleration = isGrounded ? walkDeceleration : 0;

            if (directionInput.x != 0) {
                velocity.x = Mathf.MoveTowards(velocity.x, speed * directionInput.x, acceleration * Time.deltaTime);
            } else {
                velocity.x = Mathf.MoveTowards(velocity.x, 0, deceleration * Time.deltaTime);
            }  
        }

		// if not dashing apply gravity before moving
        if(!isDashing) {
            ApplyGravity();
        }

        // move
		characterController.move(velocity * Time.deltaTime);

		if ( (directionInput.x > 0 && !isFacingRight) || (directionInput.x < 0 && isFacingRight) ) 
        {
            Flip();
		} 

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
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }


    #region inputs

    public void MoveInput(InputAction.CallbackContext context)
    {
        directionInput = context.ReadValue<Vector2>();
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
        playerController.CanJumpAfterAttack = false;
        if(isWallSliding) {
            float wallJumpf = Mathf.Sqrt( 2 * wallJumpForce * Mathf.Abs(Physics2D.gravity.y) );
            velocity.x = isFacingRight ? -wallJumpf : wallJumpf;
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
        Utilities.StartParticleSystem(dashPS, maxDashTime);

        isDashing = true;
        dashHasReset = false;
        
        Vector2 dSpeed = Vector2.zero;
        if(directionInput != Vector2.zero){
            dSpeed = directionInput * dashSpeed;
        } else {
            dSpeed.x = isFacingRight ? dashSpeed : -dashSpeed;
        }

        if (isWallSliding)
        {
            dSpeed = -dSpeed;
            Flip();
        }
        StartCoroutine(Dash(dSpeed));
    }

    public IEnumerator Dash(Vector2 dSpeed)
    {
        dashTimer.Start();
        while (dashTimer.IsOn)
        {
            velocity = dSpeed;
            dashTimer.Decrease();
            yield return new WaitForEndOfFrame();
        }
        velocity = Vector3.zero; 
        dashHasCooldown = false;
        isDashing = false;

        // Cooldown
        yield return new WaitForSeconds(dashCooldown);
        dashHasCooldown = true;
    }

    #endregion



    #region knockBack

	public IEnumerator KnockBack(Vector3 direction, Vector2 force)
	{
		isKnockbacked = true;
		velocity = direction * force;
		characterController.move(velocity * Time.deltaTime);
		while (velocity.x > .1f || velocity.x < -.1f)
		{
			velocity.x = Mathf.MoveTowards(velocity.x, 0, knockBackDeceleration * Time.deltaTime);
			characterController.move(velocity * Time.deltaTime);
			yield return new WaitForEndOfFrame();
		}
        isKnockbacked = false;
	}

	public IEnumerator KnockBack(Vector3 direction)
	{
		isKnockbacked = true;
		velocity = direction * knockBackForce;
		characterController.move(velocity * Time.deltaTime);
		while (velocity.x > .1f || velocity.x < -.1f)
		{
			velocity.x = Mathf.MoveTowards(velocity.x, 0, knockBackDeceleration * Time.deltaTime);
			characterController.move(velocity * Time.deltaTime);
			yield return new WaitForEndOfFrame();
		}
        isKnockbacked = false;
	}

    #endregion
    
}

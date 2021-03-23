using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController2D characterController;
    private Animator animator;
	private Vector3 velocity = Vector3.zero;
	private bool isFacingRight = true;
    private bool wasGrounded = false;
    private bool isJumping = false;

    private float xInput = 0;

    [SerializeField] float speed = 10f;
    [SerializeField] float walkAcceleration = 2f;
    [SerializeField] float walkDeceleration = 2f;
    [SerializeField] float airAcceleration = 2f;
    [SerializeField] float jumpHeight = 5f;
    [SerializeField] float fallMultiplier = 2.5f;

    [SerializeField] float maxCoyoteTime = .2f;
    private float coyoteTimer = 0f;

    [SerializeField] float maxJumpBufferTime = .2f;
    private float jumpBufferTimer = 0f;

    [SerializeField] ParticleSystem footstepsPS;
    private ParticleSystem.EmissionModule footstepsEmission;

    [SerializeField] GameObject jumpImpactPrefab;
    InputAction.CallbackContext jumpContext;

    void Start(){
        characterController = this.GetComponent<CharacterController2D>();
        animator = this.GetComponent<Animator>();
        footstepsEmission = footstepsPS.emission;

        jumpBufferTimer = maxJumpBufferTime;
    }

	void Update()
	{

        if (characterController.isGrounded) {
            velocity.y = 0;
            coyoteTimer = 0;
            isJumping = false;
            animator.SetBool("isGrounded", true);

            if (jumpBufferTimer < maxJumpBufferTime)
            {
                velocity.y = Mathf.Sqrt( 2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y) );
                jumpBufferTimer = maxJumpBufferTime;
            }

        } 
        else {
            animator.SetBool("isGrounded", false);
        }

        coyoteTimer += Time.deltaTime;
        jumpBufferTimer += Time.deltaTime;

        float acceleration = characterController.isGrounded ? walkAcceleration : airAcceleration;
        float deceleration = characterController.isGrounded ? walkDeceleration : 0;

        if (xInput != 0) {
            velocity.x = Mathf.MoveTowards(velocity.x, speed * xInput, acceleration * Time.deltaTime);
        }
        else {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, deceleration * Time.deltaTime);
        }

		// apply gravity before moving
        if( velocity.y < 0) {
		    velocity.y += Physics2D.gravity.y * fallMultiplier * Time.deltaTime;
        } else {
		    velocity.y += Physics2D.gravity.y * Time.deltaTime;
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
        if(velocity.x != 0 && characterController.isGrounded) {
            footstepsEmission.rateOverTime = 35f;
        } else {
            footstepsEmission.rateOverTime = 0f;
        }

        //show jump impact
        if(characterController.isGrounded && !wasGrounded) {
            Instantiate(jumpImpactPrefab, footstepsPS.transform.position, Quaternion.identity);
        }
        wasGrounded = characterController.isGrounded;

		// grab our current _velocity to use as a base for all calculations
		velocity = characterController.velocity;
	}

    public void Jump(InputAction.CallbackContext context)
    {
        // TODO: add jump buffer to make it more player friendly

        if (context.performed)
        {
            if ( !isJumping && coyoteTimer < maxCoyoteTime ) {
                velocity.y = Mathf.Sqrt( 2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y) );
            }
            else {
                jumpBufferTimer = 0;
            }
        }
        else if (context.canceled && velocity.y > 0)
        {
            velocity.y *= 0.5f;
            isJumping = true;
        }

        characterController.move( velocity * Time.deltaTime );
    }

    public void Move(InputAction.CallbackContext context)
    {
        xInput = context.ReadValue<Vector2>().x;
    }


    void Flip()
	{
		// Switch the way the player is labelled as facing.
		isFacingRight = !isFacingRight;
		// Multiply the player's x local scale by -1.
        transform.Rotate(0f, 180f, 0f);
	}

}

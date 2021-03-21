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
    private float xInput = 0;
    private bool isJumping = false;

    [SerializeField] float speed = 10f;
    [SerializeField] float walkAcceleration = 2f;
    [SerializeField] float walkDeceleration = 2f;
    [SerializeField] float airAcceleration = 2f;
    [SerializeField] float jumpHeight = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float fallMultiplier = 2.5f;

    [SerializeField] float hangTime = .2f;
    private float hangCounter = 0f;

    [SerializeField] ParticleSystem footstepsPS;
    private ParticleSystem.EmissionModule footEmission;

    [SerializeField] ParticleSystem jumpImpactPS;
    private bool wasGrounded = false;


    void Start(){
        characterController = this.GetComponent<CharacterController2D>();
        animator = this.GetComponent<Animator>();
        footEmission = footstepsPS.emission;
    }

	void Update()
	{
        if (characterController.isGrounded) {
            velocity.y = 0;
            hangCounter = hangTime;
            isJumping = false;
            animator.SetBool("isGrounded", true);
        } 
        else {
            hangCounter -= Time.deltaTime;
            animator.SetBool("isGrounded", false);
        }


        float acceleration = characterController.isGrounded ? walkAcceleration : airAcceleration;
        float deceleration = characterController.isGrounded ? walkDeceleration : 0;

        if (xInput != 0) {
            velocity.x = Mathf.MoveTowards(velocity.x, speed * xInput, acceleration * Time.deltaTime);
        }
        else {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, deceleration * Time.deltaTime);
        }

		// apply gravity before moving
		velocity.y += Physics2D.gravity.y * fallMultiplier * Time.deltaTime;

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
        if(velocity.x != 0 && characterController.isGrounded){
            footEmission.rateOverTime = 35f;
        } else {
            footEmission.rateOverTime = 0f;
        }

        //show jump impact
        if(characterController.isGrounded && !wasGrounded){
            jumpImpactPS.gameObject.SetActive(true);
            jumpImpactPS.Stop();
            jumpImpactPS.Play();
        }
        wasGrounded = characterController.isGrounded;

		// grab our current _velocity to use as a base for all calculations
		velocity = characterController.velocity;
	}

    public void Jump(InputAction.CallbackContext context)
    {
        // TODO: add jump buffer to make it more player friendly

        if(isJumping){
            return;
        }

        // we check the context to know if we are pressing or releasing the button
        if(context.performed)
        {
            // coyote time to make a better plateformer
            if(characterController.isGrounded || hangCounter > 0f){
                // Calculate the velocity required to achieve the target jump height.
                velocity.y = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y));
                characterController.move( velocity * Time.deltaTime );
            }
        }
        else if (context.canceled) 
        {
            // reduce velocity to jump lower depending on when you release the jump button
            velocity.y *= 0.5f;
            isJumping = true;
        } 
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
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

}

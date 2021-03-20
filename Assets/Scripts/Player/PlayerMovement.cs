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

    [SerializeField] float speed = 10;
    [SerializeField] float walkAcceleration = 2;
    [SerializeField] float walkDeceleration = 2;
    [SerializeField] float airAcceleration = 2;
    [SerializeField] float jumpHeight = 5;

    void Awake(){
        characterController = this.GetComponent<CharacterController2D>();
        animator = this.GetComponent<Animator>();
    }

	void Update()
	{
        if (characterController.isGrounded) {
            velocity.y = 0;
            animator.SetBool("isGrounded", true);
        } 
        else {
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
		velocity.y += Physics2D.gravity.y * Time.deltaTime;

        // move
		characterController.move( velocity * Time.deltaTime );

		// If the input is moving the player right and the player is facing left...
		if ( (xInput > 0 && !isFacingRight) || (xInput < 0 && isFacingRight) ) 
        {
			Flip();
		} 

        // update animator
        animator.SetFloat("xSpeed", Mathf.Abs(velocity.x));
        animator.SetFloat("ySpeed", -velocity.y);

		// grab our current _velocity to use as a base for all calculations
		velocity = characterController.velocity;
	}

    public void Jump()
    {
        if (characterController.isGrounded) 
        {
            // Calculate the velocity required to achieve the target jump height.
            velocity.y = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y));
            characterController.move( velocity * Time.deltaTime );
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController2D controller;
    private Animator animator;
	private Vector3 velocity = Vector3.zero;
	private bool isFacingRight = true;
    private float horizontalInput = 0;

    [SerializeField] float speed = 10;
    [SerializeField] float walkAcceleration = 2;
    [SerializeField] float walkDeceleration = 2;
    [SerializeField] float airAcceleration = 2;
    [SerializeField] float jumpHeight = 5;

    void Awake(){
        controller = this.GetComponent<CharacterController2D>();
        animator = this.GetComponent<Animator>();
    }

	void Update()
	{

        float horizontalInput = Input.GetAxisRaw("Horizontal");

        if (controller.isGrounded)
        {
            velocity.y = 0;
            animator.SetBool("isGrounded", true);

            if (Input.GetButtonDown("Jump"))
            {
                // Calculate the velocity required to achieve the target jump height.
                velocity.y = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y));
            }
        } 
        else 
        {
            velocity.y += Physics2D.gravity.y * Time.deltaTime;
            animator.SetBool("isGrounded", false);
        }


        float acceleration = controller.isGrounded ? walkAcceleration : airAcceleration;
        float deceleration = controller.isGrounded ? walkDeceleration : 0;

        if (horizontalInput != 0)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, speed * horizontalInput, acceleration * Time.deltaTime);
        }
        else
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, deceleration * Time.deltaTime);
        }

		// apply gravity before moving
		velocity.y += Physics2D.gravity.y * Time.deltaTime;

        // move
		controller.move( velocity * Time.deltaTime );
		// If the input is moving the player right and the player is facing left...
		if ( (horizontalInput > 0 && !isFacingRight) || (horizontalInput < 0 && isFacingRight) ) 
        {
			Flip();
		} 
        animator.SetFloat("xSpeed", Mathf.Abs(velocity.x));
        animator.SetFloat("ySpeed", -velocity.y);

		// grab our current _velocity to use as a base for all calculations
		velocity = controller.velocity;
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

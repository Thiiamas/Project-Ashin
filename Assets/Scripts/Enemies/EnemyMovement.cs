using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyMovement : MonoBehaviour
{
    private EnemyController2D enemyController;
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

    public float knockBackCount;
    [SerializeField] float knockBack = 4f;
    public bool toRight = false;
    public Rigidbody2D rbSource;
    public Rigidbody2D rbPlayer;

    void Start(){
        enemyController = this.GetComponent<EnemyController2D>();
        rbPlayer = this.GetComponent<Rigidbody2D>();
        footstepsEmission = footstepsPS.emission;

        jumpBufferTimer = maxJumpBufferTime;
    }

	void Update()
	{
        if (enemyController.isGrounded) {
            velocity.y = 0;
            coyoteTimer = 0;
            isJumping = false;

            if (jumpBufferTimer < maxJumpBufferTime)
            {
                velocity.y = Mathf.Sqrt( 2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y) );
                jumpBufferTimer = maxJumpBufferTime;
            }

        } 
        else {
        }

        coyoteTimer += Time.deltaTime;
        jumpBufferTimer += Time.deltaTime;

        float acceleration = enemyController.isGrounded ? walkAcceleration : airAcceleration;
        float deceleration = enemyController.isGrounded ? walkDeceleration : 0;

/*        if (xInput != 0)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, speed * xInput, acceleration * Time.deltaTime);
        }
        else
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, deceleration * Time.deltaTime);
        }*/

        // apply gravity before moving
        if ( velocity.y < 0) {
		    velocity.y += Physics2D.gravity.y * fallMultiplier * Time.deltaTime;
        } else {
		    velocity.y += Physics2D.gravity.y * Time.deltaTime;
        }

        if (knockBackCount <= 0)
        {
            //enemyController.move(velocity * Time.deltaTime);
        }
        else
        {
            knockback();
        }
        

		// If the input is moving the player right and the player is facing left...
		if ( (xInput > 0 && !isFacingRight) || (xInput < 0 && isFacingRight) ) 
        {
			Flip();
		}

		// grab our current _velocity to use as a base for all calculations
/*		velocity = enemyController.velocity;
*/	}

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

        enemyController.move( velocity * Time.deltaTime );
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

    void knockback()
    {
        Debug.Log("knockback");
        Vector2 v2Right = new Vector2(knockBack, 0.5f * knockBack) * Time.deltaTime;
        Vector3 v3Right = new Vector3(v2Right.x, v2Right.y, 0f);
        Vector2 v2Left = new Vector2(-knockBack, 0.5f * knockBack) * Time.deltaTime;
        Vector3 v3Left = new Vector3(v2Left.x, v2Left.y, 0f);
        if (toRight)
        {
            enemyController.move(v3Left);
            rbSource.AddForce(v2Right*2/ Time.deltaTime);
            /*rbSource.velocity = new Vector2(knockBack, 0.5f * knockBack);
            rbPlayer.velocity = new Vector2(-knockBack, 0.5f * knockBack);*/
        }
        else
        {
            enemyController.move(v3Right);
            rbSource.AddForce(v2Left*2/ Time.deltaTime);
            /*rbSource.velocity = new Vector2(-knockBack, 0.5f * knockBack);
            rbPlayer.velocity = new Vector2(knockBack, 0.5f * knockBack);*/
        }
        knockBackCount -= Time.deltaTime;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roumsor : Enemy
{
    EnemyController2D enemyController;
    Vector3 velocity = Vector3.zero;

    [SerializeField] float speed = 10f;
    [SerializeField] float walkAcceleration = 2f;
    [SerializeField] float walkDeceleration = 2f;
    [SerializeField] float airAcceleration = 2f;
    [SerializeField] float jumpHeight = 5f;
    [SerializeField] float fallMultiplier = 2.5f;

    [SerializeField] float maxJumpBufferTime = .2f;

    private bool isJumping = false;



    [SerializeField] PolygonCollider2D attackCollider;
    [SerializeField] LayerMask playerLayer;


    bool isAgro = false;
    public float agroRange;
    Vector3 directionToPlayer;

    public float detectionRange;
    public LayerMask attackMask;

    public Transform attack1Transform;
    public float attack1Range;
    float attack1Damage = 10.0f;
    public float attackSpeed;
    float lastAttack = 0.0f;

    public float dashRange;
    public float dashSpeed;
    public float dashCooldown = 2.0f;
    public float lastDash;
    float dashTime = 2.0f;
    bool isDashing = false;

    // Start is called before the first frame update

    protected override void Setup()
    {
        base.Setup();
        enemyController = GetComponent<EnemyController2D>();
        //directionToPlayer = (playerTransform.position - transform.position).normalized;
    }

    public void Update()
    {
        if (enemyController.isGrounded)
        {
            velocity.y = 0;
            isJumping = false;
            animator.SetBool("isGrounded", true);
        }
        else
        {
            animator.SetBool("isGrounded", false);
        }


        float acceleration = enemyController.isGrounded ? walkAcceleration : airAcceleration;

        // apply gravity before moving
        if (velocity.y < 0)
        {
            velocity.y += Physics2D.gravity.y * fallMultiplier * Time.deltaTime;
        }
        else
        {
            velocity.y += Physics2D.gravity.y * Time.deltaTime;
        }

        //Aggro check
        float distance = Vector2.Distance(playerTransform.position, transform.position);
        if (distance <= agroRange)
        {
            isAgro = true;
        } else
        {
            isAgro = false;
            // maybe come back to spawn point
        }

        directionToPlayer = (playerTransform.position - transform.position).normalized;
        if (isAgro && distance > attack1Range)
        {
            if (enemyController.isGrounded && !isDashing)
            {
                if (directionToPlayer.x > 0)
                {
                    velocity.x = Mathf.MoveTowards(velocity.x, speed, acceleration * Time.deltaTime);

                } else
                {
                    velocity.x = Mathf.MoveTowards(velocity.x, - speed, acceleration * Time.deltaTime);

                }                
            }
            enemyController.move(velocity * Time.deltaTime);
            // update animator
            animator.SetFloat("xSpeed", Mathf.Abs(velocity.x));
            animator.SetFloat("ySpeed", -velocity.y);
            // grab our current _velocity to use as a base for all calculations
            velocity = enemyController.velocity;

            if ((velocity.x > 0 && !isFacingRight) || (velocity.x < 0 && isFacingRight))
            {
                Flip();
            }
        }
        else if (distance <= attack1Range && Time.time >= lastAttack + 1 / attackSpeed)
        {
            /*rb.velocity = Vector2.zero;*/
            animator.SetFloat("xSpeed", 0);
            animator.SetTrigger("attack1");
            lastAttack = Time.time;
        }
    }

    public void attack1WithCollider()
    {
        List<Collider2D> hitten = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(playerLayer);
        Physics2D.OverlapCollider(attackCollider, filter, hitten);

        foreach (Collider2D hit in hitten)
        {
            if (hit.gameObject.tag == "Player")
            {
                hit.GetComponent<PlayerController>().TakeDamage(this.transform, attack1Damage);
            }
        }

    }
}

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
    private float jumpBufferTimer = 0f;

    private bool wasGrounded = false;
    private bool isJumping = false;



    Transform GFXTransform;
    [SerializeField] PolygonCollider2D attackCollider;
    [SerializeField] LayerMask playerLayer;


    bool isAgro = false;
    public float agroRange;
    Vector3 directionToPlayer;

    public float detectionRange;
    public LayerMask attackMask;

    public Vector2 attack1Offset;
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
        GFXTransform = GetComponentInChildren<Transform>();
        enemyController = GetComponent<EnemyController2D>();
        directionToPlayer = (playerTransform.position - transform.position).normalized;
    }

    public void Update()
    {
        if (enemyController.isGrounded)
        {
            velocity.y = 0;
            isJumping = false;
            animator.SetBool("isGrounded", true);

            if (jumpBufferTimer < maxJumpBufferTime)
            {
                velocity.y = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y));
                jumpBufferTimer = maxJumpBufferTime;
            }

        }
        else
        {
            animator.SetBool("isGrounded", false);
        }


        float acceleration = enemyController.isGrounded ? walkAcceleration : airAcceleration;
        float deceleration = enemyController.isGrounded ? walkDeceleration : 0;

        // apply gravity before moving
        if (velocity.y < 0)
        {
            velocity.y += Physics2D.gravity.y * fallMultiplier * Time.deltaTime;
        }
        else
        {
            velocity.y += Physics2D.gravity.y * Time.deltaTime;
        }

        ///////////////////////////////////////////////////////////////////////////////
        float distance = Vector2.Distance(playerTransform.position, transform.position);
        if (distance <= agroRange)
        {
            isAgro = true;
        }
        directionToPlayer = (playerTransform.position - transform.position).normalized;
        if (isAgro && distance > attack1Range)
        {
            /*if (distance <= dashRange && Time.time >= lastDash + dashCooldown && !isDashing && distance > 3.0f)
            {
                isDashing = true;
                Vector2 move = directionToPlayer * speed * Time.deltaTime * dashSpeed;
                move.y += 2;
                velocity.x = Mathf.MoveTowards(velocity.x, speed, acceleration * Time.deltaTime);
                animator.SetFloat("speed", 2.0f);
                lastDash = Time.time;
            }
            else if (isDashing)
            {
                if (Time.time - lastDash > dashTime || rb.velocity == Vector2.zero)
                {
                    isDashing = false;
                }
            }*/
            Debug.Log("ici " + enemyController.isGrounded + "et" + isDashing);
            if (enemyController.isGrounded && !isDashing)
            {
                if (directionToPlayer.x > 0)
                {
                    velocity.x = Mathf.MoveTowards(velocity.x, speed, acceleration * Time.deltaTime);

                } else
                {
                    velocity.x = Mathf.MoveTowards(velocity.x, - speed, acceleration * Time.deltaTime);

                }
                Vector2 move = directionToPlayer * speed * Time.deltaTime;
                Debug.Log("moved");
                
            }
            enemyController.move(velocity * Time.deltaTime);
            // update animator
            animator.SetFloat("xSpeed", Mathf.Abs(velocity.x));
            animator.SetFloat("ySpeed", -velocity.y);
            // grab our current _velocity to use as a base for all calculations
            velocity = enemyController.velocity;

            Vector3 localScale = GFXTransform.localScale;
            Vector3 nameLS = nameTransform.localScale;
            Vector3 levelLS = levelTransform.localScale;
            if (velocity.x >= 0.01f && localScale.x < 0)
            {
                localScale.x = -localScale.x;
                nameLS.x = -nameLS.x;
                GFXTransform.localScale = localScale;
                //nameTransform.localScale = nameLS;
                attack1Offset.x = -attack1Offset.x;
            }
            else if (velocity.x <= -0.0f && localScale.x > 0)
            {
                localScale.x = -localScale.x;
                GFXTransform.localScale = localScale;
                levelLS.x = -levelLS.x;
                //levelTransform.localScale = levelLS;
                attack1Offset.x = -attack1Offset.x;
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
    void attack1()
    {
        Vector3 pos = transform.position;
        pos.x += attack1Offset.x;
        pos.y += attack1Offset.y;

        Collider2D[] colInfo = Physics2D.OverlapCircleAll(pos, attack1Range, attackMask);
        foreach(Collider2D hitten in colInfo)
        {
            if (hitten.gameObject.tag == "Player")
            {
                hitten.GetComponent<PlayerController>().TakeDamage(this.transform, attack1Damage);
            }
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

 /*   private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = true;
        }
    }*/

    void OnDrawGizmosSelected()
    {
        Vector3 pos = transform.position;
        pos.x += attack1Offset.x;
        pos.y += attack1Offset.y;

        Gizmos.DrawWireSphere(pos, attack1Range);


    }
}

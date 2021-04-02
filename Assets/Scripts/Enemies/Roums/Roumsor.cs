using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roumsor : Enemy
{

	[Header("Movement")]
    [SerializeField] float walkAcceleration = 2f;
    [SerializeField] float fallMultiplier = 2.5f;


	[Header("Attack")]
    [SerializeField] PolygonCollider2D attackCollider;
    [SerializeField] LayerMask playerLayer;
    [SerializeField]  float agroRange;

    bool isAgro = false;
    Vector3 playerDirection;

    public float attack1Range;
    float attack1Damage = 10.0f;
    public float attackSpeed;
    float lastAttack = 0.0f;



    protected override void Setup()
    {
        base.Setup();
    }

    public void Update()
    {
        animator.SetBool("isGrounded", characterController.isGrounded);

        //Aggro check
        float distance = Vector2.Distance(playerTransform.position, transform.position);
        isAgro = (distance <= agroRange) ? true : false;

        if(!isStun) 
        {
            playerDirection = (playerTransform.position - transform.position).normalized;
            if (isAgro && distance > attack1Range && characterController.isGrounded)
            {
                velocity.x = Mathf.MoveTowards(velocity.x, playerDirection.x * speed, walkAcceleration * Time.deltaTime);
            }
            else if (distance <= attack1Range && Time.time >= lastAttack + 1 / attackSpeed)
            {
                velocity.x = 0;
                animator.SetTrigger("attack1");
                lastAttack = Time.time;
            }
        }

        // apply gravity before moving
        ApplyGravity();

        // move
        characterController.move(velocity * Time.deltaTime);

        // FLip spirte if not looking in the right direction
        if ((velocity.x > 0 && !isFacingRight) || (velocity.x < 0 && isFacingRight))
        {
            Flip();
        }


        // update animator
        animator.SetFloat("xSpeed", Mathf.Abs(velocity.x));
        animator.SetFloat("ySpeed", -velocity.y);

        // grab our current _velocity to use as a base for all calculations
        velocity = characterController.velocity;

    }

    void ApplyGravity()
    {
        if (velocity.y < 0)
        {
            velocity.y += Physics2D.gravity.y * fallMultiplier * Time.deltaTime;
        }
        else
        {
            velocity.y += Physics2D.gravity.y * Time.deltaTime;
        }
    }

    public void Attack1()
    {
        List<Collider2D> hitten = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(playerLayer);
        filter.useTriggers = true;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roumsor : Enemy
{
    [Header("Stats")]
    [SerializeField] const float MAX_ENDURANCE = 3f;

    [Header("Movement")]
    [SerializeField] float walkAcceleration = 2f;
    [SerializeField] float fallMultiplier = 2.5f;
    [SerializeField] float gravityModifier = 2;

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
        currentEndurance = MAX_ENDURANCE;
    }

    public void Update()
    {
        if (currentEndurance < 0)
        {
            isTired = true;
            animator.SetBool("isTired", true);
        }
        if (currentEndurance  < MAX_ENDURANCE)
        {
            currentEndurance += (Time.deltaTime / enduranceRechargeDividor);
        }

        if (isTired && currentEndurance > MAX_ENDURANCE - 0.5) {
            isTired = false;
            animator.SetBool("isTired", false);
        }
        gameObject.GetComponent<CapsuleCollider2D>().enabled = !isTired;

        animator.SetBool("isGrounded", characterController.isGrounded);
        if (isKnockbacked && characterController.isGrounded)
        {
            isKnockbacked = false;
        }

        //Aggro check
        float distance = Vector2.Distance(playerTransform.position, transform.position);
        isAgro = (distance <= agroRange) ? true : false;

        if(!isStun && !isKnockbacked && !isTired) 
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

        // FLip sprite if not looking in the right direction
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
        if (isKnockbacked)
        {
            velocity.y += (Physics2D.gravity.y / gravityModifier) * fallMultiplier * Time.deltaTime;
            return;
        }

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

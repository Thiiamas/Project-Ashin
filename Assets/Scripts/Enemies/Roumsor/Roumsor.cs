using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roumsor : Enemy
{

    [Header("Movement")]
    [SerializeField] float walkAcceleration = 2f;
    [SerializeField] protected float MAX_ENDURANCE;


    [Header("Agro")]
    [SerializeField] LayerMask playerLayer;
    [SerializeField] float agroRange;
    bool isAgro = false;
    Vector3 playerDirection;
    CapsuleCollider2D capsuleCollider;

    
    [Header("Basic Attack")]
    [SerializeField] Collider2D basicAttackCollider;
    [SerializeField] float basicAttackRange;
    [SerializeField] float basicAttackDamage = 10.0f;
    [SerializeField] float basicAttackCooldown;


    [Header("Endurance")]
    [SerializeField] float maxEndurance;
	[SerializeField] float enduranceRecoverMultiplier;
    float currentEndurance;
    bool isTired;


    float lastAttack = 0.0f;



	#region getters

	public bool IsTired { get { return isTired; } }
	
	#endregion

    protected override void Setup()
    {
        base.Setup();
        currentEndurance = MAX_ENDURANCE;
        capsuleCollider = gameObject.GetComponent<CapsuleCollider2D>();
    }

    public void Update()
    {
        if(!isDead)
        {
            isTired = true;
        }
        if (currentEndurance  < MAX_ENDURANCE)
        {
            currentEndurance += Time.deltaTime * enduranceRechargeMultiplicator;
        }

        // mettre variable
        if (isTired && currentEndurance > MAX_ENDURANCE - 0.5) {
            isTired = false;
        }

        capsuleCollider.enabled = !isTired;

            if (currentEndurance < maxEndurance) {
                currentEndurance += Time.deltaTime * enduranceRecoverMultiplier;
            }

            //Aggro check
            float distance = Vector2.Distance(playerTransform.position, transform.position);
            isAgro = (distance <= agroRange);

            if(!isKnockbacked && !isTired && isAgro) 
            {
                // Move
                playerDirection = (playerTransform.position - transform.position).normalized;
                if (distance > basicAttackRange && characterController.isGrounded)
                {
                    velocity.x = Mathf.MoveTowards(velocity.x, playerDirection.x * speed, walkAcceleration * Time.deltaTime);
                    
                    //Flip
                    if ((velocity.x > 0 && !isFacingRight) || (velocity.x < 0 && isFacingRight)) {
                        Flip();
                    }                
                }

                // Attack
                else if (distance <= basicAttackRange && Time.time >= lastAttack + basicAttackCooldown)
                {
                    velocity.x = 0;
                    isAttacking = true;
                }
            }
        // apply gravity before moving
        ApplyGravity();

        // move
        characterController.move(velocity * Time.deltaTime);

        // grab our current velocity to use as a base for all calculations
        velocity = characterController.velocity;

    }


    public void BasicAttack()
    {
        List<Collider2D> hitten = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(playerLayer);
        filter.useTriggers = true;
        Physics2D.OverlapCollider(basicAttackCollider, filter, hitten);

        foreach (Collider2D hit in hitten)
        {
            if (hit.gameObject.tag == "Player")
            {
                hit.GetComponent<PlayerController>().TakeDamage(this.transform, basicAttackDamage);
            }
        }

        isAttacking = false;
        lastAttack = Time.time;
    }
}

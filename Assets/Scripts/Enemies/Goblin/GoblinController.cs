using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinController : Enemy
{

    [Header("Movement")]
    [SerializeField] float walkAcceleration = 2f;


    [Header("Agro")]
    [SerializeField] float agroRange;
    bool isAgro = false;

    
    [Header("Bomb Attack")]
    [SerializeField] float bombAttackRange;
    [SerializeField] float bombAttackCooldown;
    [SerializeField] Vector2 bombThrowForce;
    [SerializeField] Transform bombSpawnPoint;
    [SerializeField] GameObject bombPrefab;


    Vector3 playerDirection;
    float lastAttack = 0.0f;
    const float FLEE_MARGIN = 1f;


	#region getters

	//public bool IsTired { get { return isTired; } }
	
	#endregion

    protected override void Setup()
    {
        base.Setup();
    }

    public void Update()
    {
        if(!isDead)
        {

            //Aggro check
            float distance = Vector2.Distance(playerTransform.position, transform.position);
            isAgro = (distance <= agroRange);

            if(!isKnockbacked && isAgro) 
            {
                playerDirection = (playerTransform.position - transform.position).normalized;

                bool attackHasCooldown = Time.time >= lastAttack + bombAttackCooldown || lastAttack == 0;
                if (distance < bombAttackRange && attackHasCooldown)
                {
                    velocity.x = 0;
                    isAttacking = true;
                }
                else if (distance < bombAttackRange - FLEE_MARGIN)
                {
                    velocity.x = Mathf.MoveTowards(velocity.x, -playerDirection.x * speed, walkAcceleration * Time.deltaTime);
                    if ((velocity.x > 0 && !isFacingRight) || (velocity.x < 0 && isFacingRight)) {
                        Flip();
                    }
                } 
                else 
                {
                    velocity.x = 0;
                    if(playerDirection.x > 0 && !isFacingRight || playerDirection.x < 0 && isFacingRight) {
                        Flip();
                    }
                }


            } 
            
        }

        // apply gravity before moving
        ApplyGravity();

        // move
        characterController.move(velocity * Time.deltaTime);

        // grab our current velocity to use as a base for all calculations
        velocity = characterController.velocity;

    }


    public void BombAttack()
    {
        isAttacking = false;
        lastAttack = Time.time;
        Rigidbody2D bombRb = Instantiate(bombPrefab, bombSpawnPoint.position, Quaternion.identity).GetComponent<Rigidbody2D>();
        if(!isFacingRight && bombThrowForce.x > 0 ){
            bombThrowForce.x *= -1;
        }
        bombRb.AddForce(bombThrowForce);
    }
}

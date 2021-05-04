using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : Enemy
{

    [Header("Agro")]
    [SerializeField] LayerMask playerLayer;
    [SerializeField] float agroRange;
    bool isAgro = false;
    Vector3 playerDirection;

    
    [Header("Basic Attack")]
    [SerializeField] Collider2D basicAttackCollider;
    [SerializeField] float basicAttackDamage = 10.0f;


    bool isShielding = false;


	#region getters

	public bool IsTired { get { return isTired; } }
	public bool IsShielding { get { return isShielding; } }
	
	#endregion

    protected override void Setup()
    {
        base.Setup();
    }

    public void Update()
    {
        
        //Aggro check
        float distance = Vector2.Distance(playerTransform.position, transform.position);
        isAgro = (distance <= agroRange);
        if(!isKnockbacked && isAgro && !isDead && !isShielding) 
        {
            // Move
            playerDirection = (playerTransform.position - transform.position).normalized;
            if (characterController.isGrounded)
            {
                velocity.x = Mathf.MoveTowards(velocity.x, playerDirection.x * speed, acceleration * Time.deltaTime);
                
                //Flip
                if ((velocity.x > 0 && !isFacingRight) || (velocity.x < 0 && isFacingRight)) {
                    Flip();
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

    public override void TakeDamage(Transform damageDealer, float damage)
    {
		if(isDead)
        {
			DestroySelf();
		}

        if(IsLookingAtPlayer() && !isAttacking)
        {
            Shield();
        }
        else 
        {
            health -= damage;

            DamagePopup.Create(transform.position + Vector3.one, damage);
            Instantiate(GameManager.Instance.HurtEffectPrefab, transform.position, Quaternion.identity);

            if (health <= 0) {
                isDead = true;
            }
        }
    }

    void Shield()
    {
        isShielding = true;
        velocity = Vector2.zero;
    }


    public void BasicAttack()
    {
        List<Collider2D> hits = Utilities.GetCollidersInCollider(basicAttackCollider, playerLayer);

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject.tag == "Player")
            {
                hit.GetComponent<PlayerController>().TakeDamage(this.transform, basicAttackDamage);
            }
        }
    }

    public void StopShielding()
    {
        isShielding = false;
        isAttacking = true;
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

	protected CharacterController2D characterController;
	protected Rigidbody2D rb;
	protected Transform GFX;
	protected Animator animator;
	protected Vector3 velocity = Vector3.zero;
	protected Transform playerTransform;


	[Header("Stats")]
	[SerializeField] protected float maxHealth = 10f;
	[SerializeField] protected float speed = 4f;
	[SerializeField] protected float collisionDamage = 2f;
    [SerializeField] protected float acceleration;


	[Header("Endurance")]
    [SerializeField] protected float maxEndurance;
	[SerializeField] protected float enduranceRechargeMultiplicator;
	protected float currentEndurance;
	protected bool isTired;
	

	[Header("KnockBack")]
    [SerializeField] protected Vector2 knockBackForce;
	[SerializeField] protected float knockBackDeceleration;
	protected bool isKnockbacked = false;


	[Header("Stun")]
    [SerializeField] float stunTime = 1f;
	protected bool isStun = false;
	Timer stunTimer;



	protected float health;
	protected float lastAttack = 0.0f;

	protected bool isFacingRight = true;
	protected bool isDead = false;
	protected bool isAttacking = false;

	const float FALL_MULTIPLIER = 2.5f;

	// Might be usefull one day

	//[SerializeField] protected int level = 1;

	/*[Header("Text")]
	[SerializeField] TextMesh textMeshName;
	[SerializeField] TextMesh textMeshLevel;*/


	#region getters

	public float Damage { get { return collisionDamage; } }
	public Vector3 Velocity {  get { return velocity; } set { velocity = value; } }
	public bool IsDead { get { return isDead; } }
	public bool IsAttacking { get { return isAttacking; } }
	public bool IsGrounded { get { return characterController.isGrounded; } }
	public bool IsStun { get { return isStun; } }
	
	#endregion


	void Start()
	{
        characterController = GetComponent<CharacterController2D>();
        rb = GetComponent<Rigidbody2D>();

		health = maxHealth;

		playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
		stunTimer = new Timer(stunTime);

		Setup();

		/*
		textMeshName.text = gameObject.name;
		textMeshLevel.text = level.ToString();
		*/

		/*		
		GFX = animator.transform;
		if (textMeshLevel != null && textMeshName != null)
        {
			textMeshName.text = gameObject.name;
			textMeshLevel.text = level.ToString();
		}
		*/
	}


	protected virtual void Setup() {}


    public void DestroySelf()
	{
		//GameManager.Instance.WaveSpawner.CurrentWave.RemoveEnemy(this);
		Destroy(gameObject);
	}


    public virtual void TakeDamage(Transform damageDealer, float damage)
    {
		if(isDead){
			DestroySelf();
		}

        health -= damage;

        DamagePopup.Create(transform.position + Vector3.one, damage);
        Instantiate(GameManager.Instance.HurtEffectPrefab, transform.position, Quaternion.identity);

        if (health <= 0) {
            isDead = true;
        }

		Vector3 direction = (transform.position - damageDealer.position).normalized;			
		StartCoroutine(KnockBack(direction, knockBackForce) );
    }


	public IEnumerator KnockBack(Vector3 direction, Vector2 force)
	{
		if(direction.y < 0.5f && direction.y >= 0){
			direction.y = 0.5f;
		}
		else if(direction.y > -0.5f && direction.y < 0){
			direction.y = -0.5f;
		}

		isKnockbacked = true;
		velocity = direction * force;
		characterController.move(velocity * Time.deltaTime);
		while (velocity.x > .1f || velocity.x < -.1f || velocity.y > .1f || velocity.y < -.1f)
		{
			velocity.x = Mathf.MoveTowards(velocity.x, 0, knockBackDeceleration * Time.deltaTime);
			velocity.y = Mathf.MoveTowards(velocity.y, 0, knockBackDeceleration * Time.deltaTime);
			characterController.move(velocity * Time.deltaTime);
			yield return new WaitForEndOfFrame();
		}
		isKnockbacked = false;

		//StartCoroutine(Stun());
	}

	public IEnumerator Stun()
	{
		if(isStun == true){
			yield break;
		}

		velocity.x = 0;
		isStun = true;
		stunTimer.Start();
		while (stunTimer.IsOn)
		{
			transform.Rotate( new Vector3(0, 0.3f, 0) );
			stunTimer.Decrease();
			yield return new WaitForEndOfFrame();
		}
		transform.rotation = Quaternion.identity;
		isFacingRight = true;
		isStun = false;
	}


    protected void ApplyGravity()
    {
        if (velocity.y < 0) {
            velocity.y += Physics2D.gravity.y * FALL_MULTIPLIER * Time.deltaTime;
        } else {
            velocity.y += Physics2D.gravity.y * Time.deltaTime;
        }
    }

	

	#region flip

	protected void Flip()
	{
		isFacingRight = !isFacingRight;
		transform.Rotate(0f, 180f, 0f);
	}

	protected void FlipTowardPlayer()
	{
		if (!IsLookingAtPlayer())
        {
			Flip();
		}
	}
	protected bool IsLookingAtPlayer() 
	{
		bool playerIsToTheRight = playerTransform.position.x - transform.position.x > 0;
		if ((isFacingRight && !playerIsToTheRight) || (!isFacingRight && playerIsToTheRight))
        {
			return false;
		}
		return true;
	}

	#endregion 


	#region Attack

	public void StopAttacking()
	{
		isAttacking = false;
        lastAttack = Time.time;
	}

	#endregion 


}

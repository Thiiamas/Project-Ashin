using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	protected CharacterController2D characterController;
	protected Rigidbody2D rb;
	public Animator animator;
	Transform GFX;


	[Header("Stats")]
	[SerializeField] protected float maxHealth = 10f;
	[SerializeField] protected int level = 1;
	[SerializeField] protected float damage = 2f;
	[SerializeField] protected float speed = 4f;
	[SerializeField] protected float currentEndurance;
	[SerializeField] protected float enduranceRechargeDividor;
	[SerializeField] protected bool isTired;


	[Header("KnockBack")]
    [SerializeField] Vector2 knockBackSpeed = new Vector2(15, 5);
	[SerializeField] float knockBackDeceleration;
	protected bool isKnockbacked = false;
	Timer knockBackTimer;


	[Header("Stun")]
    [SerializeField] float stunTime = 1f;
	protected bool isStun = false;
	Timer stunTimer;

	[Header("Text")]
	[SerializeField] TextMesh textMeshName;
	[SerializeField] TextMesh textMeshLevel;


	[SerializeField] protected Vector3 velocity = Vector3.zero;
	protected Transform playerTransform;
	protected float health;
	protected bool isFacingRight = true;
	protected bool isDead = false;
	protected bool isAttacking = false;


	#region getters

	public float Damage { get { return damage; } }
	public Vector3 Velocity { get { return velocity; } }
	public bool IsDead { get { return isDead; } }
	public bool IsAttacking { get { return isAttacking; } }
	public bool IsGrounded { get { return characterController.isGrounded; } }
	public bool IsStun { get { return isStun; } }
	
	#endregion


	void Start()
	{
        characterController = GetComponent<CharacterController2D>();
        rb = GetComponent<Rigidbody2D>();

		GFX = animator.transform;
		textMeshName.text = gameObject.name;
		textMeshLevel.text = level.ToString();
		health = maxHealth;

		playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
		stunTimer = new Timer(stunTime);

		Setup();
	}


	protected virtual void Setup()
    {
    }


    public void Die()
	{
		Destroy(gameObject);
	}


    public void TakeDamage(Transform damageDealer, float damage)
    {
        health -= damage;
		if (!isTired)
        {
			currentEndurance -= 1;
		}
        DamagePopup.Create(transform.position + Vector3.one, damage);
        Instantiate(GameManager.Instance.HurtEffectPrefab, transform.position, Quaternion.identity);

        if (health <= 0) {
            isDead = true;
        }

		if (damageDealer.position.y - transform.position.y > 0.5)
		{
			VirtualCameraManager.Instance.ShakeCamera(3, 0.7f);
		}
		Vector3 direction = (transform.position - damageDealer.position).normalized;
		StartCoroutine( KnockBack(direction, knockBackSpeed) );
    }


	public IEnumerator KnockBack(Vector3 direction, Vector2 force)
	{
		isKnockbacked = true;
		velocity = direction * force;
		characterController.move(velocity * Time.deltaTime);

		while (velocity.x > .1f || velocity.x < -.1f)
		{
			velocity.x = Mathf.MoveTowards(velocity.x, 0, knockBackDeceleration * Time.deltaTime);
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

	protected void Flip()
	{
       	if ((velocity.x > 0 && !isFacingRight) || (velocity.x < 0 && isFacingRight))
       	{
			isFacingRight = !isFacingRight;
			transform.Rotate(0f, 180f, 0f);
       	}
	}



}

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
    [SerializeField] float knockBackTime = .2f;
	[SerializeField] protected bool isKnockbacked = false;
	[SerializeField] Vector2 knockBackForce;
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
		knockBackTimer = new Timer(knockBackTime);
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

        if (health <= 0) {
            Die();
        }
        else {
			Vector3 direction = (transform.position - damageDealer.position).normalized;
			if (damageDealer.position.y - transform.position.y > 0.5)
            {
				VirtualCameraManager.Instance.ShakeCamera(3, 0.7f);
				StartCoroutine( KnockBack(direction, knockBackForce) ) ;
            }
            else
            {
				StartCoroutine( KnockBack(direction, knockBackSpeed) );
			}

		}
    }


	public IEnumerator KnockBack(Vector3 direction, Vector2 force)
	{
		isKnockbacked = true;
		knockBackTimer.Start();
		while (knockBackTimer.IsOn)
		{
			Vector3 knockBackVelocity = direction * force;
			characterController.move(knockBackVelocity * Time.deltaTime);
			velocity = knockBackVelocity;
			knockBackTimer.Decrease();
			yield return new WaitForEndOfFrame();
		}
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

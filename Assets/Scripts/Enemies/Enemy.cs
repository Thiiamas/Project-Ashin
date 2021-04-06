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
	[SerializeField] protected float speed = 2f;


    [Header("KnockBack")]
    [SerializeField] Vector2 knockBackSpeed = new Vector2(15, 5);
    [SerializeField] float knockBackTime = .2f;
	Timer knockBackTimer;

	
    [Header("Stun")]
    [SerializeField] float stunTime = 1f;
	protected bool isStun = false;


	[Header("Text")]
	[SerializeField] TextMesh textMeshName;
	[SerializeField] TextMesh textMeshLevel;


	protected Vector3 velocity = Vector3.zero;
	protected Transform playerTransform;
	protected float health;
	protected bool isFacingRight = true;

	public float Damage { get { return damage; } }


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

		Setup();
	}


	protected virtual void Setup()
    {
    }


	// Update is called once per frame
	void Update()
	{
	}
		
    void Die()
	{
		animator.SetBool("isDead", true);
		Destroy(gameObject);
	}


    public void TakeDamage(Transform damageDealer, float damage)
    {
        health -= damage;
        Instantiate(GameManager.Instance.HurtEffectPrefab, transform.position, Quaternion.identity);
        GameManager.Instance.SpawnDamagePopup(transform.position + Vector3.one, damage);

        if (health <= 0) {
            Die();
        }
        else {
            Vector3 direction = (transform.position - damageDealer.position).normalized;
            StartCoroutine(KnockBack(direction));
        }
    }


    public IEnumerator KnockBack(Vector3 direction) 
    {
		isStun = true;
        knockBackTimer.Start();
        while ( knockBackTimer.IsOn ) 
        {
            Vector3 knockBackVelocity = direction * knockBackSpeed;
            characterController.move(knockBackVelocity * Time.deltaTime);
            knockBackTimer.Decrease();            
            yield return new WaitForEndOfFrame();
        }
		velocity.x = 0;
		Invoke("StopStun", stunTime);
    }

	private void StopStun(){
		isStun = false;
	}


	protected void Flip()
	{
		isFacingRight = !isFacingRight;
		transform.Rotate(0f, 180f, 0f);
	}



}

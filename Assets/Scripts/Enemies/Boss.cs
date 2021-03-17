using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
	public Transform player;
	public Animator animator;
	float maxHealth = 100;
	float health;

	public bool isFlipped = false;
	void Start()
	{
		health = maxHealth;
	}

	// Update is called once per frame
	void Update()
	{
		if (health <= 0)
		{
			die();
		}
	}

	public void LookAtPlayer()
	{
		Vector3 flipped = transform.localScale;
		flipped.z *= -1f;

		if (transform.position.x > player.position.x && !isFlipped)
		{
			transform.localScale = flipped;
			transform.Rotate(0f, 180f, 0f);
			isFlipped = true;
		}
		else if (transform.position.x < player.position.x && isFlipped)
		{
			transform.localScale = flipped;
			transform.Rotate(0f, 180f, 0f);
			isFlipped = false;
		}
	}
	void die()
	{
		Debug.Log("Enemie is dead");
		animator.SetBool("isDead", true);
		
	}

    public void DestroyBoss()
    {
		Destroy(gameObject);
	}
    public void takeDamage(float damage)
	{
		health -= damage;
		animator.SetTrigger("hurt");
		Debug.Log("vie restante" + health);
	}
}

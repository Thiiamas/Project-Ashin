using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
	[SerializeField] Animator animator;
	float maxHealth = 100;
	float health;

	void Start()
	{
		health = maxHealth;
	}

	// Update is called once per frame
	void Update()
	{
	}

	/*
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
	*/

	void Die()
	{
		Debug.Log("Enemie is dead");
		animator.SetBool("isDead", true);
	}

    /*public void DestroyBoss()
    {
		Destroy(gameObject);
	}*/

    public void TakeDamage(float damage)
	{
		health -= damage;
		animator.SetTrigger("hurt");
		if (health <= 0) {
			Die();
		}	
	}
}

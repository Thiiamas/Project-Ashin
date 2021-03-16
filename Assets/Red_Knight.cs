using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Red_Knight : MonoBehaviour
{
	public Transform player;

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

	void die()
	{
		Debug.Log("Enemie is dead");
		Destroy(gameObject);
	}
	public void takeDamage(float damage)
	{
		health -= damage;
	}

	public void LookAtPlayer()
	{
		Vector3 flipped = transform.localScale;
		flipped.z *= -1f;

		if (transform.position.x > player.position.x && !isFlipped)
		{
			transform.localScale = flipped;
			transform.Rotate(0f, 180f, 0f);
			isFlipped = false;
		}
		else if (transform.position.x < player.position.x && isFlipped)
		{
			transform.localScale = flipped;
			transform.Rotate(0f, 180f, 0f);
			isFlipped = true;
		}
	}

}

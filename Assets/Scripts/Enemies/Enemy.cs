using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	protected Transform playerTransform;
	public Animator animator;
	Transform GFX;
	protected bool isFacingRight = true;
	[SerializeField] TextMesh textMeshName;
	[SerializeField] TextMesh textMeshLevel;

	float maxHealth = 100;
	float health;

	int level = 01;

	public bool isFlipped = false;
	void Start()
	{
		playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
		GFX = animator.transform;
		textMeshName.text = gameObject.name;
		textMeshLevel.text = level.ToString();
		health = maxHealth;
		Setup();
	}


	protected virtual void Setup()
    {
    }

	protected virtual void Flip()
	{
		isFacingRight = !isFacingRight;
		transform.Rotate(0f, 180f, 0f);
	}


	// Update is called once per frame
	void Update()
	{
		if (health <= 0)
		{
			die();
		}
	}



    //used to flip the sprite
    /*	public void LookAtPlayer()
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
        }*/

		
    void die()
	{
		Debug.Log("Enemie is dead");
		animator.SetBool("isDead", true);

	}

	//function used during death 
	public void DestroyEnemy()
	{
		Destroy(gameObject);
	}
	public void takeDamage(float damage)
	{
		health -= damage;
	}
}

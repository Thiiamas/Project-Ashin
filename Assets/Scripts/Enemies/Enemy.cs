using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	protected Transform playerTransform;
	protected Transform tranform;
	public Animator animator;

	TextMesh textMeshName;
	TextMesh textMeshLevel;
	protected Transform nameTransform;
	protected Transform levelTransform;
	Quaternion initRotName;
	Quaternion initRotlvl;

	float maxHealth = 100;
	float health;

	int level = 01;

	public bool isFlipped = false;
	void Start()
	{
		playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
		tranform = GetComponent<Transform>();
		TextMesh[] TextMeshList = GetComponentsInChildren<TextMesh>();
		foreach(TextMesh textMesh in TextMeshList)
        {
			Debug.Log(textMesh.gameObject.name);
			if (textMesh.gameObject.name == "Name")
            {
				textMeshName = textMesh;
				nameTransform = textMesh.gameObject.GetComponent<Transform>();
				initRotName = nameTransform.rotation;

			} else if(textMesh.gameObject.name == "Level")
            {
				textMeshLevel = textMesh;
				levelTransform = textMesh.gameObject.GetComponent<Transform>();
				initRotlvl = levelTransform.rotation;
			}
        }
		textMeshName.text = gameObject.name;
		textMeshLevel.text = level.ToString();
		health = maxHealth;
		Setup();
	}

	protected virtual void Setup()
    {

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

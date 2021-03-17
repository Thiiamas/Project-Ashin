using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    // Start is called before the first frame update

    float maxHealth = 100;
    float health;

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
        Debug.Log("damage taken");
    }
}

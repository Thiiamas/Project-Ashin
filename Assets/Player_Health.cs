using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Health : MonoBehaviour
{
    // Start is called before the first frame update

    float maxHealth = 100;
    [SerializeField] float health;

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
        Debug.Log("Player is dead");
    }
    public void takeDamage(float damage)
    {
        Debug.Log("Player is Hurt");
        health -= damage;
    }
}

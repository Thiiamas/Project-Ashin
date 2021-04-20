using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedBall : EnemyProjectiles
{
    public float speed = 10f;
    float damage = 10f;
    Rigidbody2D rb;

    public void Launch(Vector3 direction)
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = direction * speed;
    }

    private void Awake()
    {
        Destroy(gameObject, 5);
    }


    private void OnTriggerEnter2D(Collider2D collider)
    {
        Destroy(gameObject, Time.deltaTime);
        if (collider.tag != "Player")
        {
            return;
        }
        PlayerController playerHealth = collider.GetComponent<PlayerController>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(this.transform, damage);
        }
    }
}

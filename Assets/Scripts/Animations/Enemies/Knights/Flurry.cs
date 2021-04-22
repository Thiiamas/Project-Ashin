using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flurry : MonoBehaviour
{
    public float speed = 15f;
    float damage = 10f;
    Rigidbody2D rb;

    private void Awake()
    {
        Destroy(gameObject, 1f);
    }

    public void Launch(Vector3 direction)
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        //Destroy(gameObject, Time.deltaTime);
        if (collider.tag != "Player")
        {
            Destroy(gameObject, Time.deltaTime);
            return;
        }

        PlayerController playerHealth = collider.GetComponent<PlayerController>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(this.transform, damage);
        }
    }
}

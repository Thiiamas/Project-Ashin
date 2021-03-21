using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedBall : MonoBehaviour
{
    public float speed = 10f;
    float damage = 10f;
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        

    }

    public void Setup(Vector3 direction)
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
        if (collider.tag != "Player")
        {
            return;
        }
        Player_Health playerHealth = collider.GetComponent<Player_Health>();
        if (playerHealth != null)
        {
            playerHealth.takeDamage(damage);
            Destroy(gameObject);
        }
    }
}

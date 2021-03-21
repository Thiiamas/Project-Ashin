using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roums : Enemy
{
    public Transform firePoint;
    Rigidbody2D rb;
    public GameObject redBallPrefab;
    
    Transform tranform;
    Transform GFXTransform;

    float speed = 650f;
    public float detectionRange;
    public float attackRange;
    public float attackSpeed;
    float lastAttack = 0.0f;
    // Start is called before the first frame update

    protected override void Setup()
    {
        base.Setup();
        tranform = GetComponent<Transform>();
        GFXTransform = GetComponentInChildren<Transform>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Update()
    {
        float distance = Vector2.Distance(playerTransform.position, tranform.position);
        Vector3 directionToPlayer = (playerTransform.position - tranform.position).normalized;

        if (distance < detectionRange && distance > attackRange)
        {
            Vector2 move = directionToPlayer * speed * Time.deltaTime;
            rb.velocity = move;
            animator.SetFloat("speed", 2.0f);

            Vector3 localScale = GFXTransform.localScale;
            if (rb.velocity.x >= 0.01f && localScale.x < 0)
            {
                localScale.x = -localScale.x;
                GFXTransform.localScale = localScale;
            }
            else if (rb.velocity.x <= -0.0f && localScale.x > 0)
            {
                localScale.x = -localScale.x;
                GFXTransform.localScale = localScale;
            }
        }
        else if(distance <= attackRange && Time.time >= lastAttack + 1 / attackSpeed)
        {
            rb.velocity = Vector2.zero;
            animator.SetFloat("speed", -1.0f);
            animator.SetTrigger("rangedAttack");

            RangedAttack();
            lastAttack = Time.time;
        }
    }
    void RangedAttack()
    {
        
            animator.SetTrigger("rangedAttack");
            GameObject redBallGO = Instantiate(redBallPrefab, firePoint.position, transform.rotation);
            Vector3 direction = (playerTransform.position - redBallGO.transform.position).normalized;
            redBallGO.transform.rotation = Quaternion.LookRotation(Vector3.forward, -direction);

            redBallGO.GetComponent<RedBall>().Setup(direction);
 
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roums : Enemy
{
    EnemyController2D enemyController;
    Vector3 velocity = Vector3.zero;
    [SerializeField] float speed = 4f;
    [SerializeField] float walkAcceleration = 200f;
    [SerializeField] float walkDeceleration = 200f;
    [SerializeField] float airAcceleration = 2f;
    [SerializeField] float jumpHeight = 5f;
    [SerializeField] float fallMultiplier = 2.5f;

    public Transform firePoint;
    public GameObject redBallPrefab;

    
    Transform GFXTransform;

    bool isJumping = false;
    public float agroRange = 10f;
    bool isAgro = false;
    public float attackRange;
    public float attackSpeed;
    float lastAttack = 0.0f;
    // Start is called before the first frame update

    protected override void Setup()
    {
        base.Setup();
        tranform = GetComponent<Transform>();
        GFXTransform = GetComponentInChildren<Transform>();
        enemyController = GetComponent<EnemyController2D>();
    }

    public void Update()
    {
        if (enemyController.isGrounded)
        {
            velocity.y = 0;
            isJumping = false;
            animator.SetBool("isGrounded", true);
        }
        else
        {
            animator.SetBool("isGrounded", false);
        }
        float acceleration = enemyController.isGrounded ? walkAcceleration : airAcceleration;
        float deceleration = enemyController.isGrounded ? walkDeceleration : 0;
        // apply gravity before moving
        if (velocity.y < 0)
        {
            velocity.y += Physics2D.gravity.y * fallMultiplier * Time.deltaTime;
        }
        else
        {
            velocity.y += Physics2D.gravity.y * Time.deltaTime;
        }

        float distance = Vector2.Distance(playerTransform.position, tranform.position);
        if (distance < agroRange)
        {
            isAgro = true;
        }
        else
        {
            isAgro = false;
        }
        Vector3 directionToPlayer = (playerTransform.position - tranform.position).normalized;

        

        if (isAgro && distance > attackRange)
        {
            if (directionToPlayer.x > 0)
            {
                velocity.x = Mathf.MoveTowards(velocity.x, speed, acceleration * Time.deltaTime);

            }
            else
            {
                velocity.x = Mathf.MoveTowards(velocity.x, -speed, acceleration * Time.deltaTime);

            }
            Vector2 move = directionToPlayer * speed * Time.deltaTime;
            enemyController.move(velocity * Time.deltaTime);
            animator.SetFloat("xSpeed", velocity.x);
            animator.SetFloat("ySpeed", velocity.y);

/*            if (velocity.x >= 0.01f && localScale.x < 0)
            {
                localScale.x = -localScale.x;
                GFXTransform.localScale = localScale;
            }
            else if (velocity.x <= -0.0f && localScale.x > 0)
            {
                localScale.x = -localScale.x;
                GFXTransform.localScale = localScale;
            }*/

            
        }
        else if(distance <= attackRange && Time.time >= lastAttack + 1 / attackSpeed)
        {
            animator.SetTrigger("rangedAttack");
            lastAttack = Time.time;
        }
        Debug.Log(directionToPlayer);
        Vector3 localScale = GFXTransform.localScale;

        if (directionToPlayer.x >= 0.01f && localScale.x < 0)
        {
            localScale.x = -localScale.x;
            GFXTransform.localScale = localScale;
        }
        else if (directionToPlayer.x <= -0.0f && localScale.x > 0)
        {
            localScale.x = -localScale.x;
            GFXTransform.localScale = localScale;
        }
    }
   public void RangedAttack()
    {
        
            GameObject redBallGO = Instantiate(redBallPrefab, firePoint.position, transform.rotation);
            Vector3 direction = (playerTransform.position - redBallGO.transform.position).normalized;
            redBallGO.transform.rotation = Quaternion.LookRotation(Vector3.forward, -direction);

            redBallGO.GetComponent<RedBall>().Setup(direction);
 
    }
}

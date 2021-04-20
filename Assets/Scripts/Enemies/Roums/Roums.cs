using UnityEngine;

public class Roums : Enemy
{
    [Header("Stats")]
    [SerializeField] const float MAX_ENDURANCE = 3f;

    [Header("Movement")]
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
    const float ATTACKTIME = 5f;
    float attackTime;
    float  attackTimer;
    float lastAttack = 0.0f;

    /*protected override void Setup()
    {
        base.Setup();
        GFXTransform = GetComponentInChildren<Transform>();
        currentEndurance = MAX_ENDURANCE;
    }

    public void Update()
    {
        if (isAttacking)
        {
            if (attackTime < 0f)
            {
                attackTime -= Time.deltaTime;
            } else
            {
                isAttacking = false;
            }
        }
        if (characterController.isGrounded)
        {
            velocity.y = 0;
            isJumping = false;
            animator.SetBool("isGrounded", true);
        }
        else
        {
            animator.SetBool("isGrounded", false);
        }
        float acceleration = characterController.isGrounded ? walkAcceleration : airAcceleration;
        float deceleration = characterController.isGrounded ? walkDeceleration : 0;
        // apply gravity before moving
        if (velocity.y < 0)
        {
            velocity.y += Physics2D.gravity.y * fallMultiplier * Time.deltaTime;
        }
        else
        {
            velocity.y += Physics2D.gravity.y * Time.deltaTime;
        }

        float distance = Vector2.Distance(playerTransform.position, transform.position);
        if (distance < agroRange)
        {
            isAgro = true;
        }
        else
        {
            isAgro = false;
        }
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;

        

        if (!isStun && isAgro && distance > attackRange && !isAttacking)
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
        }
        else if(distance <= attackRange && Time.time >= lastAttack + 1 / attackSpeed && !isAttacking)
        {
            velocity = Vector3.zero;
            animator.SetTrigger("rangedAttack");
            isAttacking = true;
            lastAttack = Time.time;
        }

        //apply velocity to move
        characterController.move(velocity * Time.deltaTime);
        animator.SetFloat("xSpeed", velocity.x);
        animator.SetFloat("ySpeed", velocity.y);
        velocity = characterController.velocity;
        Flip();
    }
   public void RangedAttack()
    {
            attackTimer = ATTACKTIME;
            GameObject redBallGO = Instantiate(redBallPrefab, firePoint.position, transform.rotation);
            Vector3 direction = (playerTransform.position - redBallGO.transform.position).normalized;
            redBallGO.transform.rotation = Quaternion.LookRotation(Vector3.forward, -direction);

            redBallGO.GetComponent<RedBall>().Launch(direction);
 
    }*/


}

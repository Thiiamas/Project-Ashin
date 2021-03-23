using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class BlueKnightAI : MonoBehaviour
{

    Transform target;
    Vector2 targetPosition;
    float xTargetBox = 1f;
    float yTargetBox = 0.7f;
    public Transform GFX;

    public float speed = 500f;
    public float dashSpeed = 1.5f;
    public float startDashTime = 1f;
    float dashTime;

    public float nextWaypointDistance = 3f;

    float basicAttackRange;

    Path path;
    int currentWaypoint = 0;
    bool reachedEnd = false;

    Seeker seeker;
    Rigidbody2D rb;

    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        animator = GFX.GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        targetPosition = target.position;
        targetPosition.x += xTargetBox;
        basicAttackRange = GetComponent<EnemyAttack>().attackRange;

        dashTime = startDashTime;

 

       

        InvokeRepeating("UpdatePath", 0f, 0.5f);
        seeker.StartPath(rb.position, targetPosition, OnPathComplete);
    }

    void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, targetPosition, OnPathComplete);
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        targetPosition = target.position;
        //target a gauche de rb
        if (targetPosition.x < rb.position.x)
        {
            targetPosition.x += xTargetBox;
            //targetPosition.y += yTargetBox;
        }
        else if (targetPosition.x > rb.position.x) //droite
        {
            targetPosition.x -= xTargetBox;
        }
        Vector2 direction = Vector2.zero;
        if (path != null)
        {
            direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        }
        

        float distanceToBox = Vector2.Distance(rb.position, targetPosition);
        float xDistanceToPlayer = Mathf.Abs(rb.position.x - target.position.x);
/*        if (distanceToPlayer > basicAttackRange && distanceToPlayer < dashSpeed)
        {
            if (dashTime <= 0)
            {
                dashTime = startDashTime;
            }
            else
            {
                dashTime -= Time.deltaTime;
                Vector2 dashVector = direction * dashSpeed;
                rb.velocity = dashVector;
                animator.SetTrigger("attack");
            }

        }*/
        if (xDistanceToPlayer <= basicAttackRange)
        {
            rb.velocity = Vector2.zero;
            animator.SetTrigger("attack");
            return;
        }
        if (path == null)
            return;
        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEnd = true;
            animator.SetFloat("speed", -1f);
            return;
        }else
        {
            reachedEnd = false;
        }



        
        // ....* Time.deltaTime = pour pas que àa varier avec le frametate
        Vector2 force = direction * speed * Time.deltaTime;
        rb.AddForce(force);
        animator.SetFloat("speed", 2.0f);
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        Vector3 localScale = GFX.localScale;
        if (rb.velocity.x >= 0.01f && localScale.x < 0)
        {
            localScale.x = -localScale.x;
            GFX.localScale = localScale;
        }
        else if (rb.velocity.x <= -0.0f && localScale.x > 0)
        {
            localScale.x = -localScale.x;
            GFX.localScale = localScale;
        }
    }
}

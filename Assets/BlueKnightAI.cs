using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class BlueKnightAI : MonoBehaviour
{

    public Transform target;
    public Transform GFX;

    public float speed = 200f;
    public float nextWaypointDistance = 3f;

    float basicAttackRange = 2f;

    Path path;
    int currentWaypoint = 0;
    bool reachedEnd = false;

    Seeker seeker;
    Rigidbody2D rb;

    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, 0.5f);
        seeker.StartPath(rb.position, target.position, OnPathComplete);
    }

    void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
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

        float distanceToPlayer = Vector2.Distance(rb.position, target.position);
        if (distanceToPlayer <= basicAttackRange)
        {
            animator.SetTrigger("attack");
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
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

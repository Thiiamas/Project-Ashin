using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class FlyingEye : Enemy
{
    Rigidbody2D rb;
    Transform target;
    Path path;
    Seeker seeker;

    Transform GFX;


    Vector2 targetPosition;
    Vector2 direction = Vector2.zero;

    float xTargetBox = 0.5f;
    float speed = 600f;
    float nextWaypointDistance = 3f;
    int currentWaypoint = 0;

    PlayerController playerController;
    float attack1Dmg = 10f;

    protected override void Setup()
    {
        playerController = playerTransform.GetComponent<PlayerController>();
        GFX = animator.GetComponent<Transform>();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        Debug.Log("dansdada ");
        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }
    void UpdatePath()
    {
        Debug.Log("dans update");
        if (seeker.IsDone())
        {
            Debug.Log("dans update if ");
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
       
        if ( path == null || currentWaypoint >= path.vectorPath.Count) {
            return;
        }


        targetPosition = target.position;
        direction = ( (Vector2) path.vectorPath[currentWaypoint] - rb.position).normalized;
        

        //float distanceToBox = Vector2.Distance(rb.position, targetPosition);
        //float xDistanceToPlayer = Mathf.Abs(rb.position.x - targetPosition.x);

        // Time.deltaTime = pour ne pas varier selon le frame rate
        Vector2 force = direction * speed * Time.deltaTime;
        rb.AddForce(force);
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)  {
            currentWaypoint++;
        }

		if ( (rb.velocity.x > 0 && !isFacingRight) || (rb.velocity.x < 0 && isFacingRight) ) 
        {
			Flip();
		} 
    }


    public void attack1()
    {
        playerController.TakeDamage(this.transform, attack1Dmg);
    }

}

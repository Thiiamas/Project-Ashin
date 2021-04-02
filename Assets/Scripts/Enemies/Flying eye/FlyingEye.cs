using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class FlyingEye : Enemy
{
    Transform target;
    Path path;
    Seeker seeker;

    Vector2 targetPosition;
    Vector2 direction = Vector2.zero;

    float xTargetBox = 0.5f;
    float nextWaypointDistance = 1f;
    int currentWaypoint = 0;

    PlayerController playerController;

    protected override void Setup()
    {
        playerController = playerTransform.GetComponent<PlayerController>();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        target = playerTransform;
        InvokeRepeating("UpdatePath", 0f, 0.5f);
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

        if ( path == null || currentWaypoint >= path.vectorPath.Count || isStun ) {
            return;
        }

        targetPosition = target.position;

        direction = ( (Vector2) path.vectorPath[currentWaypoint] - rb.position).normalized;        
        characterController.move(direction * speed * Time.deltaTime);

        //float distanceToBox = Vector2.Distance(rb.position, targetPosition);
        //float xDistanceToPlayer = Mathf.Abs(rb.position.x - targetPosition.x);
        
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance <= nextWaypointDistance)  {
            currentWaypoint++;
        }

		if ( (direction.x > 0 && !isFacingRight) || (direction.x < 0 && isFacingRight) ) 
        {
			Flip();
		} 
    }


}

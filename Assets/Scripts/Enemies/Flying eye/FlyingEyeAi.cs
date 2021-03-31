using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class FlyingEyeAi : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    Transform target;
    Path path;
    Seeker seeker;

    [SerializeField] Transform GFX;


    Vector2 targetPosition;
    Vector2 direction = Vector2.zero;

    bool isFacingRight = true;
    float xTargetBox = 0.5f;
    float speed = 600f;
    float nextWaypointDistance = 3f;
    int currentWaypoint = 0;



    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        animator = GFX.GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;

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
       
        if ( path == null || currentWaypoint >= path.vectorPath.Count) {
            return;
        }


        targetPosition = target.position;
        //target a gauche de rb
        if (targetPosition.x < rb.position.x) {
            targetPosition.x += xTargetBox;
        }
        //droite
        else if (targetPosition.x > rb.position.x)  {
            targetPosition.x -= xTargetBox;
        }


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

    void Flip() 
    {
        isFacingRight = !isFacingRight;
        GFX.Rotate(0f, 180f, 0f);
    }

    /*    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Attack")
        {
            return;
        }
    }
    */

}

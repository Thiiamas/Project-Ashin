using System.Collections;
using UnityEngine;
using Pathfinding;

public class FlyingEye : Enemy
{
    Path path;
    Seeker seeker;

    Vector2 targetPosition;
    Vector2 direction = Vector2.zero;

    float nextWaypointDistance = 1f;
    int currentWaypoint = 0;
    float distanceToPlayer = 0f;

    const float FLEE_MARGIN = .5f;
    Timer dashTimer;



    [Header("Dash Attack")]
    [SerializeField] float dashAttackRange;
    [SerializeField] float dashAttackDamage;
    [SerializeField] float dashAttackCooldown;
    [SerializeField] float dashTime;
    [SerializeField] float dashForce;


    protected override void Setup()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        InvokeRepeating("UpdatePath", 0f, 0.5f);
        dashTimer = new Timer(dashTime);
    }

    void UpdatePath()
    {
        if (seeker.IsDone() && !isDead)
        {
            seeker.StartPath(this.transform.position, targetPosition, OnPathComplete);
        }
    }


    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;

            // starting at 0 makes direction.x > 0 for the first waypoint
            // which means the enemy will flip to the right when the path restart even if he wants to go left
            currentWaypoint = 1;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(isDead){
            ApplyGravity();
            characterController.move(velocity * Time.deltaTime);
            return;
        }

        if ( path == null || currentWaypoint >= path.vectorPath.Count || isStun || isKnockbacked || isAttacking) {
            return;
        }

        //targetPosition = playerTransform.position;
        targetPosition = (Vector2) playerTransform.position + GetPointOnUnitCircleCircumference() * (dashAttackRange - FLEE_MARGIN);

        distanceToPlayer = Vector2.Distance(playerTransform.position, transform.position);

        direction = ( (Vector2) path.vectorPath[currentWaypoint] - rb.position).normalized;   

        velocity.x = Mathf.MoveTowards(velocity.x, direction.x * speed, acceleration * Time.deltaTime);
        velocity.y = Mathf.MoveTowards(velocity.y, direction.y * speed, acceleration * Time.deltaTime);

        characterController.move(velocity * Time.deltaTime);

        // Attack
        if (distanceToPlayer <= dashAttackRange && (Time.time >= lastAttack + dashAttackCooldown || lastAttack == 0))
        {
            StartCoroutine(DashAttack());
        }


        // Advance in path
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance <= nextWaypointDistance)  {
            currentWaypoint++;
        }


        // Flip
        if ((velocity.x > 0 && !isFacingRight) || (velocity.x < 0 && isFacingRight)) {
            Flip();
        }     
    }


    public override void TakeDamage(Transform damageDealer, float damage)
    {
		if(isDead){
			DestroySelf();
		}

		if(isAttacking){
            StopCoroutine(DashAttack());
            StopAttacking();
		}

        health -= damage;

        DamagePopup.Create(transform.position + Vector3.one, damage);
        Instantiate(GameManager.Instance.HurtEffectPrefab, transform.position, Quaternion.identity);

        if (health <= 0) {
            isDead = true;
        }

		Vector3 direction = (transform.position - damageDealer.position).normalized;			
		StartCoroutine( KnockBack(direction, knockBackForce) );
    }


    Vector2 GetPointOnUnitCircleCircumference()
    {
        float randomAngle = Random.Range(-Mathf.PI/3, Mathf.PI/3);
        return new Vector2(Mathf.Sin(randomAngle), Mathf.Cos(randomAngle)).normalized;
    }


    IEnumerator DashAttack()
    {
        dashTimer.Start();
		isAttacking = true;
        Vector3 dashDirection = (playerTransform.position - transform.position).normalized;
        while (dashTimer.IsOn && isAttacking)
        {
            velocity = dashDirection * dashForce;
			characterController.move(velocity * Time.deltaTime);
            dashTimer.Decrease();
            yield return new WaitForEndOfFrame();
        }

        if(isAttacking){
            StopAttacking();
            velocity = Vector3.zero; 
        }
    }


}

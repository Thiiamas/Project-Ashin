using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoumsorAnimationController : AnimationController
{
    const string STATE_IDLE = "Roumsor_Idle";
    const string STATE_RUN = "Roumsor_Run";

    const string STATE_HURT = "Roumsor_Hurt";
    const string STATE_TIRED = "Roumsor_Tired";
    const string STATE_DEATH = "Roumsor_Death";

    const string STATE_ATTACK = "Roumsor_Attack";

    [SerializeField] Roumsor roumsor;

    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        speed.x = Mathf.Abs(roumsor.Velocity.x);

        if(roumsor.IsDead)
        {
            ChangeState(STATE_DEATH);
        }

        else if(roumsor.IsTired)
        {
            ChangeState(STATE_TIRED);
        }

        else if(roumsor.IsStun)
        {
            ChangeState(STATE_HURT);
        }


        else if(roumsor.IsAttacking)
        {
            ChangeState(STATE_ATTACK);
        }

        else if( roumsor.IsGrounded && speed.x < 0.1)
        {
            ChangeState(STATE_IDLE);
        }   

        else if( roumsor.IsGrounded && speed.x >= 0.1)
        {
            ChangeState(STATE_RUN);
        }

    }

    
    public void Attack()
    {
        roumsor.BasicAttack();
    }
    
    public void AttackAnimationExit()
    {
        Debug.Log("ff");
        roumsor.StopAttacking();
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEyeAnimationController : AnimationController
{
    const string STATE_IDLE = "FlyingEye_Flight";
    const string STATE_RUN = "FlyingEye_Run";

    const string STATE_HURT = "FlyingEye_Hurt";
    const string STATE_TIRED = "FlyingEye_Tired";
    const string STATE_DEATH = "FlyingEye_Death";

    const string STATE_ATTACK = "FlyingEye_Attack2";

    [SerializeField] FlyingEye flyingEye;

    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        speed.x = Mathf.Abs(flyingEye.Velocity.x);

        if(flyingEye.IsDead)
        {
            ChangeState(STATE_DEATH);
        }

        else if(flyingEye.IsStun)
        {
            ChangeState(STATE_HURT);
        }

        else if(flyingEye.IsAttacking)
        {
            ChangeState(STATE_ATTACK);
        }

        else
        {
            ChangeState(STATE_IDLE);
        }   

    }    

}

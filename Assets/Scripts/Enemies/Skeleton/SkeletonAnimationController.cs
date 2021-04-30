using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonAnimationController : AnimationController
{
    const string STATE_IDLE = "Skeleton_Idle";
    const string STATE_RUN = "Skeleton_Walk";

    const string STATE_HURT = "Skeleton_Hurt";
    const string STATE_DEATH = "Skeleton_Death";

    const string STATE_ATTACK = "Skeleton_Attack";
    const string STATE_SHIELD = "Skeleton_Shield";

    [SerializeField] Skeleton skeleton;

    // Update is called once per frame
    void Update()
    {
        speed.x = Mathf.Abs(skeleton.Velocity.x);

        if(skeleton.IsDead)
        {
            ChangeState(STATE_DEATH);
        }

        else if(skeleton.IsShielding)
        {
            ChangeState(STATE_SHIELD);
        }

        else if(skeleton.IsAttacking)
        {
            ChangeState(STATE_ATTACK);
        }

        else if( skeleton.IsGrounded && speed.x < 0.1)
        {
            ChangeState(STATE_IDLE);
        }   

        else if( skeleton.IsGrounded && speed.x >= 0.1)
        {
            ChangeState(STATE_RUN);
        }

    }

    public void Attack()
    {
        skeleton.BasicAttack();
    }

    public void AttackAnimationExit()
    {
        skeleton.StopAttacking();
    }
    
    public void ShieldAnimationExit()
    {
        skeleton.StopShielding();
    }   

}

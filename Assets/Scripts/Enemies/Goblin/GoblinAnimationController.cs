using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinAnimationController : AnimationController
{
    const string STATE_IDLE = "Goblin_Idle";
    const string STATE_RUN = "Goblin_Run";

    const string STATE_HURT = "Goblin_Hurt";
    const string STATE_DEATH = "Goblin_Death";

    const string STATE_ATTACK = "Goblin_Attack3";

    [SerializeField] Goblin goblin;

    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        speed.x = Mathf.Abs(goblin.Velocity.x);

        if(goblin.IsDead)
        {
            ChangeState(STATE_DEATH);
        }

        else if(goblin.IsStun)
        {
            ChangeState(STATE_HURT);
        }


        else if(goblin.IsAttacking)
        {
            ChangeState(STATE_ATTACK);
        }

        else if( goblin.IsGrounded && speed.x < 0.1)
        {
            ChangeState(STATE_IDLE);
        }   

        else if( goblin.IsGrounded && speed.x >= 0.1)
        {
            ChangeState(STATE_RUN);
        }

    }

    public void ThrowBomb()
    {
        goblin.BombAttack();
    }

    public void AttackAnimationExit()
    {
        goblin.StopAttacking();
    }

}

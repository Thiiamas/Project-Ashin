using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoumsorAnimationController : MonoBehaviour
{
    const string STATE_IDLE = "Roumsor_Idle";
    const string STATE_RUN = "Roumsor_Run";

    const string STATE_HURT = "Roumsor_Hurt";
    const string STATE_TIRED = "Roumsor_Tired";
    const string STATE_DEATH = "Roumsor_Death";

    const string STATE_ATTACK = "Roumsor_Attack";

    [SerializeField] Transform roumsorTransform;
    Animator animator;
    Roumsor roumsor;
    string currentState = "";
    Vector2 speed = new Vector2(0, 0);

    // Start is called before the first frame update
    void Start()
    {
        roumsor = roumsorTransform.GetComponent<Roumsor>();
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

    void ChangeState(string newState)
    {
        if(currentState == newState){
            return;
        }
        currentState = newState;
        animator.Play(newState);
    }
    
    public void StartAttack()
    {
        roumsor.BasicAttack();
    }

}

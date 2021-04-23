using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinAnimationController : MonoBehaviour
{
    const string STATE_IDLE = "Goblin_Idle";
    const string STATE_RUN = "Goblin_Run";

    const string STATE_HURT = "Goblin_Hurt";
    const string STATE_DEATH = "Goblin_Death";

    const string STATE_ATTACK = "Goblin_Attack3";

    [SerializeField] Transform goblinTransform;
    Animator animator;
    GoblinController goblinController;
    string currentState = "";
    Vector2 speed = new Vector2(0, 0);

    // Start is called before the first frame update
    void Start()
    {
        goblinController = goblinTransform.GetComponent<GoblinController>();
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        speed.x = Mathf.Abs(goblinController.Velocity.x);

        if(goblinController.IsDead)
        {
            ChangeState(STATE_DEATH);
        }

        else if(goblinController.IsStun)
        {
            ChangeState(STATE_HURT);
        }


        else if(goblinController.IsAttacking)
        {
            ChangeState(STATE_ATTACK);
        }

        else if( goblinController.IsGrounded && speed.x < 0.1)
        {
            ChangeState(STATE_IDLE);
        }   

        else if( goblinController.IsGrounded && speed.x >= 0.1)
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
    
    public void ThrowBomb()
    {
        goblinController.BombAttack();
    }

}

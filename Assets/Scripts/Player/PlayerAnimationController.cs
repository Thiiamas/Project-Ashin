using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    
    const string STATE_IDLE = "Player_Idle_1";
    const string STATE_RUN = "Player_Run";

    const string STATE_FALL = "Player_Fall";
    const string STATE_JUMP = "Player_Jump";

    const string STATE_DIE = "Player_Die";
    const string STATE_DASH = "Player_Dash";
    const string STATE_WALLSLIDE = "Player_WallSlide";

    const string STATE_ATTACK = "Player_Attack_2";
    const string STATE_ATTACK_UP = "Player_AttackUp";
    const string STATE_ATTACK_DOWN = "Player_AttackDown";


    [SerializeField] Transform player;
    Animator animator;
    PlayerController playerController;
    PlayerMovement playerMovement;
    PlayerAttack playerAttack;
    string currentState = "";
    float xSpeed = 0f;
    float ySpeed = 0f;

    // Start is called before the first frame update
    void Start()
    {
        playerController = player.GetComponent<PlayerController>();
        playerMovement = player.GetComponent<PlayerMovement>();
        playerAttack = player.GetComponent<PlayerAttack>();
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        xSpeed = Mathf.Abs(playerMovement.Velocity.x);
        ySpeed = playerMovement.Velocity.y;

        if(playerController.IsDead)
        {
            ChangeState(STATE_DIE);
        }

        else if(playerMovement.IsDashing)
        {
            ChangeState(STATE_DASH);
        }

        else if(playerMovement.IsWallSliding)
        {
            ChangeState(STATE_WALLSLIDE);
        }

        else if(playerAttack.IsAttacking)
        {
            if(currentState != STATE_ATTACK && currentState != STATE_ATTACK_DOWN && currentState != STATE_ATTACK_UP)
            {
                if (playerMovement.DirectionInput.y > 0)  {
                    ChangeState(STATE_ATTACK_UP);
                }
                else if (playerMovement.DirectionInput.y < 0) {
                    ChangeState(STATE_ATTACK_DOWN);
                }
                else {
                    ChangeState(STATE_ATTACK);
                }
            }

        }

        else if( !playerMovement.IsGrounded && ySpeed >= 0.1)
        {
            ChangeState(STATE_JUMP);
        }   
             
        else if( !playerMovement.IsGrounded && ySpeed < 0.1)
        {
            ChangeState(STATE_FALL);
        }

        else if( playerMovement.IsGrounded && xSpeed < 0.1)
        {
            ChangeState(STATE_IDLE);
        }   

        else if( playerMovement.IsGrounded && xSpeed >= 0.1)
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
    
    public void FinishAttackAnimation()
    {
        playerAttack.FinishAttack();
    }

    public void FinishDeathAnimation()
    {
        playerAttack.enabled = false;
        playerMovement.enabled = false;
        playerController.enabled = false;
        this.enabled = false;
    }


}

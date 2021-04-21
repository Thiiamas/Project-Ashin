using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpBlueKnight : StateMachineBehaviour
{
    BlueKnight blueKnight;
    CharacterController2D characterController;
    float jumpForce = 8f;
    float timer = 0f;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        blueKnight = animator.GetComponentInParent<BlueKnight>();
        characterController = animator.GetComponentInParent<CharacterController2D>();
        blueKnight.Velocity = Vector2.zero;
        Vector3 direction = new Vector3(0, 1f, 0);
        Vector2 force = new Vector2(10, 10);
        Vector2 velocity = blueKnight.Velocity;
        velocity.y = Mathf.Sqrt(2 * jumpForce * Mathf.Abs(Physics2D.gravity.y));
        characterController.move(velocity * Time.deltaTime);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime < 0.5f)
        {
            Vector2 velocity = blueKnight.Velocity;
            velocity.y = Mathf.Sqrt(2 * jumpForce * Mathf.Abs(Physics2D.gravity.y));
            characterController.move(velocity * Time.deltaTime);
        }
        else
        {
            animator.SetTrigger("jumpEnded");
        }
        
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}

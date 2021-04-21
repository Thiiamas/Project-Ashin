using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashBoss : StateMachineBehaviour
{
    BlueKnight blueKnight;
    CharacterController2D characterController;
    Transform playerTransform;
    Timer dashTimer;
    Vector3 targetPos;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        blueKnight = animator.GetComponentInParent<BlueKnight>();
        characterController = animator.GetComponentInParent<CharacterController2D>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        targetPos = playerTransform.position;
        blueKnight.StartDive(targetPos);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (blueKnight.IsGrounded)
        {
            blueKnight.isDashing = false;
            animator.SetTrigger("dashEnded");
            return;
        }
        float distance = (playerTransform.position - blueKnight.transform.position).magnitude;

/*        if (!blueKnight.IsDashing && !blueKnight.IsGrounded && blueKnight.isEnraged)
        {
            blueKnight.StartDash();
        }*/

        if (!blueKnight.IsDashing || distance < 1)
        {
            animator.SetTrigger("dashEnded");
            return;
        }
        characterController.move(blueKnight.Velocity * Time.deltaTime);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakedecisionBlueKnight : StateMachineBehaviour
{
    BlueKnight blueKnight;
    CharacterController2D charactercontroller;
    Transform playerTransform;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        blueKnight = animator.GetComponentInParent<BlueKnight>();
        charactercontroller = animator.GetComponentInParent<CharacterController2D>();
        playerTransform = blueKnight.PlayerTransform;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        string decision = blueKnight.TakeDecision();

        if (Vector3.Distance(playerTransform.position, blueKnight.transform.position) > blueKnight.basicAttackRange)
        {
            if (decision == "")
            {
                blueKnight.RunTowardsPlayer(false);
            }
        }
        else
        {
            // if no decision && inrange for attack, well, attack ffs
/*            animator.Play("Swing1Boss");
*/        }

    }

    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called before OnStateMove is called on any state inside this state machine
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateIK is called before OnStateIK is called on any state inside this state machine
    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    // OnStateMachineEnter is called when entering a state machine via its Entry Node
    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        
    }

    // OnStateMachineExit is called when exiting a state machine via its Exit Node
    //override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    //{
    //    
    //}
}

using System;
using UnityEngine;

public class AnimationEvent : StateMachineBehaviour
{
    [SerializeField] string stateEnterFunctionName ;
    [SerializeField] string stateExitFunctionName ;

    AnimationController animationController;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(stateEnterFunctionName != "") {
            animationController = animator.GetComponent<AnimationController>();
            animationController.SendMessage(stateEnterFunctionName);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(stateExitFunctionName != "") {
            animationController = animator.GetComponent<AnimationController>();
            animationController.SendMessage(stateExitFunctionName);
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }
}
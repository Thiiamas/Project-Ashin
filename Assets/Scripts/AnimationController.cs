using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    protected Animator animator;
    protected string currentState = "";
    protected Vector2 speed = new Vector2(0, 0);

    void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    protected void ChangeState(string newState)
    {
        if(currentState == newState){
            return;
        }
        currentState = newState;
        animator.Play(newState);
    }


}

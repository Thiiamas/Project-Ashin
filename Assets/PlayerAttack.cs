using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack: MonoBehaviour
{
    public Animator animator;

    float iAttackSpeed = 2.0f;
    float attackSpeed;
    float lastAttack = 0.0f;
    private void Start()
    {
        attackSpeed = iAttackSpeed;
        animator.SetFloat("attackSpeedMulti", attackSpeed/2.0f);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && (Time.time >= lastAttack + (1/attackSpeed)))
        {
            basicAttack();
            lastAttack = Time.time;
        }

        //Force the animation to transition after 1 loop for attack air
        if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("AttackAir"))
        {
            animator.SetBool("isAttacking", true);
            if (this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > this.animator.GetCurrentAnimatorStateInfo(0).length)
            {
                animator.SetBool("isAttacking", false);
            }
        }

    }

    void basicAttack()
    {
        animator.SetTrigger("Attack");
        attackSpeed = attackSpeed +0.5f;
      /*  if (attackSpeed < 15f)
        {
            increaseAttackSpeed(0.5f);
        }*/
    }

    void increaseAttackSpeed(float value)
    {
        attackSpeed = attackSpeed + 0.5f;
        animator.SetFloat("attackSpeedMulti", attackSpeed / iAttackSpeed);
    }
}

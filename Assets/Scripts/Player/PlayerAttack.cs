using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack: MonoBehaviour
{
    private Animator animator;

    [SerializeField] PolygonCollider2D attackCollider;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] int basicAttackDamage = 10;
    [SerializeField] float attackSpeed = 2.0f;
    private float nextAttackTime = 0.0f;

    private void Start()
    {
        animator = this.GetComponent<Animator>();
        animator.SetFloat("attackSpeed", attackSpeed);
    }
    
    // Update is called once per frame
    void Update()
    {
        //Force the animation to transition after 1 loop for attack air
        
        /*if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("AttackAir"))
        {
            animator.SetBool("isAttacking", true);
            if (this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > this.animator.GetCurrentAnimatorStateInfo(0).length)
            {
                animator.SetBool("isAttacking", false);
            }
        }*/
    }

    public void BasicAttack()
    {
        if(Time.time >= nextAttackTime)
        {
            animator.SetTrigger("Attack");  
                     
            List<Collider2D> hitEnemies = new List<Collider2D>();
            ContactFilter2D filter = new ContactFilter2D();
            filter.SetLayerMask(enemyLayer);
            Physics2D.OverlapCollider(attackCollider, filter, hitEnemies);

            foreach(Collider2D enemy in hitEnemies)
            {
                //enemy.GetComponentInParent<Boss>().TakeDamage(basicAttackDamage);
            }

            nextAttackTime = Time.time + 1/attackSpeed;
        }
    }

}

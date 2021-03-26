using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack: MonoBehaviour
{
    private Animator animator;
    private CharacterController2D characterController;

    [SerializeField] PolygonCollider2D attackCollider;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] int basicAttackDamage = 10;
    [SerializeField] float attackSpeedMultiplier = 1f;
    private bool isAttacking = false;
    public bool IsAttacking {
        get => isAttacking;
    }

    private void Start()
    {
        animator = this.GetComponent<Animator>();
        characterController = this.GetComponent<CharacterController2D>();
        animator.SetFloat("attackSpeedMultiplier", attackSpeedMultiplier);
    }
    
    // Update is called once per frame
    void Update()
    {
    }

    public void BasicAttack(InputAction.CallbackContext context)
    {
        if(context.performed && !isAttacking)
        {
            isAttacking = true;
            animator.SetTrigger("attackTrigger");
            animator.SetBool("isAttacking", isAttacking);

            List<Collider2D> hitEnemies = new List<Collider2D>();
            ContactFilter2D filter = new ContactFilter2D();
            filter.SetLayerMask(enemyLayer);
            Physics2D.OverlapCollider(attackCollider, filter, hitEnemies);

            foreach(Collider2D enemy in hitEnemies)
            {
                enemy.GetComponentInParent<Enemy>().takeDamage(basicAttackDamage);
            }
        }
    }

    public void FinishAttack()
    {
        isAttacking = false;
        animator.SetBool("isAttacking", isAttacking);
    }

}

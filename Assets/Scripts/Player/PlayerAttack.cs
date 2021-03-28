using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack: MonoBehaviour
{
    Animator animator;
    CharacterController2D characterController;
    PlayerController playerController;
    
    bool isAttacking = false;
    public bool IsAttacking { get { return isAttacking; } }

    [SerializeField] PolygonCollider2D attackCollider;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] int basicAttackDamage = 10;
    [SerializeField] float attackSpeedMultiplier = 1f;

    private void Start()
    {
        animator = this.GetComponent<Animator>();
        characterController = this.GetComponent<CharacterController2D>();
        playerController = this.GetComponent<PlayerController>();

        animator.SetFloat("attackSpeedMultiplier", attackSpeedMultiplier);
    }
    
    public void AttackInput(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            StartCoroutine( playerController.InputBuffer(() => playerController.CanAttack(), Attack) );
        }
    }

    void Attack()
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

    public void FinishAttack()
    {
        isAttacking = false;
        animator.SetBool("isAttacking", isAttacking);
    }

}

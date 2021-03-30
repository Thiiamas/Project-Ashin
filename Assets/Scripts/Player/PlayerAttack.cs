using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Experimental.Rendering.Universal;

public class PlayerAttack: MonoBehaviour
{
    Animator animator;
    CharacterController2D characterController;
    PlayerController playerController;
    public GameObject attackLight;
    public Transform attackPoint;
    bool isAttacking = false;
    public bool IsAttacking { get { return isAttacking; } }

    [SerializeField] PolygonCollider2D attackCollider;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] int basicAttackDamage = 10;
    [SerializeField] float attackSpeedMultiplier = 1f;
    [SerializeField] Transform damagePopupPrefab;

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

        //create light
        /*        GameObject light = Instantiate(attackLight, attackPoint.position, Quaternion.identity);
                light.transform.parent = attackPoint.transform;
                light.transform.position = Vector3.zero;
                GameObject light = Instantiate(attackLight, attackPoint);*/

        List<Collider2D> hitEnemies = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(enemyLayer);
        Physics2D.OverlapCollider(attackCollider, filter, hitEnemies);

        foreach(Collider2D enemy in hitEnemies)
        {
            spawnDamagePopup(enemy.transform.position + Vector3.one, basicAttackDamage);
            enemy.GetComponentInParent<Enemy>().takeDamage(basicAttackDamage);
        }
	}

    public void FinishAttack()
    {
        isAttacking = false;
        animator.SetBool("isAttacking", isAttacking);
    }

    void spawnDamagePopup(Vector3 pos, int damage)
    {
        Transform damagePopupTransform = Instantiate(damagePopupPrefab, pos, Quaternion.identity);
        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(damage);
    }

}

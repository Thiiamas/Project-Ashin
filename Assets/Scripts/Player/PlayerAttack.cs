using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack: MonoBehaviour
{
    Animator animator;
    CharacterController2D characterController;
    PlayerController playerController;
    PlayerMovement playerMovement;
    bool isAttacking = false;
    public bool IsAttacking { get { return isAttacking; } }


    [Header("Game Object")]
    [SerializeField] GameObject attackLight;
    [SerializeField] GameObject attackLightUp;
    [SerializeField] GameObject attackLightDown;
    [SerializeField] Transform attackPoint;
    [SerializeField] Transform attackPointUp;
    [SerializeField] Transform attackPointDown;



    PolygonCollider2D attackCollider;
    PolygonCollider2D attackColliderUp;
    PolygonCollider2D attackColliderDown;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] int basicAttackDamage = 10;
    [SerializeField] float attackSpeedMultiplier = 1f;
    [SerializeField] Transform damagePopupPrefab;

    private void Start()
    {
        characterController = this.GetComponent<CharacterController2D>();
        playerController = this.GetComponent<PlayerController>();
        playerMovement = this.GetComponent<PlayerMovement>();

        attackCollider = attackPoint.GetComponent<PolygonCollider2D>();
        attackColliderUp = attackPointUp.GetComponent<PolygonCollider2D>();
        attackColliderDown = attackPointDown.GetComponent<PolygonCollider2D>();

        animator = playerController.GFX.GetComponent<Animator>();
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
        PolygonCollider2D collider;
        if (playerMovement.directionInput.y > 0)
        {
            Instantiate(attackLightUp, attackPointUp.position, transform.rotation);
            collider = attackColliderUp;
        } else if (playerMovement.directionInput.y < 0)
        {
            collider = attackColliderDown;
            Instantiate(attackLightDown, attackPointDown.position, transform.rotation);
        }
        else
        {
            //create light
            Instantiate(attackLight, attackPoint.position, transform.rotation);
            collider = attackCollider;
        }
        List<Collider2D> hitEnemies = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(enemyLayer);
        Physics2D.OverlapCollider(collider, filter, hitEnemies);

        foreach(Collider2D enemy in hitEnemies)
        {
            playerController.aCanJump = true;
            spawnDamagePopup(enemy.transform.position + Vector3.one, basicAttackDamage);
            if (enemy.GetComponent<Enemy>() != null)
            {
                enemy.GetComponentInParent<Enemy>().takeDamage(basicAttackDamage);
            }
            if (!playerMovement.IsGrounded && playerMovement.directionInput.y < 0)
            {
                Vector3 pDirection = (transform.position - enemy.transform.position).normalized;
                playerMovement.KnockBackTowards(pDirection, new Vector2(0,5));
            }
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

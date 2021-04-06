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


    [Header("Stats")]
    [SerializeField] int basicAttackDamage = 10;
    [SerializeField] float attackSpeedMultiplier = 1f;
    [SerializeField] LayerMask enemyLayer;


    [Header("Game Object")]
    [SerializeField] GameObject attackLight;
    [SerializeField] Transform attackPoint;
    [SerializeField] Transform attackPointUp;
    [SerializeField] Transform attackPointDown;



    private void Start()
    {
        characterController = this.GetComponent<CharacterController2D>();
        playerController = this.GetComponent<PlayerController>();
        playerMovement = this.GetComponent<PlayerMovement>();

        animator = playerController.GFX.GetComponent<Animator>();
        //animator.SetFloat("attackSpeedMultiplier", attackSpeedMultiplier);
    }
    
    public void AttackInput(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            StartCoroutine( playerController.InputBuffer(() => playerController.CanAttack(), Attack) );
        }
    }

    void setInactiveAttackPoint()
    {
        attackPoint.gameObject.SetActive(false);
    }
    void setInactiveAttackPointUp()
    {
        attackPointUp.gameObject.SetActive(false);
    }
    void setInactiveAttackPointDown()
    {
        attackPoint.gameObject.SetActive(false);
    }

    void Attack()
	{
        isAttacking = true;

        GameObject light;
        
        // Up
        if (playerMovement.directionInput.y > 0)
        {
            attackPointUp.gameObject.SetActive(true);
            Invoke("setInactiveAttackPointUp", 0.5f);
            light = Instantiate(attackLight, attackPointUp);
            light.transform.localRotation *= Quaternion.Euler(0, 0, 90);
        }

        // Down
        else if (playerMovement.directionInput.y < 0)
        {
            attackPointDown.gameObject.SetActive(true);
            Invoke("setInactiveAttackPointDown", 0.5f);
            light = Instantiate(attackLight, attackPointDown);
            light.transform.localRotation *= Quaternion.Euler(0, 0, -90);
        }

        // Normal
        else
        {
            attackPoint.gameObject.SetActive(true);
            Invoke("setInactiveAttackPoint", 0.5f);
            light = Instantiate(attackLight, attackPoint);

        }
        PolygonCollider2D attackCollider = light.GetComponent<PolygonCollider2D>();

        List<Collider2D> hitEnemies = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(enemyLayer);
        Physics2D.OverlapCollider(attackCollider, filter, hitEnemies);
        foreach (Collider2D enemy in hitEnemies)
        {
            playerController.CanJumpAfterAttack = true;
            if (enemy.GetComponent<Enemy>() != null)
            {
                enemy.GetComponentInParent<Enemy>().TakeDamage(this.transform, basicAttackDamage);
            }
            if (!playerMovement.IsGrounded && playerMovement.directionInput.y < 0)
            {
                Vector3 pDirection = (transform.position - enemy.transform.position).normalized;
                playerMovement.KnockBackwithForce(pDirection, new Vector2(0, 5));
            }
        }
        
    }


    public void FinishAttack()
    {
        isAttacking = false;
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack: MonoBehaviour
{
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
    }
    

    // Update is called once per frame
    void Update()
    {
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

        PolygonCollider2D attackCollider = InstantiateLight(playerMovement.DirectionInput.y);
        
        List<Collider2D> hitEnemies = GetCollidersInCollider(attackCollider, enemyLayer);
        foreach (Collider2D enemy in hitEnemies)
        {
            playerController.CanJumpAfterAttack = true;
            if (enemy.GetComponent<Enemy>() != null)
            {
                enemy.GetComponentInParent<Enemy>().TakeDamage(this.transform, basicAttackDamage);
            }
            if (!playerMovement.IsGrounded && playerMovement.DirectionInput.y < 0)
            {
                Vector3 pDirection = (transform.position - enemy.transform.position).normalized;
                playerMovement.KnockBackwithForce(pDirection, new Vector2(0, 5));
            }
        }   
    }


    PolygonCollider2D InstantiateLight(float yInput)
    {
        GameObject light;

        // Up
        if (yInput > 0)
        {
            light = Instantiate(attackLight, attackPointUp);
            light.transform.localRotation *= Quaternion.Euler(0, 0, 90);
        }

        // Down
        else if (yInput < 0)
        {
            light = Instantiate(attackLight, attackPointDown);
            light.transform.localRotation *= Quaternion.Euler(0, 0, -90);
        }

        // Normal
        else
        {
            light = Instantiate(attackLight, attackPoint);
        }
        
        return light.GetComponent<PolygonCollider2D>();
    }


    List<Collider2D> GetCollidersInCollider(Collider2D collider, LayerMask layer)
    {
        List<Collider2D> hits = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(layer);
        Physics2D.OverlapCollider(collider, filter, hits);
        return hits;
    }


    public void FinishAttack()
    {
        isAttacking = false;
    }


}

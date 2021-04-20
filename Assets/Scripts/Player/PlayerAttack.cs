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


    [Header("Stats")]
    [SerializeField] float attackSpeedMultiplier = 1f;
    [SerializeField] LayerMask enemyLayer;


    [Header("Game Object")]
    [SerializeField] GameObject basicAttackPrefab;
    [SerializeField] Transform attackPoint;



    #region getters

    public bool IsAttacking { get { return isAttacking; } }

    #endregion
    


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
        InstantiateAttack(playerMovement.DirectionInput.y, basicAttackPrefab);
    }

    public void HasAttackEnnemy(Enemy enemy)
    {   
        playerController.CanJumpAfterAttack = true;
   		VirtualCameraManager.Instance.ShakeCamera(3, 0.7f);
        if (!playerMovement.IsGrounded && playerMovement.DirectionInput.y < 0)
        {
            Vector3 pDirection = (transform.position - enemy.transform.position).normalized;
            StartCoroutine(playerMovement.KnockBack(pDirection, new Vector2(0, 10)));
        }
    }


    void InstantiateAttack(float yInput, GameObject attackPrefab)
    {
        GameObject light = Instantiate(attackPrefab, attackPoint);
        float rotationValue = playerMovement.IsFacingRight ? 90f : -90f;

        // Up
        if (yInput > 0)
        {
            light.transform.RotateAround(this.transform.position, Vector3.forward, rotationValue);    
        }

        // Down
        else if (yInput < 0)
        {
            light.transform.RotateAround(this.transform.position, Vector3.forward, -rotationValue);
        }
   
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

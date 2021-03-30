using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player scripts")]
    CharacterController2D characterController;
    PlayerAttack playerAttack;
    PlayerMovement playerMovement;


    [Header("Stats Settings")]
    [SerializeField] BarController healthBar;
    [SerializeField] BarController manaBar;
    [SerializeField] float maxHealth = 100;
    [SerializeField] float maxMana = 100;
    float health = 0f;
    float mana = 0f;
    bool canJump, CanWallJump, collidingWithWall;

    [Header("Stats Settings")]
    [SerializeField] float bufferTime = .2f;
    Timer bufferTimer;


    void Awake()
    {
    }
    
    void Start()
    {
        characterController = this.GetComponent<CharacterController2D>();
        playerAttack = this.GetComponent<PlayerAttack>();
        playerMovement = this.GetComponent<PlayerMovement>();

        health = maxHealth;
        healthBar.SetMaxValue(maxHealth);
        healthBar.SetValue(maxHealth);

        mana = maxMana;
        manaBar.SetMaxValue(maxMana);
        manaBar.SetValue(maxMana);

        bufferTimer = new Timer(bufferTime);
    }

    // Update is called once per frame
    void Update() {}

    void Die()
    {
        Destroy(this.gameObject);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0) {
            Die();
        }
        else {
            healthBar.SetValue(health);
        }
    }

    public IEnumerator InputBuffer ( Func<bool> conditionFunction, Action actionFunction ) 
    {
        // stop every other instance of input buffer (not the current one)
        StopCoroutine( "InputBuffer" );

        bufferTimer.Start();
        while ( bufferTimer.IsOn ) 
        {
            bufferTimer.Decrease();
            bool condition = conditionFunction();
            if ( condition ) {
                bufferTimer.Stop();
                actionFunction?.Invoke();
                yield break;
            } else {
                yield return new WaitForEndOfFrame();
            }
        }
    }

    public bool CanJump()
    {
        canJump = (playerMovement.IsGrounded || playerMovement.IsCoyoteTimerOn) && !playerMovement.IsJumping && !playerAttack.IsAttacking && !playerMovement.IsDashing;
        CanWallJump = playerMovement.IsWallSliding && !playerAttack.IsAttacking && !playerMovement.IsDashing;
        return  canJump || CanWallJump;
    }

    public bool CanAttack()
    {
        return !playerAttack.IsAttacking && !playerMovement.IsDashing && !playerMovement.IsWallSliding;
    }

    public bool CanDash()
    {
        return !playerAttack.IsAttacking && !playerMovement.IsDashing && playerMovement.DashHasReset && playerMovement.DashHasCooldown;
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Header("Scripts")]
    CharacterController2D characterController;
    PlayerAttack playerAttack;
    PlayerMovement playerMovement;
    SpriteRenderer spriteRenderer;

    bool canJump, CanWallJump, collidingWithWall;
    public bool aCanJump = true;
    
    [Header("Game Object")]
    [SerializeField] public GameObject GFX;
    [SerializeField] BarController healthBar;
    [SerializeField] BarController manaBar;


    [Header("Stats")]
    [SerializeField] float maxHealth = 100;
    [SerializeField] float maxMana = 100;
    float health = 0f;
    float mana = 0f;


    [Header("Invincibility")]
    [SerializeField] float invincibilityTime = 1.5f;
    [SerializeField] float invincibilityDeltaTime = 0.15f;
    bool isInvincible = false;


    [Header("Camera Shake")]
    [SerializeField] float shakeTime = 1f;
    [SerializeField] float shakeIntensity = 1f;


    [Header("Buffer")]
    [SerializeField] float bufferTime = .2f;
    Timer bufferTimer;


    
    void Start()
    {
        characterController = this.GetComponent<CharacterController2D>();
        playerAttack = this.GetComponent<PlayerAttack>();
        playerMovement = this.GetComponent<PlayerMovement>();
        spriteRenderer = GFX.GetComponent<SpriteRenderer>();

        health = maxHealth;
        healthBar.SetMaxValue(maxHealth);
        healthBar.SetValue(maxHealth);

        mana = maxMana;
        manaBar.SetMaxValue(maxMana);
        manaBar.SetValue(maxMana);

        bufferTimer = new Timer(bufferTime);
    }

    #region damage

    void Die()
    {
        Destroy(this.gameObject);
    }

    public void TakeDamage(Transform damageDealer, float damage)
    {
        if(isInvincible) { 
            return;
        }

        health -= damage;
        Instantiate(GameManager.Instance.HurtEffectPrefab, transform.position, Quaternion.identity);

        if (health <= 0) {
            Die();
        }
        else {
            playerMovement.KnockBack(damageDealer);
            TimeManager.SlowMotion(0.01f);
            Invoke("ResetTimeAfterDamage", .005f);
        }
    }

    private void ResetTimeAfterDamage()
    {
        TimeManager.RestoreTime();
        healthBar.SetValue(health);
        VirtualCameraManager.Instance.ShakeCamera(shakeIntensity, shakeTime);
        spriteRenderer.material = GameManager.Instance.WhiteMaterial; 
        Invoke("ResetMaterial", 0.2f);
        StartCoroutine(BecomeTemporarilyInvincible());
    }

    private IEnumerator BecomeTemporarilyInvincible()
    {
        isInvincible = true;
        Physics2D.IgnoreLayerCollision(3,7, true);

        for (float i = 0; i < invincibilityTime; i += invincibilityDeltaTime)
        {
            // Alternate between 0 and 1 scale to simulate flashing
            if (GFX.transform.localScale == Vector3.one) {
                GFX.transform.localScale = Vector3.zero;
            } 
            else {
                GFX.transform.localScale = Vector3.one;
            }
            yield return new WaitForSeconds(invincibilityDeltaTime);
        }

        GFX.transform.localScale = Vector3.one;
        isInvincible = false;
        Physics2D.IgnoreLayerCollision(3,7, false);
    }

    private void ResetMaterial()
    {
        spriteRenderer.material = GameManager.Instance.DefaultMaterial;
    }

    #endregion


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

    //WIP add "aCanJump" attribute to handle jump reset after succefull attack
    public bool CanJumpTest()
    {
        canJump = (playerMovement.IsGrounded || playerMovement.IsCoyoteTimerOn) && !playerMovement.IsJumping && !playerAttack.IsAttacking && !playerMovement.IsDashing;
        CanWallJump = playerMovement.IsWallSliding && !playerAttack.IsAttacking && !playerMovement.IsDashing;
        return canJump || CanWallJump || aCanJump;
    }

    public bool CanAttack()
    {
        return !playerAttack.IsAttacking && !playerMovement.IsDashing && !playerMovement.IsWallSliding;
    }

    public bool CanDash()
    {
        return !playerAttack.IsAttacking && !playerMovement.IsDashing && playerMovement.DashHasReset && playerMovement.DashHasCooldown;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag == "Enemy")
        {
            TakeDamage(col.transform, 10f);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    CharacterController2D characterController;
    PlayerAttack playerAttack;
    PlayerMovement playerMovement;
    SpriteRenderer spriteRenderer;

    bool canJump, CanWallJump, collidingWithWall;
    bool isDead;

    [NonSerialized] public bool CanJumpAfterAttack = true;
    
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


    [Header("Slow motion")]
    [SerializeField] float slowMotionFactor = 0.01f;
    [SerializeField] float slowMotionTime = 1f;


    [Header("Camera Shake")]
    [SerializeField] float shakeTime = 1f;
    [SerializeField] float shakeIntensity = 1f;


    [Header("Buffer")]
    [SerializeField] float bufferTime = .2f;
    Timer bufferTimer;

    #region getters

    public bool IsDead { get { return isDead; } }

    #endregion

    
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

    public void Die()
    {
        playerAttack.enabled = false;
        playerMovement.enabled = false;
        this.enabled = false;
    }

    public void TakeDamage(Transform damageDealer, float damage)
    {
        if(isInvincible) { 
            return;
        }

        health -= damage;
        healthBar.SetValue(health);

        // hurt prefab
        Instantiate(GameManager.Instance.HurtEffectPrefab, transform.position, Quaternion.identity);

        // knockback
        Vector3 direction = (transform.position - damageDealer.position).normalized;
        StartCoroutine(playerMovement.KnockBack(direction));
        
        StartCoroutine(DamageCoroutine());
    }

    public IEnumerator DamageCoroutine()
    {
        spriteRenderer.material = GameManager.Instance.WhiteMaterial; 
        TimeManager.SlowMotion(slowMotionFactor);

        yield return new WaitForSecondsRealtime(slowMotionTime);
        
        TimeManager.RestoreTime();
        spriteRenderer.material = GameManager.Instance.DefaultMaterial;

        // shake camera
        VirtualCameraManager.Instance.ShakeCamera(shakeIntensity, shakeTime);

        if (health <= 0) {
            isDead = true;
        } else {
            StartCoroutine(BecomeTemporarilyInvincible());
        }
    }

    private IEnumerator BecomeTemporarilyInvincible()
    {
        isInvincible = true;
        //Physics2D.IgnoreLayerCollision(3,7, true);
        //gameObject.GetComponent<BoxCollider2D>().enabled = false;

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
        return canJump || CanWallJump || CanJumpAfterAttack;
    }

    public bool CanAttack()
    {
        return !playerAttack.IsAttacking && !playerMovement.IsDashing && !playerMovement.IsWallSliding;
    }

    public bool CanDash()
    {
        //return !playerAttack.IsAttacking && 
        return !playerMovement.IsDashing && playerMovement.DashHasReset && playerMovement.DashHasCooldown;
    }

    
    /*void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag == "Enemy")
        {
            Enemy enemy = col.gameObject.GetComponent<Enemy>();
            TakeDamage(col.transform, enemy.Damage);
        }
    }*/


    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Enemy" )
        {
            Enemy enemy = col.gameObject.GetComponent<Enemy>();
            TakeDamage(col.transform, enemy.Damage);
        } 
    }

}

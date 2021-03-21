using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Header("Stats Settings")]
    [SerializeField] BarController healthBar;
    [SerializeField] BarController manaBar;
    [SerializeField] float maxHealth = 100;
    [SerializeField] float maxMana = 100;

    private float health;
    private float mana;

    void Awake()
    {
    }
    
    void Start()
    {
        health = maxHealth;
        healthBar.SetMaxValue(maxHealth);
        healthBar.SetValue(maxHealth);

        mana = maxMana;
        manaBar.SetMaxValue(maxMana);
        manaBar.SetValue(maxMana);
    }

    // Update is called once per frame
    void Update()
    {
    }

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
            healthBar.SetValue(maxHealth);
        }
    }
}

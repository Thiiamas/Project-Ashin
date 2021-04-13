using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class BasicAttackController : MonoBehaviour
{
    Light2D attackLight;
    PlayerAttack playerAttack;

    [SerializeField] const float DISAPPEAR_TIMER_MAX = 0.4f;
    [SerializeField] float damage = 10f;

    float disappearTimer;


    // Start is called before the first frame update
    void Start()
    {
        disappearTimer = DISAPPEAR_TIMER_MAX;
        attackLight = this.GetComponent<Light2D>();
        
        playerAttack = GameManager.Instance.PlayerTransform.GetComponent<PlayerAttack>();
    }

   
    // Update is called once per frame
    void Update()
    {
        if (disappearTimer > DISAPPEAR_TIMER_MAX * 0.5 && attackLight.intensity < 2f)
        {
            float increaseScaleAmount = 0.05f;
            attackLight.intensity += increaseScaleAmount;

        } 
        else
        {
            float decreaseScaleAmount = 0.05f;
            attackLight.intensity -= decreaseScaleAmount;
        }

        disappearTimer -= Time.deltaTime;

        if (disappearTimer < 0.1)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Enemy")
        {
            Enemy enemy = col.GetComponent<Enemy>();
            enemy.TakeDamage(playerAttack.transform, damage);
            playerAttack.HasAttackEnnemy(enemy);
        }
    }
    
}

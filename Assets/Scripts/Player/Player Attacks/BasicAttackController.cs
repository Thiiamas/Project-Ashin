using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class BasicAttackController : MonoBehaviour
{
    Light2D attackLight;
    PlayerAttack playerAttack;

    [SerializeField] float damage = 10f;

    float disappearTimer;
    const float INTENSITY_SCALE = 0.05f;
    const float DISAPPEAR_TIMER_MAX = 0.4f;


    // Start is called before the first frame update
    void Start()
    {
        disappearTimer = DISAPPEAR_TIMER_MAX;
        attackLight = this.GetComponent<Light2D>();
        attackLight.intensity = 0;
        playerAttack = GameManager.Instance.PlayerTransform.GetComponent<PlayerAttack>();
    }

   
    // Update is called once per frame
    void Update()
    {
        if (disappearTimer > DISAPPEAR_TIMER_MAX * 0.5 && attackLight.intensity < 1f)
        {
            attackLight.intensity += INTENSITY_SCALE;
        } 
        else if( attackLight.intensity > 0f )
        {
            attackLight.intensity -= INTENSITY_SCALE;
        }

        disappearTimer -= Time.deltaTime;

        if (disappearTimer < 0)
        {
            Destroy(gameObject);
        }
    }


    void OnTriggerEnter2D(Collider2D col)
    {
        // since enemy has 2 colliders we check if its the box collider
        if(col.gameObject.tag == "Enemy" && col.GetType() == typeof(BoxCollider2D))
        {
            Enemy enemy = col.GetComponent<Enemy>();
            enemy.TakeDamage(playerAttack.transform, damage);
            playerAttack.HasAttackEnnemy(enemy);
        }
    }
    
}

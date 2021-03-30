using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Attack1Light : MonoBehaviour
{
    Light2D attackLight;

    [SerializeField]  const  float DISAPPEAR_TIMER_MAX = 0.4f;
    float disappearTimer;
    // Start is called before the first frame update
    void Start()
    {
        disappearTimer = DISAPPEAR_TIMER_MAX;
        attackLight = this.GetComponent<Light2D>();
    }

   
    // Update is called once per frame
    void Update()
    {
        if (disappearTimer > DISAPPEAR_TIMER_MAX * 0.5 && attackLight.intensity < 2f)
        {
            float increaseScaleAmount = 0.05f;
            attackLight.intensity += increaseScaleAmount;

        } else
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
}

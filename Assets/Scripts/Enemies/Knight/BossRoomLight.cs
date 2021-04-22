using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class BossRoomLight : MonoBehaviour
{
    Light2D light;
    float initIntensity;

    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light2D>();
        initIntensity = light.intensity;
    }

    //a faire en routine ?
    public IEnumerator Dimlight()
    {
        Debug.Log("in ie");
        while (light.intensity > 0.2f)
        {
            light.intensity -= 0.5f * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator ResetInt()
    {
        while (light.intensity < initIntensity)
        {
            light.intensity += 0.5f * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}

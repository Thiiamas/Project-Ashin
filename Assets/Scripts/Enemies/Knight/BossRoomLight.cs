using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class BossRoomLight : MonoBehaviour
{
    Light2D customLight;
    float initIntensity;

    // Start is called before the first frame update
    void Start()
    {
        customLight = GetComponent<Light2D>();
        initIntensity = customLight.intensity;
    }

    //a faire en routine ?
    public IEnumerator Dimlight()
    {
        Debug.Log("in ie");
        while (customLight.intensity > 0.2f)
        {
            customLight.intensity -= 0.5f * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator ResetInt()
    {
        while (customLight.intensity < initIntensity)
        {
            customLight.intensity += 0.5f * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}

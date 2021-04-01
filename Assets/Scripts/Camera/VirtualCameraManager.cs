using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class VirtualCameraManager : MonoBehaviour
{

    private static VirtualCameraManager instance;
    public static VirtualCameraManager Instance { get { return instance; } }


    private CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin perlin;
    private float shakeTimer;


    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        virtualCamera = this.GetComponent<CinemachineVirtualCamera>();
        perlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    // Update is called once per frame
    void Update()
    {

        if( shakeTimer > 0 )
        {
            shakeTimer -= Time.deltaTime;
            if(shakeTimer <= 0) {
                perlin.m_AmplitudeGain = 0f;
            }
        }
        
    }


    public void ShakeCamera(float intensity, float time) 
    {
        perlin.m_AmplitudeGain = intensity;
        shakeTimer = time;
    }






}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private static GameManager instance;
    private Transform playerTransform;
    private WaveSpawner waveSpawner;


    [Header("Text Pop Up")]
    [SerializeField] public GameObject damagePopupPrefab;


    [Header("Particle Effect")]
    [SerializeField] public GameObject HurtEffectPrefab;


    [Header("Materials")]
    [SerializeField] public Material WhiteMaterial;
    [SerializeField] public Material DefaultMaterial;


    
    #region getters

    public static GameManager Instance { get { return instance; } }
    public Transform PlayerTransform { get { return playerTransform; } }
    public WaveSpawner WaveSpawner { get { return waveSpawner; } }

    #endregion

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        //waveSpawner = GameObject.FindGameObjectWithTag("WaveSpawner").GetComponent<WaveSpawner>();
    }

}

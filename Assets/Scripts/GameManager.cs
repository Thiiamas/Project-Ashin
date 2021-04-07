using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }


    [Header("Text Pop Up")]
    [SerializeField] public GameObject damagePopupPrefab;


    [Header("Particle Effect")]
    [SerializeField] public GameObject HurtEffectPrefab;



    [Header("Materials")]
    [SerializeField] public Material WhiteMaterial;
    [SerializeField] public Material DefaultMaterial;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }
    }

}

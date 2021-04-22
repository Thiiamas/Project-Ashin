using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSun : MonoBehaviour
{
    Transform playerTransform;
    [SerializeField] float attackDelay = 0.5f;
    [SerializeField] GameObject LightRay;
    float lastAttack;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        lastAttack = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
       if (Time.time > lastAttack + attackDelay)
        {
            lastAttack = Time.time;
            GameObject lightRay = Instantiate(LightRay, transform.position, Quaternion.identity);
            lightRay.SetActive(true);
        } 
    }
}

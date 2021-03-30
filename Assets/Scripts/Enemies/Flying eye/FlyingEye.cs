using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEye : Enemy
{
    PlayerController playerController;
    float attack1Dmg = 10f;
    // Start is called before the first frame update

     protected override void Setup()
    {
        playerController = playerTransform.GetComponent<PlayerController>();
    }
    //fired in animation "attack1" of flyingEye
    public void attack1()
    {
        playerController.TakeDamage(attack1Dmg);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

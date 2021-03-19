using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEye : Enemy
{
    Player_Health playerHealth;
    float attack1Dmg = 10f;
    // Start is called before the first frame update

     protected override void Setup()
    {
        playerHealth = player.GetComponent<Player_Health>();
    }
    //fired in animation "attack1" of flyingEye
    public void attack1()
    {
        Debug.Log("ici");
        playerHealth.takeDamage(attack1Dmg);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

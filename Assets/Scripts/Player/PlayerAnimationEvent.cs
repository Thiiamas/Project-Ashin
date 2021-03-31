using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    
    PlayerAttack playerAttack;    
    
    // Start is called before the first frame update
    void Start()
    {
        playerAttack = GetComponentInParent<PlayerAttack>();
    }

    public void FinishAttack()
    {
        playerAttack.FinishAttack();
    }

}

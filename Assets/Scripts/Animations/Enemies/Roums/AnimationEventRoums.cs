using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventRoums : MonoBehaviour
{
    Roumsor roumsor;
    Roums roums;
    // Start is called before the first frame update
    void Start()
    {
        roumsor = GetComponentInParent<Roumsor>();
        roums = GetComponentInParent<Roums>();
    }

    public void attack1()
    {
        roumsor.Attack1();
    }

    public void attackRange()
    {
        roums.RangedAttack();
    }
}

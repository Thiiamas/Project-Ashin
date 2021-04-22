using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueKnightGFX : MonoBehaviour
{
    BlueKnight blueKnight;
    // Start is called before the first frame update
    void Start()
    {
        blueKnight = GetComponentInParent<BlueKnight>();
    }

    public void DiveAttack()
    {
        blueKnight.DiveAttack();
    }

    public void Flurry()
    {
        blueKnight.Flurry();
    }

    public void basicAttack()
    {
        blueKnight.basicAttack();
    }
}

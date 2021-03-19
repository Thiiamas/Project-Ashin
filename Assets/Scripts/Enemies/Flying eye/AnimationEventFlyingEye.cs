using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventFlyingEye : MonoBehaviour
{
    FlyingEye eye;
    // Start is called before the first frame update

    void Start()
    {
        eye = GetComponentInParent<FlyingEye>();
    }

    public void attack1()
    {
        eye.attack1();
    }
}

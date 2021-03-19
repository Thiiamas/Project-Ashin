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
        if (eye == null)
        {
            Debug.Log("aaa");
        }
    }

    public void attack1()
    {
        Debug.Log("et ici");
        eye.attack1();
    }
}

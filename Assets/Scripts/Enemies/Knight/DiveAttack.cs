using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiveAttack : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
        Destroy(gameObject, 0.4f);
    }
}

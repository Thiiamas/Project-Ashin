using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunRayBoss : MonoBehaviour
{
    [SerializeField] Transform parentTransform;
    Transform initTransform;
    private void Start()
    {
        initTransform = transform;
    }
    public void PointsTowards(Transform target)
    {
        //Makes light point toward the player
        Vector3 vector = (parentTransform.position - target.position);
        Quaternion rotation = transform.rotation;
        Vector3 eulerAngles = rotation.eulerAngles;
        eulerAngles.z = 180 - Vector2.Angle(parentTransform.up, vector);
        if (transform.position.x - target.position.x < 0)
        {
            eulerAngles.z = -eulerAngles.z;
        }
        rotation.eulerAngles = eulerAngles;
        transform.rotation = rotation;
    }

    public void ResetTransform() 
    {
        transform.position = initTransform.position;
        transform.rotation = initTransform.rotation;
    }

}


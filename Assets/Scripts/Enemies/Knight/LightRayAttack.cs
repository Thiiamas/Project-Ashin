using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightRayAttack : MonoBehaviour
{
    Animator animator;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] Transform sunTraansform;
    Transform playerTransform;
    Vector3 vector;

    float attackDamage = 10f;
    void Start()
    {
        animator = GetComponent<Animator>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        vector = (sunTraansform.position - playerTransform.position);

        //Makes light point toward the player
        Quaternion rotation = transform.rotation;
        Vector3 eulerAngles = rotation.eulerAngles;
        eulerAngles.z = 180 - Vector2.Angle(sunTraansform.up, vector);
        if (transform.position.x - playerTransform.position.x < 0)
        {
            eulerAngles.z = -eulerAngles.z;
        }
        rotation.eulerAngles = eulerAngles;
        transform.rotation = rotation;

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Fire()
    {
        List<Collider2D> hitten = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(playerLayer);
        filter.useTriggers = true;
        PolygonCollider2D collider = GetComponent<PolygonCollider2D>();
        Physics2D.OverlapCollider(collider, filter, hitten);

        foreach (Collider2D hit in hitten)
        {
            if (hit.gameObject.tag == "Player")
            {
                hit.GetComponent<PlayerController>().TakeDamage(this.transform, attackDamage);
            }
        }
        Destroy(gameObject);
    }
    private void OnDrawGizmos()
    {
       
        Debug.Log(Vector2.Angle(sunTraansform.up, vector));
        Gizmos.DrawLine(playerTransform.position, transform.position);
    }
}

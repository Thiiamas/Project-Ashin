using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombController : MonoBehaviour
{

    const string STATE_EXPLODE = "Bomb_Explosion";


    //[SerializeField] CircleCollider2D explosionCollider;
    [SerializeField] float explosionRadius = 1f;
    [SerializeField] float timeBeforeExplosion = 1f;
    Rigidbody2D rb;
    Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
    }


    public void Explode()
    {
        /*
            List<Collider2D> hitten = new List<Collider2D>();
            ContactFilter2D filter = new ContactFilter2D();
            filter.SetLayerMask(playerLayer);
            filter.useTriggers = true;
            Physics2D.OverlapCollider(basicAttackCollider, filter, hitten);

            foreach (Collider2D hit in hitten)
            {
                if (hit.gameObject.tag == "Player")
                {
                    hit.GetComponent<PlayerController>().TakeDamage(this.transform, basicAttackDamage);
                }
            }
        */
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].gameObject.tag == "Player")
            {
                //Destroy(hitColliders[i].gameObject);
            }
        }
    }

    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }

    void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        StartCoroutine(StartExplosionTimer());
    }
    
    IEnumerator StartExplosionTimer()
    {
        yield return new WaitForSeconds(timeBeforeExplosion);
        animator.Play(STATE_EXPLODE);
    }

}

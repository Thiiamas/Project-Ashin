using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
	public int attackDamage = 20;

	public Transform attackPoint;
	public float attackRange = 1f;
	public LayerMask attackMask;

	public void basicAttack()
	{
		Collider2D colInfo = Physics2D.OverlapCircle(attackPoint.position, attackRange, attackMask);
		if (colInfo != null)
		{
			colInfo.GetComponent<PlayerController>().TakeDamage(this.transform, attackDamage);
		}
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(attackPoint.position, attackRange);
	}
}

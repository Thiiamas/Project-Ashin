using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
	public int attackDamage = 20;

	public float attackRange = 1f;
	public Transform basicAttackPoint;
	public LayerMask playerLayer;
	
	

	public void bossBasicAttack()
	{
		Collider2D colInfo = Physics2D.OverlapCircle(basicAttackPoint.position, attackRange, playerLayer);
		if (colInfo != null)
		{
			Debug.Log("ICI");
			colInfo.GetComponent<Player_Health>().takeDamage(attackDamage);
		}
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(basicAttackPoint.position, attackRange);
	}
}

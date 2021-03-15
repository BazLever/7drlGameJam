using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public float damage = 15;
    public float knockback = 100;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().TakeDamage(damage, (other.transform.position - transform.position).normalized * knockback);
            GetComponentInParent<WalkingEnemy>().StopLunging();
        }
    }
}

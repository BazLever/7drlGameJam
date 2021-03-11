using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public float damage = 15;
    public float knockback = 15;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.GetComponent<WalkingEnemy>().TakeDamage(damage, (other.transform.position - transform.position).normalized * knockback);
            Debug.Log("Hit enemy");
        }
    }
}

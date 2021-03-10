using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public float damage = 15;
    public float knockback = 5;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.GetComponent<WalkingEnemy>().TakeDamage(damage, (transform.position - other.transform.position).normalized * knockback);
            Debug.Log("Hit enemy");
        }
    }
}

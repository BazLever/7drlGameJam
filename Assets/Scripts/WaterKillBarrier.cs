using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterKillBarrier : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            playerController.TakeDamage(playerController.currentHealth + 1, Vector3.zero);
        }
        else if (other.gameObject.CompareTag("Enemy"))
        {
            WalkingEnemy enemy = other.GetComponent<WalkingEnemy>();
            enemy.TakeDamage(enemy.currentHealth + 1, Vector3.zero);
        }
    }
}

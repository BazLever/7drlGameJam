using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKillBarrier : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            playerController.TakeDamage(playerController.currentHealth + 1, Vector3.zero);
            Debug.Log("Player hit kill barrier");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPartRespawner : MonoBehaviour
{

    Vector3 startLocation;

    void Start()
    {
        startLocation = gameObject.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PartSpawnMenu")
        {
            gameObject.transform.position = startLocation;
        }
    }

}

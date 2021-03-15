using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardObject : MonoBehaviour
{
    private Transform mainCamera;

    private void Start()
    {
        mainCamera = Camera.main.transform;
    }

    void Update()
    {
        transform.rotation = Quaternion.LookRotation((mainCamera.position - transform.position).normalized, mainCamera.up);
    }
}

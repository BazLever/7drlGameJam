using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardObject : MonoBehaviour
{
    public Transform mainCamera;

    void Update()
    {
        transform.rotation = Quaternion.LookRotation((mainCamera.position - transform.position).normalized, mainCamera.up);
    }
}

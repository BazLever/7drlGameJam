using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshBaker : MonoBehaviour
{
    NavMeshSurface navMeshSurface;

    // Start is called before the first frame update
    void Start()
    {
        navMeshSurface = GetComponent<NavMeshSurface>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            BakeNavMesh();
        }
    }

    public void BakeNavMesh()
    {
        navMeshSurface.BuildNavMesh();
    }
}

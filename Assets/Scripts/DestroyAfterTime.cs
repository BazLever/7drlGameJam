using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float timeUntilDestroyed = 1f;

    void Update()
    {
        // destroy itself once the timer's run out
        if (timeUntilDestroyed <= 0)
            Destroy(gameObject);
        else
            timeUntilDestroyed -= Time.deltaTime;
    }
}

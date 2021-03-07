using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileRandomizer : MonoBehaviour
{

    public GameObject[] tileVariants;
    public int tileChosen;
    int tileRotation;

    void Start()
    {

        tileChosen = Random.Range(0, (tileVariants.Length));
        tileRotation = Random.Range(0, 4);
        tileVariants[tileChosen].SetActive(true);
        if (tileRotation == 0)
        {
            tileVariants[tileChosen].transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        if (tileRotation == 1)
        {
            tileVariants[tileChosen].transform.localEulerAngles = new Vector3(0, 90, 0);
        }
        if (tileRotation == 2)
        {
            tileVariants[tileChosen].transform.localEulerAngles = new Vector3(0, 180, 0);
        }
        if (tileRotation == 3)
        {
            tileVariants[tileChosen].transform.localEulerAngles = new Vector3(0, 270, 0);
        }
    }

    private void Update()
    {
        if (transform.position.y <= -40)
        {
            ChangeActiveTile();
        }
    }

    public void ChangeActiveTile()
    {
        tileVariants[tileChosen].SetActive(false);
        tileChosen = Random.Range(0, (tileVariants.Length));
        tileRotation = Random.Range(0, 4);
        tileVariants[tileChosen].SetActive(true);
        if (tileRotation == 0)
        {
            tileVariants[tileChosen].transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        if (tileRotation == 1)
        {
            tileVariants[tileChosen].transform.localEulerAngles = new Vector3(0, 90, 0);
        }
        if (tileRotation == 2)
        {
            tileVariants[tileChosen].transform.localEulerAngles = new Vector3(0, 180, 0);
        }
        if (tileRotation == 3)
        {
            tileVariants[tileChosen].transform.localEulerAngles = new Vector3(0, 270, 0);
        }
    }
}

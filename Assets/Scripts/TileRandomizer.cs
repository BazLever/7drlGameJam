using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileRandomizer : MonoBehaviour
{

    public GameObject[] tileVariants;

    private int tileChosen = 0;
    private int tileRotation;

    private Vector3 originPosition;

    void Awake()
    {
        RandomiseActiveTile();

        originPosition = transform.position;
    }

    public void RandomiseActiveTile()
    {
        // deactivate the previously active tile
        tileVariants[tileChosen].SetActive(false);

        // set random tile active
        tileChosen = Random.Range(0, (tileVariants.Length));
        tileVariants[tileChosen].SetActive(true);

        // rotate the tile in a random direction
        tileRotation = Random.Range(0, 4);
        tileVariants[tileChosen].transform.localEulerAngles = new Vector3(0, tileRotation * 90, 0);
    }

    /// <summary>
    /// attempts to raise the tile to its origin and returns true if the tile moved
    /// </summary>
    public bool Raise(float moveSpeed)
    {
        // return false since the tile won't move
        if (transform.position == originPosition)
            return false;

        transform.position = Vector3.MoveTowards(transform.position, originPosition, moveSpeed * Time.deltaTime);
        return true;
    }

    /// <summary>
    /// attempts to lower the tile and returns true if the tile moved
    /// </summary>
    public bool Lower(float moveSpeed, float lowerAmount)
    {
        // return false since the tile won't move
        if (transform.position == originPosition - new Vector3(0, lowerAmount, 0))
            return false;

        transform.position = Vector3.MoveTowards(transform.position, originPosition - new Vector3(0, lowerAmount, 0), moveSpeed * Time.deltaTime);
        return true;
    }
}

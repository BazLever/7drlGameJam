using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    //general round stats
    int round = 0;
    int enemiesToSpawn;

    //tile changing variables
    public GameObject[] randomTile;
    int tileChosen;
    bool tileFalling;
    bool tileRising;
    public float rateOfFall;

    void Start()
    {

    }


    void Update()
    {
        if (tileFalling == true)
        {
            randomTile[tileChosen].transform.position -= new Vector3(0, rateOfFall, 0);
            if (randomTile[tileChosen].transform.position.y <= -40)
            {
                tileFalling = false;
            }
        }
        if (tileRising == true)
        {
            randomTile[tileChosen].transform.position += new Vector3(0, rateOfFall, 0);
            if (randomTile[tileChosen].transform.position.y >= 0)
            {
                randomTile[tileChosen].transform.position = new Vector3(randomTile[tileChosen].transform.position.x, 0, randomTile[tileChosen].transform.position.z);
                tileRising = false;
                tileChosen = Random.Range(0, randomTile.Length);
                tileFalling = true;
            }
        }



        //debug code
        if (Input.GetKeyDown(KeyCode.P))
        {
            NewRound();
        }
    }


    public void NewRound()
    {
        round += 1;
        //Tile falling code
        if (round > 1)
        {
            tileRising = true;
        }
    }
}

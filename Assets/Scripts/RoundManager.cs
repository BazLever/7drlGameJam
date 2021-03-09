using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RoundManager : MonoBehaviour
{
    public int currentRound = 1;

    private bool roundInProgress;
    private bool roundOver;

    private int tileToDrop = -1;
    private int tileToRaise = -1; // neg 1 means no tile in the array (i handle for this when calling for it)

    public float tileMoveSpeed = 16;
    public float tileLowerAmount = 60;
    public float tileShakeTime = 1;
    private float tileShakeTimer;
    public TileRandomizer[] tiles;

    public int enemyAmount = 10;
    public float enemyAmountRoundMultiplier = 1.5f;
    public int maxSpawnedEnemyAmount = 35;
    private int enemiesToSpawnThisRound;
    private GameObject[] enemies;


    public NavMeshBaker navMeshBaker;
    public GameObject enemy;


    void Start()
    {
        navMeshBaker.BakeNavMesh();

        tileShakeTimer = tileShakeTime;

        // initialise round 1
        roundInProgress = true;
        roundOver = false;
        enemiesToSpawnThisRound = (int)(enemyAmount * (currentRound * enemyAmountRoundMultiplier));
    }

    void Update()
    {
        // end round
        if (roundOver)
        {
            roundInProgress = false;

            tileToRaise = tileToDrop;
            while (tileToDrop == tileToRaise)
                tileToDrop = Random.Range(0, tiles.Length);

            // vvv handles for if the tile to raise is -1
            if (tileToRaise >= 0)
                tiles[tileToRaise].RandomiseActiveTile();

            navMeshBaker.BakeNavMesh();

            currentRound++;
            roundOver = false;
        }

        // reset the platforms before round start
        if (!roundInProgress && MoveTiles())
        {
            navMeshBaker.BakeNavMesh();
            enemiesToSpawnThisRound = (int)(enemyAmount * (currentRound * enemyAmountRoundMultiplier));
            roundInProgress = true;
        }

        // everything that happens mid round
        if (roundInProgress)
        {
            if (Input.GetKeyDown(KeyCode.N))
                roundOver = true;

            // enemy spawning and stuff here
        }
    }

    /// <summary>
    /// moves both the tile to raise and the tile to drop toward their desired positions and returns true if they are both successfully placed
    /// </summary>
    private bool MoveTiles()
    {
        //                           vvv handles for if the tile to raise is -1
        bool finishedRaisingTile = tileToRaise >= 0 ? !tiles[tileToRaise].Raise(tileMoveSpeed) : true;
        bool finishedDropingTile = false;

        if (tileShakeTimer <= 0)
        {
            finishedDropingTile = !tiles[tileToDrop].Lower(tileMoveSpeed, tileLowerAmount);
        }
        else
        {
            tileShakeTimer -= Time.deltaTime;
            // need to shake the platform here
        }

        if (finishedRaisingTile && finishedDropingTile)
        {
            tileShakeTimer = tileShakeTime;
            return true;
        }

        return false;
    }
}

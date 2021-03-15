using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RoundManager : MonoBehaviour
{
    public int currentRound = 0;

    private bool roundInProgress;
    private bool roundOver;

    private int tileToDrop = -1;
    private int tileToRaise = -1; // neg 1 means no tile in the array (i handle for this when calling for it)

    [Space]
    [Header("Map Tile Variables:")]
    public float tileMoveSpeed = 16;
    public float tileLowerAmount = 60;
    public float tileShakeTime = 1;
    private float tileShakeTimer;
    public TileRandomizer[] tiles;

    [Space]
    [Header("Enemy Spawning Variables:")]
    public int enemyAmount = 10;
    public float enemySpawnHeight = 50f;
    public float enemyAmountRoundMultiplier = 1.5f;
    public int maxSpawnedEnemyAmount = 35;
    public float timeBetweenEnemySpawns = 0.25f;
    private float timerBetweenEnemySpawns = 0;
    private int enemiesToSpawnThisRound = 0;
    private int currentEnemyAmount = 0;
    private int tileToSpawnEnemyOn = 0;
    public GameObject[] eliteEnemies;

    [Space]
    [Header("Referances:")]
    public NavMeshBaker navMeshBaker;
    public Animator roundCompleteAnim;
    public GameObject enemy;
    private PlayerController playerController;


    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        navMeshBaker.BakeNavMesh();

        tileShakeTimer = tileShakeTime;

        // initialise round 1
        roundInProgress = true;
        roundOver = false;
        enemiesToSpawnThisRound = (int)(enemyAmount + (currentRound * enemyAmountRoundMultiplier));
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

            tiles[tileToDrop].tileShakeParticles.Play();

            // vvv handles for if the tile to raise is -1
            if (tileToRaise >= 0)
                tiles[tileToRaise].RandomiseActiveTile();

            navMeshBaker.BakeNavMesh();

            roundCompleteAnim.SetTrigger("Show");

            playerController.Heal(playerController.maxHealth);

            currentRound++;
            roundOver = false;
        }

        // reset the platforms before round start
        if (!roundInProgress && MoveTiles())
        {
            navMeshBaker.BakeNavMesh();
            enemiesToSpawnThisRound = (int)(enemyAmount + (currentRound * enemyAmountRoundMultiplier));
            currentEnemyAmount = 0;
            tileToSpawnEnemyOn = 0;

            // begin the next round
            roundInProgress = true;
        }

        // everything that happens mid round
        if (roundInProgress)
        {
            if (enemiesToSpawnThisRound <= 0 && currentEnemyAmount <= 0)
                roundOver = true;

            // enemy spawning and stuff here
            if (timerBetweenEnemySpawns <= 0)
            {
                if (currentEnemyAmount < maxSpawnedEnemyAmount && enemiesToSpawnThisRound > 0)
                {
                    // spawn the actual enemy
                    GameObject newEnemy;
                    if (enemiesToSpawnThisRound > 1)
                        newEnemy = Instantiate(enemy, tiles[tileToSpawnEnemyOn].transform.position + new Vector3(0, enemySpawnHeight, 0), Quaternion.identity);
                    else
                        newEnemy = Instantiate(eliteEnemies[Random.Range(0, eliteEnemies.Length)], tiles[tileToSpawnEnemyOn].transform.position + new Vector3(0, enemySpawnHeight, 0), Quaternion.identity);

                    // progress the tile to spawn the enemy on for the next enemy
                    tileToSpawnEnemyOn += tileToSpawnEnemyOn < tiles.Length - 1 ? 1 : -tileToSpawnEnemyOn;
                    // make sure the enemies won't spawn on the dropped platform
                    if (tileToSpawnEnemyOn == tileToDrop)
                        tileToSpawnEnemyOn += tileToSpawnEnemyOn < tiles.Length - 1 ? 1 : -tileToSpawnEnemyOn;

                    //reset the enemy spawning timer
                    timerBetweenEnemySpawns = timeBetweenEnemySpawns;

                    currentEnemyAmount++;
                    enemiesToSpawnThisRound--;
                }
            }
            else
                timerBetweenEnemySpawns -= Time.deltaTime;
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

    /// <summary>
    /// adds the number to the current enemy amount
    /// </summary>
    public void ModifyCurrentEnemyAmount(int number)
    {
        currentEnemyAmount += number;
    }
}

using Player;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
//using UnityEditor.iOS;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

public class PlatformSpawner : MonoBehaviour
{
    private const float PLAyer_DISTANCE_LEVEL_PART = 40F;
    private const float PLAyer_DISTANCE_LEVEL_PART_Destroy = 50F;

    [SerializeField] Transform levelpart_Start;
    [SerializeField] List<Transform> levelpartLists;
    [SerializeField] List<Transform> levelpartLists2;
    [SerializeField] List<Transform> levelpartLists3;
    [SerializeField] Movement movement;
    [SerializeField] private Transform player;  // Reference to the player
    [Range(0f, 1f)] readonly float coinchanse = 0.5f;
    public GameObject Coin;

    private readonly Queue<Transform> activeLevelPartsQueue = new();
    private readonly System.Random rng = new();

    private Vector3 lastEndPosition;
    private Vector3 startPosition;

    [SerializeField] bool newMediumLevel = false;
    [SerializeField] bool newHardLevel = false;

    public void Awake()
    {
        // Subscribe to the OnScoreChanged event
        ScoreController scoreManager = FindObjectOfType<ScoreController>();
        if (scoreManager != null)
        {
            scoreManager.OnScroreChanged += HandleScoreChanged;
        }

        lastEndPosition = levelpart_Start.Find("EndPosition").position;
        startPosition = levelpart_Start.Find("StartPlatform").position;
        int startingSpawnLevelPart = 1;

        for (int i = 0; i < startingSpawnLevelPart; i++)
        {
            SpawnLevelPart();
        }
    }
    private void Update()
    {
        if (Vector3.Distance(movement.transform.position, lastEndPosition) < PLAyer_DISTANCE_LEVEL_PART)
        {
            SpawnLevelPart();
        }

        if (Vector3.Distance(movement.transform.position, startPosition) > PLAyer_DISTANCE_LEVEL_PART_Destroy && levelpart_Start != null)
        {
            Destroy(levelpart_Start.gameObject);
        }

    }
    public void DespawnPlatforms(float shiftOffset)
    {
        // Int to add platforms to Queue
        int count = activeLevelPartsQueue.Count;

        for (int i = 0; i < count; i++)
        {
            //Move platforms down

            Transform platform = activeLevelPartsQueue.Dequeue();
            platform.position -= new Vector3(0, shiftOffset, 0);

            //For the moving Platforms
            MovingPlatform movingPlatform = platform.GetComponent<MovingPlatform>();
            if (movingPlatform != null)
            {
                movingPlatform.ShiftPositions(new Vector3(0, -shiftOffset, 0));
            }
            // Destroys last platform if above a certain amount
            if (activeLevelPartsQueue.Count >= 3)
            {
                Destroy(platform.gameObject);
            }
            else
            {
                activeLevelPartsQueue.Enqueue(platform); // Re-add the platform to the queue
            }
        }

        lastEndPosition -= new Vector3(0, shiftOffset, 0);
    }

    private void HandleScoreChanged(int newScore)
    {   // New Level difficulty
        if (newScore == 50) 
        {
            newMediumLevel = true; // Medium
        }
        else if (newScore == 100)
        {
            newMediumLevel = false;
            newHardLevel = true; // Hard
        }
    }
    private void SpawnLevelPart()
    {
        //Transform chosenLevelPart = levelpartLists[Random.Range(0, levelpartLists.Count)];

        int index = rng.Next(levelpartLists.Count); // Similar to Random.Range(0, Count)
        Transform chosenLevelPart = levelpartLists[index];
        
        if (newMediumLevel)
        {
            //chosenLevelPart = levelpartLists2[Random.Range(0, levelpartLists2.Count)];
            index = rng.Next(levelpartLists2.Count); // Similar to Random.Range(0, Count)
            chosenLevelPart = levelpartLists2[index];
        }

        else if (newHardLevel)
        {
            //chosenLevelPart = levelpartLists3[Random.Range(0, levelpartLists3.Count)];
            index = rng.Next(levelpartLists3.Count); // Similar to Random.Range(0, Count)
            chosenLevelPart = levelpartLists3[index];
        }
        Transform lastLevelPartTransform = SpawnLevelPart(chosenLevelPart, lastEndPosition);
        lastEndPosition = lastLevelPartTransform.Find("EndPosition").position;

        Transform coinSpawner = lastLevelPartTransform.Find("CoinSpawner");
        
        float roll = Random.value; // Random value between 0 and 1
        if (roll < coinchanse) // Item drop
        {
            GameObject coinInstance = Instantiate(Coin, coinSpawner.position, Quaternion.identity);
            coinInstance.transform.SetParent(lastLevelPartTransform);

            Debug.LogWarning("Hello again");
        }
    }
    private Transform SpawnLevelPart(Transform levelPart, Vector3 sPawnPosition)
    {
        Transform levelpartTransform = Instantiate(levelPart, sPawnPosition, Quaternion.identity);
        activeLevelPartsQueue.Enqueue(levelpartTransform);
        return levelpartTransform;   
    }
}

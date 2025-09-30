using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // A public array to hold all the different enemy prefabs
    public GameObject[] allEnemyPrefabs;
    // A queue to hold the prefabs in a non-repeating order
    private Queue<GameObject> enemyPrefabQueue = new Queue<GameObject>();

    // Cached reference to the AR Dungeon Root Transform
    private Transform arDungeonRootTransform;

    void Awake()
    {
        // Get the parent of this spawner (assumed to be the ARDungeonRoot)
        arDungeonRootTransform = transform.parent;

        if (arDungeonRootTransform == null)
        {
            Debug.LogError("EnemySpawner must be a child of the ARDungeonRoot to set the correct AR parent.");
        }

        InitializeEnemyQueue();
    }

    private void InitializeEnemyQueue()
    {
        // Convert the array to a list so we can shuffle it
        List<GameObject> tempList = new List<GameObject>(allEnemyPrefabs);
        // Shuffle the list using a simple Fisher-Yates shuffle algorithm
        for (int i = 0; i < tempList.Count; i++)
        {
            GameObject temp = tempList[i];
            int randomIndex = Random.Range(i, tempList.Count);
            tempList[i] = tempList[randomIndex];
            tempList[randomIndex] = temp;
        }
        // Add the shuffled prefabs to the queue
        foreach (GameObject prefab in tempList)
        {
            enemyPrefabQueue.Enqueue(prefab);
        }
    }

    public EnemyStats SpawnEnemy()
    {
        // If the queue is empty, all enemies have been shown. Reinitialize it.
        if (enemyPrefabQueue.Count == 0)
        {
            InitializeEnemyQueue();
        }

        // Dequeue the next enemy prefab from the list
        GameObject selectedPrefab = enemyPrefabQueue.Dequeue();

        // 1. Instantiate the selected prefab
        // 2. Set its parent to the ARDungeonRoot Transform for AR tracking
        GameObject newEnemy = Instantiate(selectedPrefab, arDungeonRootTransform);

        // Ensure the local position is zero so it spawns at the root's anchor point
        newEnemy.transform.localPosition = Vector3.zero;

        // FIX: Ensure the enemy is spawned inactive so the DungeonManager can reveal it 
        // after the intro or at the correct time.
        newEnemy.SetActive(false);

        EnemyStats enemyStats = newEnemy.GetComponent<EnemyStats>();

        if (enemyStats == null)
        {
            Debug.LogError("Enemy prefab is missing EnemyStats component!");
        }
        return enemyStats;
    }
}
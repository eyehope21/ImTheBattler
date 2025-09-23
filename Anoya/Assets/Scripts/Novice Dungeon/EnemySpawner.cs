using System.Collections.Generic; // Make sure this is included
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // A public array to hold all the different enemy prefabs
    public GameObject[] allEnemyPrefabs;
    // A queue to hold the prefabs in a non-repeating order
    private Queue<GameObject> enemyPrefabQueue = new Queue<GameObject>();
    // A reference to the parent transform where the enemy UI should be placed
    public Transform enemyUIParent;

    void Awake()
    {
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
        // Instantiate the selected prefab as a child of the enemyUIParent
        GameObject newEnemy = Instantiate(selectedPrefab, enemyUIParent);
        EnemyStats enemyStats = newEnemy.GetComponent<EnemyStats>();
        if (enemyStats == null)
        {
            Debug.LogError("Enemy prefab is missing EnemyStats component!");
        }
        return enemyStats;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Minion Prefabs by Term")]
    [Tooltip("Prefab for Prelim")]
    public GameObject prelimMinionPrefab;
    [Tooltip("Prefab for Midterms")]
    public GameObject midtermMinionPrefab;
    [Tooltip("Prefab for Prefinals")]
    public GameObject prefinalsMinionPrefab;
    [Tooltip("Prefab for Finals")]
    public GameObject finalsMinionPrefab;
    private Queue<GameObject> enemyPrefabQueue = new Queue<GameObject>();

    private Transform arDungeonRootTransform;

    public SchoolTerm selectedTerm { get; private set; }

    public void SetTermFilter(SchoolTerm term)
    {
        selectedTerm = term;
    }

    void Awake()
    {
        arDungeonRootTransform = transform.parent;
        if (arDungeonRootTransform == null)
        {
            Debug.LogError("EnemySpawner must be a child of the ARDungeonRoot to set the correct AR parent.");
        }
    }

    public void InitializeDungeonQueue()
    {
        enemyPrefabQueue.Clear();
        GameObject prefabToUse = GetMinionPrefabForCurrentTerm();

        if (prefabToUse == null)
        {
            Debug.LogError($"No prefab found for selected term: {selectedTerm}!");
            return;
        }

        for (int i = 0; i < 10; i++)
        {
            enemyPrefabQueue.Enqueue(prefabToUse);
        }
    }

    private GameObject GetMinionPrefabForCurrentTerm()
    {
        return selectedTerm switch
        {
            SchoolTerm.Prelim => prelimMinionPrefab,
            SchoolTerm.Midterms => midtermMinionPrefab,
            SchoolTerm.Prefinals => prefinalsMinionPrefab,
            SchoolTerm.Finals => finalsMinionPrefab,
            _ => null
        };
    }

    public EnemyStats SpawnEnemy()
    {
        if (enemyPrefabQueue.Count == 0)
        {
            InitializeDungeonQueue();
        }

        if (enemyPrefabQueue.Count == 0)
        {
            Debug.LogError("Enemy queue is empty after initialization. Cannot spawn.");
            return null;
        }

        GameObject selectedPrefab = enemyPrefabQueue.Dequeue();

        GameObject newEnemy = Instantiate(selectedPrefab, arDungeonRootTransform);

        newEnemy.transform.localPosition = Vector3.zero;
        newEnemy.SetActive(false);

        EnemyStats enemyStats = newEnemy.GetComponent<EnemyStats>();

        if (enemyStats == null)
        {
            Debug.LogError("Enemy prefab is missing EnemyStats component!");
            return null;
        }
        return enemyStats;
    }
}
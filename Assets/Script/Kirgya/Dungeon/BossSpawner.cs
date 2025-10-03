using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    // --- BOSS PREFAB FIELDS ---
    [Header("Boss Term Prefabs")]
    [Tooltip("Assign the boss prefab for the Prelim term.")]
    public GameObject prelimBossPrefab;
    [Tooltip("Assign the boss prefab for the Midterms term.")]
    public GameObject midtermBossPrefab;
    [Tooltip("Assign the boss prefab for the Prefinals term.")]
    public GameObject prefinalsBossPrefab;
    // --------------------------

    private Transform arDungeonRootTransform;

    public SchoolTerm selectedTerm { get; private set; }
    // REMOVED: public Difficulty selectedDifficulty { get; private set; }

    public void SetTermFilter(SchoolTerm term)
    {
        selectedTerm = term;
    }

    // REMOVED: public void SetDifficultyFilter(Difficulty difficulty) {}

    void Awake()
    {
        arDungeonRootTransform = transform.parent;
        if (arDungeonRootTransform == null)
        {
            Debug.LogError("BossSpawner must be a child of the ARDungeonRoot to set the correct AR parent.");
        }
    }

    public void InitializeBoss()
    {
        GameObject prefabCheck = selectedTerm switch
        {
            SchoolTerm.Prelim => prelimBossPrefab,
            SchoolTerm.Midterms => midtermBossPrefab,
            SchoolTerm.Prefinals => prefinalsBossPrefab,
            _ => null
        };

        if (prefabCheck == null)
        {
            Debug.LogError($"BOSS PREFAB MISSING for {selectedTerm}! Assign it in the Inspector.");
        }
    }

    public BossStats SpawnBoss()
    {
        // 1. Select the correct prefab based on the term
        GameObject bossPrefabToInstantiate = selectedTerm switch
        {
            SchoolTerm.Prelim => prelimBossPrefab,
            SchoolTerm.Midterms => midtermBossPrefab,
            SchoolTerm.Prefinals => prefinalsBossPrefab,
            _ => prelimBossPrefab // Fallback
        };

        if (bossPrefabToInstantiate == null)
        {
            Debug.LogError($"Boss Prefab is missing for term: {selectedTerm}! Cannot spawn boss.");
            return null;
        }

        // 2. Instantiate the boss prefab
        GameObject obj = Instantiate(bossPrefabToInstantiate, arDungeonRootTransform);

        obj.transform.localPosition = Vector3.zero;
        obj.SetActive(false);

        // Get the BossStats component
        BossStats bossStats = obj.GetComponent<BossStats>();

        if (bossStats == null)
        {
            Debug.LogError($"Spawned boss '{obj.name}' is missing the BossStats component!");
        }

        // REMOVED: bossStats.InitializeStats(selectedDifficulty);

        return bossStats;
    }
}
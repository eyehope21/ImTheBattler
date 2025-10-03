using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    // --- NEW: BOSS PREFAB FIELDS ---
    [Header("Boss Term Prefabs")]
    [Tooltip("Assign the boss prefab for the Prelim term.")]
    public GameObject prelimBossPrefab;
    [Tooltip("Assign the boss prefab for the Midterms term.")]
    public GameObject midtermBossPrefab;
    [Tooltip("Assign the boss prefab for the Prefinals term.")]
    public GameObject prefinalsBossPrefab;
    [Tooltip("Assign the boss prefab for the Finals term.")]
    public GameObject finalsBossPrefab;
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
            SchoolTerm.Finals => finalsBossPrefab,
            _ => null
        };

        if (prefabCheck == null)
        {
            Debug.LogError($"BOSS PREFAB MISSING for {selectedTerm}! Assign it in the Inspector.");
        }
    }

    public BossStats SpawnBoss()
    {
        GameObject bossPrefabToInstantiate = null;

        switch (selectedTerm)
        {
            case SchoolTerm.Prelim:
                bossPrefabToInstantiate = prelimBossPrefab;
                break;
            case SchoolTerm.Midterms:
                bossPrefabToInstantiate = midtermBossPrefab;
                break;
            case SchoolTerm.Prefinals:
                bossPrefabToInstantiate = prefinalsBossPrefab;
                break;
            case SchoolTerm.Finals:
                bossPrefabToInstantiate = finalsBossPrefab;
                break;
            default:
                Debug.LogWarning("No SchoolTerm selected. Defaulting to Prelim boss.");
                bossPrefabToInstantiate = prelimBossPrefab; // Fallback
                break;
        }

        if (bossPrefabToInstantiate == null)
        {
            Debug.LogError($"Boss Prefab is missing for term: {selectedTerm}! Cannot spawn boss.");
            return null;
        }

        // 2. Instantiate the boss prefab
        GameObject obj = Instantiate(bossPrefabToInstantiate, arDungeonRootTransform);

        // Ensure the local position is zero so it spawns at the root's anchor point
        obj.transform.localPosition = Vector3.zero;

        // Ensure the boss is spawned inactive
        obj.SetActive(false);

        // Get the BossStats component (which should be on the root object)
        BossStats bossStats = obj.GetComponent<BossStats>();

        if (bossStats == null)
        {
            Debug.LogError($"Spawned boss '{obj.name}' is missing the BossStats component!");
        }

        return bossStats;
    }
}
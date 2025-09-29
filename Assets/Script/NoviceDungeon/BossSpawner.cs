using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public GameObject bossPrefab;

    // Cached reference to the AR Dungeon Root Transform (the Grandparent)
    private Transform arDungeonRootTransform;

    void Awake()
    {
        // Get the parent of this spawner, which should be the ARDungeonRoot
        // This assumes BossSpawner is placed directly under the ARDungeonRoot in the hierarchy.
        // If it's a grandchild, you'd use transform.parent.parent;
        arDungeonRootTransform = transform.parent;

        if (arDungeonRootTransform == null)
        {
            Debug.LogError("BossSpawner must be a child of the ARDungeonRoot to set the correct AR parent.");
        }
    }

    public BossStats SpawnBoss()
    {
        // 1. Instantiate the boss prefab
        // 2. Set its parent to the ARDungeonRoot Transform for AR tracking
        GameObject obj = Instantiate(bossPrefab, arDungeonRootTransform);

        // Ensure the local position is zero so it spawns at the root's anchor point
        obj.transform.localPosition = Vector3.zero;

        // Return the BossStats component
        return obj.GetComponent<BossStats>();
    }
}
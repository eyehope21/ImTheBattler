using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public GameObject bossPrefab;

    public BossStats SpawnBoss()
    {
        GameObject obj = Instantiate(bossPrefab, transform);
        return obj.GetComponent<BossStats>();
    }
}
using UnityEngine;

public class PortalPlacement : MonoBehaviour
{
    public GameObject portalPrefab;
    public float spawnDistance = 5f;

    void Start()
    {
        // Spawns the portal instantly, 3 meters in front of the camera
        Vector3 spawnPosition = Camera.main.transform.position + Camera.main.transform.forward * spawnDistance;
        Instantiate(portalPrefab, spawnPosition, Quaternion.identity);
    }
}

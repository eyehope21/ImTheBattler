using UnityEngine;
using TMPro;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PortalManager : MonoBehaviour
{
    public GameObject portalPrefab;
    public Image guideArrowUI;
    public TMP_Text portalFoundTextUI;

    private GameObject spawnedPortal;
    private Vector3 portalSpawnLocation;
    private ARPlaneManager arPlaneManager;

    void Start()
    {
        arPlaneManager = FindObjectOfType<ARPlaneManager>();
        arPlaneManager.planesChanged += OnPlanesChanged;
    }

    void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        if (spawnedPortal == null && args.added.Count > 0)
        {
            ARPlane plane = args.added[0];

            // Choose a random location on the plane at least 2m away
            Vector3 randomPoint = GetRandomPointOnPlane(plane);
            if (Vector3.Distance(Camera.main.transform.position, randomPoint) > 2f)
            {
                portalSpawnLocation = randomPoint;
                guideArrowUI.gameObject.SetActive(true);
                arPlaneManager.planesChanged -= OnPlanesChanged;
            }
        }
    }

    void Update()
    {
        if (spawnedPortal == null && portalSpawnLocation != Vector3.zero)
        {
            // Rotate the arrow to guide the player
            Vector3 direction = (portalSpawnLocation - Camera.main.transform.position).normalized;
            guideArrowUI.transform.rotation = Quaternion.LookRotation(direction);

            // Check if player is near the spawn location
            if (Vector3.Distance(Camera.main.transform.position, portalSpawnLocation) < 2f)
            {
                portalFoundTextUI.gameObject.SetActive(true);
                SpawnPortal();
            }
        }
    }

    void SpawnPortal()
    {
        // Hide the UI elements
        guideArrowUI.gameObject.SetActive(false);
        portalFoundTextUI.gameObject.SetActive(false);

        // Spawn the portal
        spawnedPortal = Instantiate(portalPrefab, portalSpawnLocation, Quaternion.identity);
    }

    Vector3 GetRandomPointOnPlane(ARPlane plane)
    {
        // Simple method to get a point on the plane
        return plane.transform.position;
    }
}
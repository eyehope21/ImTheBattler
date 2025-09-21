using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class PortalPlacement : MonoBehaviour
{
    public GameObject portalPrefab;

    private ARPlaneManager arPlaneManager;
    private GameObject spawnedPortal;

    void Start()
    {
        arPlaneManager = GetComponent<ARPlaneManager>();
        arPlaneManager.planesChanged += OnPlanesChanged;
    }

    void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        // Don't spawn if a portal already exists
        if (spawnedPortal != null)
        {
            return;
        }

        // Check if any new planes have been detected
        if (args.added.Count > 0)
        {
            // Spawn the portal on the first detected plane
            Debug.Log("Plane detected! Spawning portal..."); //  New log
            ARPlane firstPlane = args.added[0];
            spawnedPortal = Instantiate(portalPrefab, firstPlane.transform.position, Quaternion.identity);

            // Unsubscribe from the event so it only spawns once
            arPlaneManager.planesChanged -= OnPlanesChanged;
        }
    }
}
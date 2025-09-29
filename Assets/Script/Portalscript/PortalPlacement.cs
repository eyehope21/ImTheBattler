using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class PortalPlacement : MonoBehaviour
{
    public GameObject portalPrefab;

    private ARPlaneManager arPlaneManager;
    private ARAnchorManager arAnchorManager;
    private GameObject spawnedPortal;

    void Start()
    {
        arPlaneManager = GetComponent<ARPlaneManager>();
        arAnchorManager = GetComponent<ARAnchorManager>();
        if (arPlaneManager != null)
        {
            arPlaneManager.planesChanged += OnPlanesChanged;
        }
    }

    void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        if (spawnedPortal != null)
        {
            return;
        }

        if (args.added.Count > 0)
        {
            ARPlane firstPlane = args.added[0];

            Pose planePose = new Pose(firstPlane.transform.position, firstPlane.transform.rotation);

            ARAnchor anchor = arAnchorManager.AttachAnchor(firstPlane, planePose);
            spawnedPortal = Instantiate(portalPrefab, anchor.transform);

            arPlaneManager.planesChanged -= OnPlanesChanged;
        }
    }
}
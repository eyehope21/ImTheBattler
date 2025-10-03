using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PortalManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject portalPrefab;
    public GameObject sparksPrefab;

    [Header("UI References")]
    public TMP_Text statusText;
    public Button instantSpawnButton;

    private ARRaycastManager arRaycastManager;
    private ARAnchorManager arAnchorManager;
    private GameObject spawnedPortal;
    private GameObject spawnedSparks;

    private float searchTimer = 0f;
    public float searchTimeLimit = 10f;

    // The static list is now declared here
    private static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    void Start()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
        arAnchorManager = GetComponent<ARAnchorManager>();
        instantSpawnButton.gameObject.SetActive(false);
        statusText.text = "Searching for planes...";

        StartCoroutine(CheckForPlanes());
    }

    IEnumerator CheckForPlanes()
    {
        while (searchTimer < searchTimeLimit)
        {
            searchTimer += Time.deltaTime;
            yield return null;
        }

        if (spawnedPortal == null)
        {
            statusText.text = "Could not find a plane. Tap to spawn anyway.";
            instantSpawnButton.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        if (spawnedPortal != null)
        {
            return;
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (arRaycastManager.Raycast(Input.GetTouch(0).position, s_Hits, TrackableType.PlaneWithinPolygon))
            {
                if (s_Hits[0].trackable is ARPlane arPlane && arPlane.alignment == PlaneAlignment.HorizontalUp)
                {
                    Pose hitPose = s_Hits[0].pose;
                    if (spawnedSparks == null)
                    {
                        spawnedSparks = Instantiate(sparksPrefab, hitPose.position, hitPose.rotation);
                        statusText.text = "Plane found! Tap the sparks to spawn the portal.";
                    }
                    else
                    {
                        spawnedSparks.transform.position = hitPose.position;
                        spawnedSparks.transform.rotation = hitPose.rotation;
                    }
                    SpawnPortal(hitPose);
                }
                else
                {
                    DestroySparks();
                }
            }
            else
            {
                DestroySparks();
            }
        }
    }

    public void OnInstantSpawnButton()
    {
        Vector3 spawnPosition = Camera.main.transform.position + Camera.main.transform.forward * 3f;
        SpawnPortal(new Pose(spawnPosition, Quaternion.identity));
    }

    void SpawnPortal(Pose pose)
    {
        if (spawnedPortal == null)
        {
            // ✅ This is the corrected line that attaches the anchor using the ARPlane
            if (s_Hits.Count > 0 && s_Hits[0].trackable is ARPlane arPlane)
            {
                ARAnchor anchor = arAnchorManager.AttachAnchor(arPlane, pose);
                spawnedPortal = Instantiate(portalPrefab, anchor.transform);
            }
            else
            {
                // Fallback if no plane is found
                spawnedPortal = Instantiate(portalPrefab, pose.position, pose.rotation);
            }

            if (spawnedSparks != null)
            {
                Destroy(spawnedSparks);
            }

            statusText.text = "Portal has appeared!";
        }
    }

    void DestroySparks()
    {
        if (spawnedSparks != null)
        {
            Destroy(spawnedSparks);
        }
    }
}
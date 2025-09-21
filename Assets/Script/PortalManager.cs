using UnityEngine;
using TMPro;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class ARPortalManager : MonoBehaviour
{
    public GameObject portalPrefab;
    public TMP_Text statusText;   // ✅ now using TMP_Text
    public float portalSearchTime = 10f;

    private GameObject portalInstance;
    private ARRaycastManager raycastManager;
    private bool portalPlaced = false;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Start()
    {
        raycastManager = FindObjectOfType<ARRaycastManager>();
        StartCoroutine(PortalSequence());
    }

    IEnumerator PortalSequence()
    {
        statusText.text = "Searching for a portal...";
        yield return new WaitForSeconds(portalSearchTime);

        statusText.text = "Tap on a surface to place the portal!";
    }

    void Update()
    {
        if (!portalPlaced && statusText.text.Contains("Tap") && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;

                portalInstance = Instantiate(portalPrefab, hitPose.position, hitPose.rotation);
                portalPlaced = true;

                statusText.text = "A portal has opened nearby!";
            }
        }

        // Check if player moved close enough to the portal
        if (portalInstance != null)
        {
            float distance = Vector3.Distance(Camera.main.transform.position, portalInstance.transform.position);

            if (distance < 2f) // within 2 meters
            {
                EnterPortal();
            }
        }
    }

    void EnterPortal()
    {
        statusText.text = "Entering the portal...";
        StartCoroutine(LoadNextScene());
    }

    IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("BattleScene"); // change to your scene
    }
}

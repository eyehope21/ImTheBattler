using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlacementController : MonoBehaviour
{
    [Header("AR References")]
    // The ARRaycastManager is no longer needed for forced placement, but we'll keep the reference for clarity.
    public ARRaycastManager raycastManager;

    // Assign your NoviceDungeonManager here
    public NoviceDungeonManager dungeonManager;

    // The distance (in meters) in front of the camera to place the enemy root
    public float placementDistance = 1.5f;

    private bool isPlaced = false;

    void Start()
    {
        // Start a coroutine to wait for the AR camera to be ready
        StartCoroutine(ForcePlacementAfterARLoad());
    }

    IEnumerator ForcePlacementAfterARLoad()
    {
        yield return new WaitForSeconds(0.1f); 
        if (isPlaced) yield break;

        Camera mainCam = Camera.main;

        if (mainCam == null)
        {
            Debug.LogError("AR Main Camera not found for forced placement! Check camera tag.");
            yield break;
        }

        // 2. Calculate a position 1.5 meters in front of the AR Camera.
        // The 'transform' here refers to the AR Dungeon Root GameObject this script is attached to.
        Vector3 placementPosition = mainCam.transform.position + mainCam.transform.forward * placementDistance;

        // 3. Set the ARDungeonRoot's position and rotation
        transform.position = placementPosition;

        // Rotate the dungeon root to face the player (only yaw, keep it upright)
        Quaternion targetRotation = Quaternion.Euler(0, mainCam.transform.eulerAngles.y, 0);
        transform.rotation = targetRotation;

        // 4. Mark as placed and start the game
        isPlaced = true;

        Debug.Log("Dungeon Forcibly Placed 1.5m in front of the camera.");
    }
}
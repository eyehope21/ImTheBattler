using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlacementController : MonoBehaviour
{
    [Header("AR References")]
    // Assign the ARRaycastManager component from the Inspector
    public ARRaycastManager raycastManager;

    // Assign your NoviceDungeonManager here
    public NoviceDungeonManager dungeonManager;

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private bool isPlaced = false;

    void Update()
    {
        // Don't check for placement if the dungeon is already placed
        if (isPlaced) return;

        // Check for a touch on the screen to initiate placement
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            AttemptToPlaceDungeon(Input.GetTouch(0).position);
        }
    }

    void AttemptToPlaceDungeon(Vector2 screenPosition)
    {
        // 1. Raycast the screen position against detected planes
        if (raycastManager.Raycast(screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            // Found a surface! Get the position and rotation (Pose)
            Pose hitPose = hits[0].pose;

            // 2. Set the ARDungeonRoot's position and rotation
            // The ARDungeonRoot IS the tracked object, we move its Transform
            transform.position = hitPose.position;
            transform.rotation = hitPose.rotation;

            // 3. Mark as placed and start the game
            isPlaced = true;

            // Start the dungeon/game sequence now that it's grounded
            // Assuming your Dungeon Manager starts the game flow (spawning enemies, etc.)
            dungeonManager.StartDungeon();

            Debug.Log("Dungeon Placed successfully in AR!");

            // Optionally disable plane visualization after placement
            // FindObjectOfType<ARPlaneManager>().enabled = false;
        }
    }
}
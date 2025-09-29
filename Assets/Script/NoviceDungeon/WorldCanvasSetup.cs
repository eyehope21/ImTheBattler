using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCanvasSetup : MonoBehaviour
{
    // Call the assignment function a short time after the AR system starts
    void Start()
    {
        // Use Invoke to delay the assignment, giving the AR session time to initialize
        // and tag the camera correctly. 0.5 seconds is usually plenty.
        Invoke("AssignCameraToCanvas", 0.5f);
    }

    void AssignCameraToCanvas()
    {
        // 1. Get the Canvas component on this GameObject.
        Canvas canvas = GetComponent<Canvas>();

        if (canvas != null && canvas.renderMode == RenderMode.WorldSpace)
        {
            // 2. Find the Main Camera using the Tag.
            Camera mainCam = Camera.main;

            if (mainCam != null)
            {
                // 3. Assign the camera to the Canvas's worldCamera property (the Event Camera slot).
                canvas.worldCamera = mainCam;
                Debug.Log($"Successfully assigned {mainCam.name} to World Space Canvas on {gameObject.name}.");
            }
            else
            {
                // This will log if the camera tag is wrong or the AR session is extremely slow.
                Debug.LogError("AR Main Camera not found! Check camera tag is set to 'MainCamera'.");
            }
        }
        else if (canvas == null)
        {
            Debug.LogError($"Canvas component missing on {gameObject.name}.");
        }
    }
}
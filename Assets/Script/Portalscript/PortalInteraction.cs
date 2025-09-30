using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalInteraction : MonoBehaviour
{
    public string nextSceneName = "NoviceDungeon";

    void Update()
    {
        // Check for a tap on the screen
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // Cast a ray from the camera to the tap position
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position), Vector2.zero);

            // Check if the ray hit the portal
            if (hit.collider != null && hit.collider.CompareTag("Portal"))
            {
                Debug.Log("Portal tapped! Loading next scene...");
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }
}


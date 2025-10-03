using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections; // <-- Added this

public class AnchorController : MonoBehaviour
{
    private ARAnchor anchor;

    // Call this method once you are ready to fix the background's position
    public void SetAnchorAtCurrentPosition()
    {
        // Start the safe anchoring process
        StartCoroutine(SafelyAddAnchor());
    }

    private IEnumerator SafelyAddAnchor()
    {
        // 1. Wait one frame to ensure all other AR components have finished their Awake/Start calls.
        yield return null;

        // 2. Add the ARAnchor component only if it hasn't been destroyed in the meantime
        if (this != null && gameObject != null)
        {
            if (anchor != null)
            {
                Destroy(anchor);
            }

            // We do not need to explicitly check for ARAnchorManager, as AR Foundation 
            // usually handles finding it when the component is added. Waiting a frame 
            // is usually sufficient for manager initialization.
            anchor = gameObject.AddComponent<ARAnchor>();
            Debug.Log("AR Background successfully anchored.");
        }
    }
}
using UnityEngine;

[DisallowMultipleComponent]
public class Totem : MonoBehaviour
{
    [Header("Identity (shown in MessageText)")]
    public string totemName = "Totem";
    [TextArea(2,4)]
    public string description = "Short description of what this totem represents.";

    [Header("Puzzle")]
    public bool isCorrect = false;          // mark true for Assembly/Binary totems

    [Header("Visuals")]
    public GameObject glowObject;           // drag the child "Glow" here (optional)

    // Called by PuzzleManager to toggle the glow
    public void SetGlow(bool state)
    {
        if (glowObject != null)
        {
            glowObject.SetActive(state);
            return;
        }

        // fallback: try to find a child named "Glow" (if not assigned)
        Transform g = transform.Find("Glow");
        if (g != null) g.gameObject.SetActive(state);
    }

    // Clicking: notify PuzzleManager (requires a Collider on this GameObject)
    private void OnMouseDown()
    {
        PuzzleManager mgr = FindObjectOfType<PuzzleManager>();
        if (mgr != null)
        {
            mgr.OnTotemChosen(this);
        }
        else
        {
            Debug.LogWarning("PuzzleManager not found in scene. Make sure a PuzzleManager exists.");
        }
    }

    // Utility: optional, used by editor or other scripts
    public void ResetTotem()
    {
        SetGlow(false);
    }
}

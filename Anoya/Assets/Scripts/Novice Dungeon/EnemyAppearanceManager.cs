using UnityEngine;
using UnityEngine.UI;

public class EnemyAppearanceManager : MonoBehaviour
{
    // The Image components of all the possible enemy appearances.
    // Assign these in the Unity Inspector by dragging the child Image GameObjects.
    public Image[] enemyImages;

    // This method will be called to set a random enemy appearance
    public void SetRandomAppearance()
    {
        if (enemyImages.Length > 0)
        {
            // First, deactivate all image GameObjects to ensure a clean slate.
            foreach (Image img in enemyImages)
            {
                img.gameObject.SetActive(false);
            }

            // Pick a random index from the array.
            int randomIndex = Random.Range(0, enemyImages.Length);

            // Activate the GameObject of the randomly chosen image.
            enemyImages[randomIndex].gameObject.SetActive(true);
        }
    }
}
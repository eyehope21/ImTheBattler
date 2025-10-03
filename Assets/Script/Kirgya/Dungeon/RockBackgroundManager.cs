using System.Collections;
// RockBackgroundManager.cs (Attach this to the AR_Background_Rocks parent prefab)

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RockBackgroundManager : MonoBehaviour
{
    [Header("Configuration")]
    public int numberOfRocks = 6;

    [Tooltip("Drag all 6 rock sprites here (Rocks 1-6)")]
    public Sprite[] availableSprites;

    [Tooltip("The template for the rock sprites (a child with SpriteRenderer)")]
    public GameObject spriteTemplatePrefab;

    [Tooltip("Area around the center to randomly place the rocks")]
    public Vector2 placementSpread = new Vector2(0.3f, 0.2f);

    private void Start()
    {
        if (availableSprites == null || availableSprites.Length == 0)
        {
            Debug.LogError("Rock sprites not assigned! Cannot spawn background.");
            return;
        }

        SpawnRandomRocks();
    }

    private void SpawnRandomRocks()
    {
        if (spriteTemplatePrefab == null)
        {
            Debug.LogError("Sprite Template Prefab is null. Please assign a basic SpriteRenderer child.");
            return;
        }

        // Get a shuffled list of all available sprites (to ensure all 6 are used)
        List<Sprite> spritesToUse = availableSprites.OrderBy(x => Random.value).ToList();

        for (int i = 0; i < Mathf.Min(numberOfRocks, spritesToUse.Count); i++)
        {
            // 1. Instantiate the sprite template as a child of this object
            GameObject rockObject = Instantiate(
                spriteTemplatePrefab,
                transform.position,
                Quaternion.identity,
                transform // Make it a child of the AR_Background_Rocks anchor
            );

            // 2. Randomize the position relative to the anchor's center (0,0,0)
            float randomX = Random.Range(-placementSpread.x / 2f, placementSpread.x / 2f);
            float randomY = Random.Range(-placementSpread.y / 2f, placementSpread.y / 2f);

            // Note: Z position is already set by the Dungeon Manager on the parent.
            rockObject.transform.localPosition = new Vector3(randomX, randomY, 0);

            // 3. Randomize rotation for visual variance
            rockObject.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));

            // 4. Assign the sprite
            SpriteRenderer sr = rockObject.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = spritesToUse[i];

                // Set the render order to ensure it's behind the enemy
                // The Z-depth already handles the main front/back separation, 
                // but this helps if the enemy also uses Sorting Layers/Order.
                sr.sortingOrder = -5; // A low number puts it further back visually
            }
        }
    }
}
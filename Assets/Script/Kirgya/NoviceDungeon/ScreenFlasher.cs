using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFlasher : MonoBehaviour
{
    // Assign the DamageFlashPanel UI Image here
    [SerializeField] private Image flashImage;

    [Header("Flash Settings")]
    [Tooltip("The max opacity (alpha) of the red flash.")]
    [Range(0.0f, 1.0f)]
    [SerializeField] private float maxOpacity = 0.5f;

    [Tooltip("How long the flash takes to fade out (in seconds).")]
    [SerializeField] private float fadeTime = 0.2f;

    private Coroutine flashCoroutine;

    // Call this method from the PlayerStats script when damage is taken
    public void FlashScreen()
    {
        if (flashImage == null)
        {
            Debug.LogError("Flash Image is not assigned in the Screen Flasher!");
            return;
        }

        // Stop any ongoing flash before starting a new one
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = StartCoroutine(DoFlash());
    }

    private IEnumerator DoFlash()
    {
        // 1. Instantly set the full flash color
        Color startColor = flashImage.color;
        startColor.a = maxOpacity;
        flashImage.color = startColor;

        float timer = 0f;

        // 2. Fade the image back to transparent (alpha 0)
        while (timer < fadeTime)
        {
            timer += Time.deltaTime;

            // Calculate the new alpha value, moving from maxOpacity down to 0
            float newAlpha = Mathf.Lerp(maxOpacity, 0f, timer / fadeTime);

            Color currentColor = flashImage.color;
            currentColor.a = newAlpha;
            flashImage.color = currentColor;

            yield return null;
        }

        // 3. Ensure the image is completely transparent at the end
        Color finalColor = flashImage.color;
        finalColor.a = 0f;
        flashImage.color = finalColor;

        flashCoroutine = null;
    }
}
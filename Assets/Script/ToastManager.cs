using UnityEngine;
using TMPro;
using System.Collections;

public class ToastManager : MonoBehaviour
{
    public static ToastManager Instance;

    public TMP_Text toastText;
    public CanvasGroup canvasGroup;
    public float fadeDuration = 0.5f;
    public float displayDuration = 3f; // ✅ Set to 3 seconds

    private Coroutine currentToast;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        canvasGroup.alpha = 0;
    }

    public void ShowToast(string message)
    {
        if (currentToast != null) StopCoroutine(currentToast);
        currentToast = StartCoroutine(FadeToast(message));
    }

    private IEnumerator FadeToast(string message)
    {
        toastText.text = message;

        // Fade in
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime * (1 / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1;

        yield return new WaitForSeconds(displayDuration);

        // Fade out
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime * (1 / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0;
    }
}

using UnityEngine;
using TMPro;
using System.Collections;

public class ToastManager : MonoBehaviour
{
    public static ToastManager Instance;

    public CanvasGroup canvasGroup;
    public TMP_Text toastText;
    public float fadeDuration = 0.5f;
    public float displayDuration = 2f;

    private Coroutine currentToast;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        canvasGroup.alpha = 0f;
    }

    public void ShowToast(string message)
    {
        if (currentToast != null) StopCoroutine(currentToast);
        currentToast = StartCoroutine(FadeToast(message));
    }

    private IEnumerator FadeToast(string message)
    {
        toastText.text = message;

        while (canvasGroup.alpha < 2)
        {
            canvasGroup.alpha += Time.deltaTime / fadeDuration;
            yield return null;
        }
        canvasGroup.alpha = 1;

        yield return new WaitForSeconds(displayDuration);

        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime / fadeDuration;
            yield return null;
        }
        canvasGroup.alpha = 0;
    }
}
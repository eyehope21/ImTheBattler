using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class RegisterSuccessToast : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public TMP_Text toastText;
    public float fadeDuration = 0.5f;
    public float displayDuration = 2f;
    public string nextScene = "Login";

    private Coroutine currentToast;

    public void ShowSuccessAndLoadScene(string message)
    {
        if (currentToast != null) StopCoroutine(currentToast);
        currentToast = StartCoroutine(FadeToast(message));
    }

    private IEnumerator FadeToast(string message)
    {
        toastText.text = message;

        // Fade in
        float time = 0f;
        while (time < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, time / fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1;

        yield return new WaitForSeconds(displayDuration);

        // Fade out
        time = 0f;
        while (time < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, time / fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0;

        // Load the next scene after the toast fades out
        SceneManager.LoadScene(nextScene);
    }
}
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoginSuccessToast : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public TMP_Text toastText;
    public float fadeDuration = 0.5f;
    public float displayDuration = 2f;
    public string nextScene = "ARScene";

    public void ShowSuccessAndLoadScene(string message)
    {
        StopAllCoroutines();
        StartCoroutine(FadeToast(message));
    }

    private IEnumerator FadeToast(string message)
    {
        toastText.text = message;

        float time = 0f;
        while (time < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, time / fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1;

        yield return new WaitForSeconds(displayDuration);

        time = 0f;
        while (time < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, time / fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0;

        SceneManager.LoadScene(nextScene);
    }
}
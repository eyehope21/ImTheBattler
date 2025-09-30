using UnityEngine;
using TMPro;
using System.Collections;

public class ToastManager : MonoBehaviour
{
    public static ToastManager Instance;

    public CanvasGroup canvasGroup;
    public TMP_Text toastText;
    public float duration = 2f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowToast(string message)
    {
        StopAllCoroutines();
        StartCoroutine(ShowToastCoroutine(message));
    }

    IEnumerator ShowToastCoroutine(string message)
    {
        toastText.text = message;
        canvasGroup.alpha = 1;
        yield return new WaitForSeconds(duration);
        canvasGroup.alpha = 0;
    }
}

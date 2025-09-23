using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public TMP_Text itemText;  // assign in Inspector

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector2 originalPosition;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        originalPosition = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData) => canvasGroup.blocksRaycasts = false;
    public void OnDrag(PointerEventData eventData) => rectTransform.anchoredPosition += eventData.delta / transform.lossyScale;
    public void OnEndDrag(PointerEventData eventData) => canvasGroup.blocksRaycasts = true;

    public void ReturnToOriginal() => rectTransform.anchoredPosition = originalPosition;
}
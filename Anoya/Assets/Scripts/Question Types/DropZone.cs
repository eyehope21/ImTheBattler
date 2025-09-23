using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    public string correctAnswerID;

    public void OnDrop(PointerEventData eventData)
    {
        DraggableItem item = eventData.pointerDrag.GetComponent<DraggableItem>();
        if (item != null)
        {
            if (item.name == correctAnswerID) // correct match
            {
                item.transform.SetParent(transform);
                item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                item.GetComponent<CanvasGroup>().blocksRaycasts = false; // lock it
                Debug.Log("Correct match: " + item.name);
            }
            else
            {
                item.ReturnToOriginal();
                Debug.Log("Wrong match: " + item.name);
            }
        }
    }
}
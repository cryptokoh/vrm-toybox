using UnityEngine;
using UnityEngine.EventSystems;

public class DragWindow : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector2 clickOffset;
    private RectTransform rectTransform;
    private RectTransform canvasRectTransform;

    void Awake()
    {
        rectTransform = transform.parent.GetComponent<RectTransform>();
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            canvasRectTransform = canvas.GetComponent<RectTransform>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localCursor);
        clickOffset = rectTransform.anchoredPosition - localCursor;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localCursor))
        {
            rectTransform.anchoredPosition = localCursor + clickOffset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // You can add code here for when the user stops dragging the window
    }
}

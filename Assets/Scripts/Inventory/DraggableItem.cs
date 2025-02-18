using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private static LayerMask mask = 0;

    [Header("Item Data")]
    public InventoryItem itemData;

    public Canvas canvas;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;

    private void Awake()
    {
        if (mask == 0)
        {
            mask = LayerMask.GetMask("Placeable");
        }
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent; // Store original slot
        transform.SetParent(canvas.transform); // Move to canvas so it isn’t clipped
        canvasGroup.blocksRaycasts = false; // So that drop events can pass through
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Move the UI element with the pointer and adjust for canvas scale
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        // safety! dropped over ui object so fuck it back in the original place
        if (EventSystem.current.IsPointerOverGameObject())
        {
            if (transform.parent == canvas.transform)
            {
                transform.SetParent(originalParent);
                rectTransform.anchoredPosition = Vector2.zero;
            }
        }
        else
        {
            // Dropped in the world!
            Vector3 worldPos = Vector3.zero;
            Ray ray = Camera.main.ScreenPointToRay(eventData.position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 500f, mask))
            {
                worldPos = hit.point;
            }
            else
            {
                // If nothing was hit just fuck it back to the inv
                transform.SetParent(originalParent);
                rectTransform.anchoredPosition = Vector2.zero;
                return;
            }

            // Instantiate the plant
            if (itemData != null && itemData.plantPrefab != null)
            {
                Instantiate(itemData.plantPrefab, worldPos, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("Item data or plant prefab is missing!");
            }

            // Remove from inventory
            Destroy(gameObject);
        }
    }
}

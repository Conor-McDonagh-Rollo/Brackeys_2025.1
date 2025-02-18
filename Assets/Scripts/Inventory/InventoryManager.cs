using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform inventoryPanel; // Topmost panel
    public GameObject inventoryItemPrefab; // Slot for spawning the items inside, should have the draggable script

    [Header("Starting Items")]
    public List<InventoryItem> startingItems;

    private Canvas canvas;

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        foreach (var item in startingItems)
        {
            AddItem(item);
        }
    }

    public void AddItem(InventoryItem newItem)
    {
        GameObject newItemGO = Instantiate(inventoryItemPrefab, inventoryPanel);
        DraggableItem draggable = newItemGO.GetComponent<DraggableItem>();

        if (draggable != null)
        {
            draggable.itemData = newItem;
            draggable.canvas = canvas;
        }

        // Set the icon on the slot's image component
        Image image = newItemGO.transform.GetChild(0).GetComponent<Image>();
        if (image != null && newItem.icon != null)
        {
            image.sprite = newItem.icon;
        }
    }
}

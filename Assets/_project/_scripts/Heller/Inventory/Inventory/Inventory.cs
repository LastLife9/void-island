using System.Collections.Generic;
using UnityEngine;
public class Inventory : MonoBehaviour
{
	#region Singleton
	public static Inventory instance;
	void Awake()
	{
		if (instance != null)
		{
			Debug.LogWarning("More than one instance of Inventory found!");
			return;
		}
		instance = this;
        onItemChangedCallback += UpdateUI;    // Subscribe to the onItemChanged callback
        // Populate our slots array
        slots = itemsParent.GetComponentsInChildren<InventorySlot>();
    }
    #endregion
    public Transform itemsParent;   // The parent object of all the items
    public GameObject inventoryUI;  // The entire UI
    //Inventory inventory;    // Our current inventory
    InventorySlot[] slots;  // List of all the slots
    // Callback which is triggered when
    // an item gets added/removed.
    public delegate void OnItemChanged();
	public OnItemChanged onItemChangedCallback;
	public int space = 20;  // Amount of slots in inventory
	// Current list of items in inventory
	public List<ItemScript> items = new List<ItemScript>();
    // Add a new item. If there is enough room we
    // return true. Else we return false.
    void Start()
    {
    }
    void Update()
    {
        // Check to see if we should open/close the inventory
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryUI.SetActive(!inventoryUI.activeSelf);
        }
    }
    // Update the inventory UI by:
    //		- Adding items
    //		- Clearing empty slots
    // This is called using a delegate on the Inventory.
    void UpdateUI()
    {
        // Loop through all the slots
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < items.Count)  // If there is an item to add
            {
                slots[i].AddItem(items[i]);   // Add it
                continue;
            }
            // Otherwise clear the slot
            slots[i].ClearSlot();
        }
    }
    public void UpdateInventory(ItemScript item)
    {
        if (items.Contains(item))
        {
            if(item.count <= 0)
            {
                Remove(item);
                return;
            }
            onItemChangedCallback?.Invoke();
            return;
        }
        if (item.count <= 0)
        {
            return;
        }
        Add(item);
    }
    void Add(ItemScript item)
	{
		// Don't do anything if it's a default item
		//if (!item.isDefaultItem)
		//{
		// Check if out of space
		if (items.Count >= space)
		{
			Debug.Log("Not enough room.");
		}
		items.Add(item);    // Add item to list
		// Trigger callback
		onItemChangedCallback?.Invoke();
		//}
	}
	// Remove an item
	void Remove(ItemScript item)
	{
		items.Remove(item);     // Remove item from list
		// Trigger callback
		onItemChangedCallback?.Invoke();
	}
}
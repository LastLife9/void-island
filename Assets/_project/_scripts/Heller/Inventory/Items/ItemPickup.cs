using UnityEngine;
public class ItemPickup : Interactable
{
	public ItemScript item;   // Item to put in the inventory on pickup
	public int count;
	// When the player interacts with the item
	public override void Interact()
	{
		base.Interact();

		PickUp();   // Pick it up!
	}
	// Pick up the item
	void PickUp()
	{
		Debug.Log("Picking up " + item.name);
		ItemsManager.instance.TakeItem(item.name, count);    // Add to inventory
		// If successfully picked up
		Destroy(gameObject);    // Destroy item from scene
	}
}
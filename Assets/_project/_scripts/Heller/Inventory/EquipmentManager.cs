using UnityEngine;
/* Keep track of equipment. Has functions for adding and removing items. */
public class EquipmentManager : MonoBehaviour
{
	#region Singleton
	public static EquipmentManager instance;
	void Awake()
	{
		instance = this;
	}
	#endregion
	//public enum MeshBlendShape { Head, Torso, Pants, Legs };
	[SerializeField] Equipment[] defaultEquipment;
	EquipmentScript[] currentEquipment = new EquipmentScript[6];   // Items we currently have equipped
	// Callback for when an item is equipped/unequipped
	public delegate void OnEquipmentChanged(EquipmentScript newItem, EquipmentScript oldItem);
	public OnEquipmentChanged onEquipmentChanged;
	Inventory inventory;    // Reference to our inventory
	ItemsManager itemsManager;
	void Start()
	{
		inventory = Inventory.instance;     // Get a reference to our inventory
		itemsManager = ItemsManager.instance;
		// Initialize currentEquipment based on number of equipment slots
		//currentMeshes = new SkinnedMeshRenderer[numSlots];
		EquipDefaults();
	}
	// Equip a new item
	public void Equip(EquipmentScript newItem)
	{
		// Find out what slot the item fits in
		int slotIndex = (int)newItem.equipSlot;
		EquipmentScript oldItem = Unequip(slotIndex);
		// An item has been equipped so we trigger the callback
		onEquipmentChanged?.Invoke(newItem, oldItem);
		// Insert the item into the slot
		currentEquipment[slotIndex] = newItem;
		PlayerPrefs.SetString("Equiped" + (int)newItem.equipSlot, newItem.name);
		//AttachToMesh(newItem, slotIndex);
	}
	// Unequip an item with a particular index
	public EquipmentScript Unequip(int slotIndex)
	{
		EquipmentScript oldItem = null;
		// Only do this if an item is there
		if (currentEquipment[slotIndex] != null)
		{
			// Add the item to the inventory
			oldItem = currentEquipment[slotIndex];
			oldItem.UnequipItem();
			if (IsDefault(oldItem.name))
			{
				return null;
			}
			PlayerPrefs.SetString("Equiped" + (int)oldItem.equipSlot, "");
			//inventory.Add(oldItem);
			//SetBlendShapeWeight(oldItem, 0);
			// Destroy the mesh
			//if (currentMeshes[slotIndex] != null)
			//{
			//	Destroy(currentMeshes[slotIndex].gameObject);
			//}
			// Remove the item from the equipment array
			currentEquipment[slotIndex] = null;
			// EquipmentScript has been removed so we trigger the callback
			onEquipmentChanged?.Invoke(null, oldItem);
        }
		//EquipDefaults();
        return oldItem;
	}
	// Unequip all items
	public void UnequipAll()
	{
		for (int i = 0; i < currentEquipment.Length; i++)
		{
			Unequip(i);
		}
		//EquipDefaults();
	}
	/*
	void AttachToMesh(EquipmentScript item, int slotIndex)
	{
		SkinnedMeshRenderer newMesh = Instantiate(item.mesh) as SkinnedMeshRenderer;
		newMesh.transform.parent = targetMesh.transform.parent;
		newMesh.rootBone = targetMesh.rootBone;
		newMesh.bones = targetMesh.bones;
		currentMeshes[slotIndex] = newMesh;
		SetBlendShapeWeight(item, 100);
	}
	void SetBlendShapeWeight(EquipmentScript item, int weight)
	{
		foreach (MeshBlendShape blendshape in item.coveredMeshRegions)
		{
			int shapeIndex = (int)blendshape;
			targetMesh.SetBlendShapeWeight(shapeIndex, weight);
		}
	}
	*/
	void EquipDefaults()
	{
		EquipmentScript equipmentScript = null;
		foreach (Equipment e in defaultEquipment)
		{
            equipmentScript = itemsManager.GetEquipment(e.name);
			PlayerPrefs.SetString("Equiped" + (int)equipmentScript.equipSlot, "");
            if (!PlayerPrefs.HasKey("Equiped" + (int)equipmentScript.equipSlot) || PlayerPrefs.GetString("Equiped" + (int)equipmentScript.equipSlot) == "")
            {
                //Equip(equipmentScript);
				equipmentScript.EquipItem();
            }
        }
	}
	public bool IsDefault(string equipName)
	{
        foreach (Equipment e in defaultEquipment)
        {
            if(equipName == e.name)
			{
				return true;
			}
        }
		return false;
    }
	//void Update()
	//{
	//	// Unequip all items if we press U
	//	if (Input.GetKeyDown(KeyCode.U))
	//		UnequipAll();
	//}
}
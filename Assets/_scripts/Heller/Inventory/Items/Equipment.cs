using UnityEngine;
public enum EquipmentSlot { Head, Chest, Legs, Weapon, Shield, Feet }
/* An Item that can be equipped. */
[CreateAssetMenu(fileName = "New Equipment", menuName = "Equipment")]
public class Equipment : Item
{
	public EquipmentSlot equipSlot; // Slot to store equipment in
	public int armorModifier;       // Increase/decrease in armor
	public int damageModifier;      // Increase/decrease in damage
	public SkinnedMeshRenderer mesh;
	public EquipmentManager.MeshBlendShape[] coveredMeshRegions;
	// When pressed in inventory
	public override void Use()
	{
		base.Use();
		EquipmentManager.instance.Equip(this);  // Equip it
		RemoveFromInventory();                  // Remove it from inventory
	}
}
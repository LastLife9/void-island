/* Handles the players stats and adds/removes modifiers when equipping items. */
public class PlayerStats : CharacterStats
{
	// Use this for initialization
	void Start()
	{
		EquipmentManager.instance.onEquipmentChanged += OnEquipmentChanged;
	}
	// Called when an item gets equipped/unequipped
	void OnEquipmentChanged(EquipmentScript newItem, EquipmentScript oldItem)
	{
		// Add new modifiers
		if (newItem != null)
		{
			armor.AddModifier(newItem.armorModifier);
			damage.AddModifier(newItem.damageModifier);
		}
		// Remove old modifiers
		if (oldItem != null)
		{
			armor.RemoveModifier(oldItem.armorModifier);
			damage.RemoveModifier(oldItem.damageModifier);
		}
	}
	public override void Die()
	{
		base.Die();
		//PlayerManager.instance.KillPlayer();
	}
}
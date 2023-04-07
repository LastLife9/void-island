using UnityEngine;

public class EquipmentScript : ItemScript
{
    public EquipmentSlot equipSlot; // Slot to store equipment in
    public int armorModifier;       // Increase/decrease in armor
    public int damageModifier;      // Increase/decrease in damage
    //public EquipmentScript(Equipment item)
    //{
    //    this.name = item.name;
    //    this.icon = item.icon;
    //    if (PlayerPrefs.HasKey(name + "Count"))
    //    {
    //        count = PlayerPrefs.GetInt(name + "Count");
    //        return;
    //    }
    //    count = 0;
    //}
    public EquipmentScript(Equipment item)
    {
        name = item.name;
        icon = item.icon;
        equipSlot = item.equipSlot;
        armorModifier = item.armorModifier;
        damageModifier = item.damageModifier;
        if (PlayerPrefs.HasKey(name + "Count"))
        {
            count = PlayerPrefs.GetInt(name + "Count");
            return;
        }
        count = 0;
    }
    public override void MoveItem()
    {
        base.MoveItem();
        EquipmentManager.instance.Equip(this);  // Equip it
        RemoveFromInventory();                  // Remove it from inventory
    }
}

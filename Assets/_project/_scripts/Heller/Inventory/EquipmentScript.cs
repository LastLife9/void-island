using UnityEngine;
using UnityEngine.Events;
public class EquipmentScript : ItemScript
{
    public EquipmentSlot equipSlot; // Slot to store equipment in
    public int armorModifier;       // Increase/decrease in armor
    public int damageModifier;      // Increase/decrease in damage
    public EquipmentScript()
    {
        base.name = "";
        base.icon = null;
    }
    public EquipmentScript(Equipment item)
    {
        base.name = item.name;
        base.icon = item.icon;
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
        EquipItem();
    }
    public override void EquipItem()
    {
        base.EquipItem();
        EquipmentManager.instance.Equip(this);
        EquipmentPanel.instance.AddItemToEqiupPanel(new UnityAction(UnequipItem), this);
    }
    public override void UnequipItem()
    {
        if(EquipmentManager.instance.IsDefault(name))
        {
            return;
        }
        base.UnequipItem();
        EquipmentManager.instance.Unequip((int)equipSlot);
    }
}
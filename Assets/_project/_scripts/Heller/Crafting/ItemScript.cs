using UnityEngine;
using UnityEngine.Events;
public enum ItemType { Item, Building, Armor, Instrument, Rescources };
public enum ItemState { Null, Inventory, Equiped };
public class ItemScript
{
    public delegate void OnItemUse();
    public OnItemUse onItemUseCallback;
    public ItemType itemType;
    public ItemState itemState = ItemState.Null;
    public string name;
    public Sprite icon;
    public int count;
    //asd
    #region Contructors
    public ItemScript(Item item)
    {
        //Start();
        itemType = item.itemType;
        name = item.name;
        icon = item.icon;
        itemState = ItemState.Null;
        count = 0;
        if (PlayerPrefs.HasKey(name + "Count"))
        {
            count = PlayerPrefs.GetInt(name + "Count");
        }
        if (PlayerPrefs.HasKey(name + "State"))
        {
            if (PlayerPrefs.GetInt(name + "State") == 1)
            {
                itemState = ItemState.Inventory;
            }
            if (PlayerPrefs.GetInt(name + "State") == 2)
            {
                itemState = ItemState.Equiped;
            }
        }
    }
    public ItemScript()
    {
        //Start();
        name = "";
        icon = null;
    }
    #endregion
    public void ChangeItemCount(int newCount = 0)
    {
        count += newCount;
        PlayerPrefs.SetInt(name + "Count", count);
        if(count <= 0)
        {
            UpdateItem(ItemState.Null);
        }
        if(itemState.Equals(ItemState.Null))
        {
            UpdateItem(ItemState.Inventory);
        }
        Inventory.instance.UpdateInventory(this);
        QuickAccessScript.instance.UpdatePanel();
    }
    void UpdateItem(ItemState state)
    {
        itemState = state;
        if (itemState == ItemState.Inventory)
        {
            PlayerPrefs.SetInt(name + "State", 1);
            return;
        }
        if (itemState == ItemState.Equiped)
        {
            PlayerPrefs.SetInt(name + "State", 2);
            return;
        }
        PlayerPrefs.SetInt(name + "State", 0);
    }
    //public void RemoveFromInventory()
    //{
    //    //Inventory.instance.Remove(this);
    //}
    #region useFunctions
    public void UseFromQuickAccess()
    {
        if (itemType == ItemType.Item)
        {
            UseItem();
            return;
        }
        if (itemType == ItemType.Building)
        {
            UseBuilding();
            return;
        }
        if (itemType == ItemType.Instrument)
        {
            ChooseInstrumentInArms();
            return;
        }
    }
    public void UseFromInventory()
    {
        if (itemType == ItemType.Item)
        {
            UseItem();
            return;
        }
    }
    void UseBuilding()
    {
        HouseBuildingSystem.instance.SelectBuildingFromItem(name);
    }
    void UseItem()
    {
        ItemsManager.instance.GiveItem(name, 1);
    }
    void ChooseInstrumentInArms ()
    {

    }
    #endregion
    #region EquipItem
    public virtual void MoveItem()
    {
        // Use the item
        // Something might happen
        Debug.Log("Using " + name);
        if(itemType == ItemType.Rescources)
        {
            return;
        }
        if(itemType != ItemType.Armor)
        {
            EquipItemToQuickAccess();
            return;
        }
        EquipArmor();
    }
    public virtual void EquipArmor()
    {
        UpdateItem(ItemState.Equiped);
        Inventory.instance.UpdateInventory(this);
    }
    public virtual void UnequipArmor()
    {
        UpdateItem(ItemState.Inventory);
        Inventory.instance.UpdateInventory(this);
    }
    void EquipItemToQuickAccess()
    {
        QuickAccessScript.instance.AddItemToQuickAccess(new UnityAction(UseFromQuickAccess), this);
    }
    #endregion
}
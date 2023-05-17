using System.Collections.Generic;
using UnityEngine;
public class ItemsManager : MonoBehaviour
{
    #region Singleton
    public static ItemsManager instance;
    void Awake()
    {
        instance = this;
        BuildItemDatabase();
    }
    #endregion
    [SerializeField] GameObject craftPanel;
    List<ItemScript> itemsScript = new List<ItemScript>();
    List<EquipmentScript> equipmentsScript = new List<EquipmentScript>();
    public List<Item> items = new List<Item>();
    public List<Equipment> equipments = new List<Equipment>();
    private void Start()
    {
        foreach (var itemScript in itemsScript) 
        {
            Inventory.instance.UpdateInventory(itemScript);
            itemScript.Start();
        }
        //TakeItem("Leaf", 10);
        //TakeItem("Wood", 10);
        //TakeItem("Wall1", 100);
        //TakeItem("Floor1", 100);
        //TakeItem("Stove", 100);
    }
    public ItemScript GetItem(string name)
    {
        return itemsScript.Find(item => item.name == name);
    }
    public EquipmentScript GetEquipment(string name)
    {
        return equipmentsScript.Find(item => item.name == name);
    }
    void BuildItemDatabase()
    {
        //itemsScript = new List<ItemScript>(items.Count);
        for (int i = 0; i < equipments.Count; i++)
        {
            items.Add(equipments[i]);
            equipmentsScript.Add(new EquipmentScript(equipments[i]));
        }
        for(int i = 0; i < items.Count; i++)
        {
            itemsScript.Add(new ItemScript(items[i]));
        }
    }
    public void TakeItem(string name, int count)
    {
        GetItem(name).ChangeItemCount(count);
    }
    public void GiveItem(string name, int count)
    {
        GetItem(name).ChangeItemCount(-count);
    }
}
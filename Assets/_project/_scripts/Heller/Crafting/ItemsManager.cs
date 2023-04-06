using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class ItemsManager : MonoBehaviour
{
    public static ItemsManager instance;
    [SerializeField] GameObject craftPanel;
    List<ItemScript> itemsScript = new List<ItemScript>();
    public List<Item> items = new List<Item>();
    void Awake()
    {
        instance = this;
        BuildItemDatabase();
    }
    private void Start()
    {
        foreach (var itemScript in itemsScript) 
        {
            Inventory.instance.UpdateInventory(itemScript);
        }
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.U))
        {
            craftPanel.SetActive(!craftPanel.activeInHierarchy);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TakeItem("Leaf", 1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TakeItem("Wood", 1);
        }
    }
    public ItemScript GetItem(string name)
    {
        return itemsScript.Find(item => item.name == name);
    }
    void BuildItemDatabase()
    {
        //itemsScript = new List<ItemScript>(items.Count);
        for(int i = 0; i < items.Count; i++)
        {
            itemsScript.Add(new ItemScript(items[i]));
        }
    }
    public void TakeItem(string name, int count)
    {
        ItemScript item = GetItem(name);
        item.ChangeItemCount(count);
        Inventory.instance.UpdateInventory(item);
    }
    public void GiveItem(string name, int count)
    {
        ItemScript item = GetItem(name);
        item.ChangeItemCount(-count);
        Inventory.instance.UpdateInventory(item);
    }
}
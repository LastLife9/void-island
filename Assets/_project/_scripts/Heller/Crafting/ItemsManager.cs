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
    public List<Item> items = new List<Item>();
    private void Start()
    {
        foreach (var itemScript in itemsScript) 
        {
            Inventory.instance.UpdateInventory(itemScript);
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
        GetItem(name).ChangeItemCount(count);
    }
    public void GiveItem(string name, int count)
    {
        GetItem(name).ChangeItemCount(-count);
    }
}
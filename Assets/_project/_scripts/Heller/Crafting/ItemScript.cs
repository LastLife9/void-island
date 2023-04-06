using UnityEngine;
public class ItemScript
{
    public string name;
    public Sprite icon;
    public int count;
    //public Dictionary<string, int> stats = new Dictionary<string, int>();
    public ItemScript(Item item)
    {
        name = item.name;
        icon = item.icon;
        if (PlayerPrefs.HasKey(name + "Count"))
        {
            count = PlayerPrefs.GetInt(name + "Count");
            return;
        }
        count = 0;
    }
    public ItemScript()
    {
        name = "";
        icon = null;
    }
    public void ChangeItemCount(int newCount = 0)
    {
        count += newCount;
        PlayerPrefs.SetInt(name + "Count", count);
    }
    public virtual void Use()
    {
        // Use the item
        // Something might happen
        Debug.Log("Using " + name);
    }
    public void RemoveFromInventory()
    {
        //Inventory.instance.Remove(this);
    }
}
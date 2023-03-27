using UnityEngine;
public class ItemScript
{
    public int id;
    public string name;
    public Sprite icon;
    public int count;
    //public Dictionary<string, int> stats = new Dictionary<string, int>();
    public ItemScript(Item item)
    {
        this.id = item.id;
        this.name = item.name;
        this.icon = item.icon;
        if (PlayerPrefs.HasKey(name + "Count"))
        {
            count = PlayerPrefs.GetInt(name + "Count");
            return;
        }
        count = 0;
    }
    public void ChangeItemCount(int newCount = 0)
    {
        count += newCount;
        PlayerPrefs.SetInt(name + "Count", count);
    }
}
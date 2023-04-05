using UnityEngine;
[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Item : ScriptableObject
{
    public new string name;
    public Sprite icon;
    public virtual void Use()
    {
        // Use the item
        // Something might happen
        Debug.Log("Using " + name);
    }
    public void RemoveFromInventory()
    {
        Inventory.instance.Remove(this);
    }
}
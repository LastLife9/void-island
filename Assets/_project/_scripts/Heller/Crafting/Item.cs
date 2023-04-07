using UnityEngine;
[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Item : ScriptableObject
{
    public ItemType itemType;
    public new string name;
    public Sprite icon;
}
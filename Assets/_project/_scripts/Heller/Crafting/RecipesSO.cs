using UnityEngine;
[CreateAssetMenu(fileName = "New Recipe", menuName = "Recipe")]
public class RecipesSO : ScriptableObject
{
    public new string name;
    public Item[] requiredItems;
    public Item itemToCraft;
    public float timeToCraft = .9f;
    public int[] requiredItemsCount;
    public int itemToCraftCount;
    public string description;
}
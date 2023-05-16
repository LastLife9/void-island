using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
[System.Serializable]
struct CraftItem
{
    public TextMeshProUGUI recipeName;
    public Image recipeImage;
    public GameObject recipePanel;
    public Button recipeButton;
    public RecipesSO recipe;
}

public class CraftListPanelScript : MonoBehaviour
{
    [SerializeField] CraftItem[] craftItems;
    private void Awake()
    {
        UpdateCraftItems();
    }
    void UpdateCraftItems()
    {
        if (craftItems.Length <= 0)
        {
            return;
        }
        for (int i = 0; i < craftItems.Length; i++)
        {
            craftItems[i].recipeName.text = craftItems[i].recipe.name;
            craftItems[i].recipeImage.sprite = craftItems[i].recipe.itemToCraft.icon;
            int num = i;
            craftItems[i].recipeButton.onClick.RemoveAllListeners();
            craftItems[i].recipeButton.onClick.AddListener(delegate { EnableRecipePanel(num); });
        }
    }
    void EnableRecipePanel(int num)
    {
        for (int i = 0; i < craftItems.Length; i++)
        {
            craftItems[i].recipePanel.SetActive(false);
        }
        craftItems[num].recipePanel.SetActive(true);
    }
}

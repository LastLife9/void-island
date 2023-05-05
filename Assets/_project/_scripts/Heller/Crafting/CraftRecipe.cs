using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
public class CraftRecipe : MonoBehaviour
{
    [Header("Object")]
    [SerializeField] Image[] itemImages;
    [SerializeField] TextMeshProUGUI[] requiredItemsCountText;
    [SerializeField] TextMeshProUGUI sliderText;
    [SerializeField] Image itemToCraftImage;
    [SerializeField] Image slider;
    [SerializeField] Button craftButton;
    [SerializeField] RecipesSO recipe;
    float timeToCraft = 1f;
    int[] requiredItemsCount;
    int itemToCraftCount;
    Item[] requiredItems;
    Item itemToCraft;
    bool isCanCraft = true;
    CraftManagerScript craftManagerScript;
    UnityAction enableCraft;
    private void Awake()
    {
        timeToCraft = recipe.timeToCraft;
        requiredItemsCount = recipe.requiredItemsCount;
        itemToCraftCount = recipe.itemToCraftCount;
        requiredItems = recipe.requiredItems;
        itemToCraft = recipe.itemToCraft;
        enableCraft = new UnityAction(EnableCraftAfterCoroutine);
    }
    private void Start()
    {
        craftManagerScript = CraftManagerScript.instance;
    }
    private void OnEnable()
    {
        EnableCraft();
    }
    void EnableCraftAfterCoroutine()
    {
        isCanCraft = true;
        EnableCraft();
    }
    void EnableCraft()
    {
        for(int i = 0; i < itemImages.Length; i++)
        {
            itemImages[i].gameObject.SetActive(false);
            requiredItemsCountText[i].gameObject.SetActive(false);
        }
        craftButton.interactable = false;
        itemToCraftImage.sprite = itemToCraft.icon;
        for (int i = 0; i < requiredItems.Length; i++)
        {
            sliderText.text = "0%";
            itemImages[i].gameObject.SetActive(true);
            requiredItemsCountText[i].gameObject.SetActive(true);
            itemImages[i].sprite = requiredItems[i].icon;
            requiredItemsCountText[i].color = Color.green;
            requiredItemsCountText[i].text = ItemsManager.instance.GetItem(requiredItems[i].name).count + "/" + requiredItemsCount[i];
            if (ItemsManager.instance.GetItem(requiredItems[i].name).count
                < requiredItemsCount[i])
            {
                requiredItemsCountText[i].color = Color.red;
                isCanCraft = false;
            }
        }
        if(isCanCraft)
        {
            craftButton.interactable = true;
        }
    }
    public void CraftItem()
    {
        if(isCanCraft)
        {
            isCanCraft = false;
            craftButton.interactable = false;
            craftManagerScript.StartCraftItem(enableCraft, requiredItems,sliderText,slider,timeToCraft,itemToCraft,requiredItemsCount,itemToCraftCount);
        }
    }
}
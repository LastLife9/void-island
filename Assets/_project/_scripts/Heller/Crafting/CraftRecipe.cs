using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
    private void Awake()
    {
        timeToCraft = recipe.timeToCraft;
        requiredItemsCount = recipe.requiredItemsCount;
        itemToCraftCount = recipe.itemToCraftCount;
        requiredItems = recipe.requiredItems;
        itemToCraft = recipe.itemToCraft;
    }
    private void OnEnable()
    {
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
            sliderText.text = "0 %";
            itemImages[i].gameObject.SetActive(true);
            requiredItemsCountText[i].gameObject.SetActive(true);
            itemImages[i].sprite = requiredItems[i].icon;
            requiredItemsCountText[i].color = Color.green;
            requiredItemsCountText[i].text = ItemsManager.instance.GetItem(requiredItems[i].name).count + " / " + requiredItemsCount[i];
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
            StartCoroutine(CraftItemIE());
        }
    }
    IEnumerator CraftItemIE()
    {
        for (int i = 0; i < requiredItems.Length; i++)
        {
            ItemsManager.instance.GiveItem(requiredItems[i].name, requiredItemsCount[i]);
        }
        for (int i = 0; i < 100; i++ )
        {
            sliderText.text = slider.fillAmount * 100 + " %";
            slider.fillAmount += .05f;
            yield return new WaitForSeconds(timeToCraft/100);
        }
        ItemsManager.instance.TakeItem(itemToCraft.name, itemToCraftCount);
        slider.fillAmount = 0;
        EnableCraft();
    }
}
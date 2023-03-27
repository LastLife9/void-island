using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CraftRecipe : MonoBehaviour
{
    [Header("Object")]
    [SerializeField] Item[] requiredItems;
    [SerializeField] Image[] itemImages;
    [SerializeField] TextMeshProUGUI[] requiredItemsCountText;
    [SerializeField] Image itemToCraftImage;
    [SerializeField] Image slider;
    [SerializeField] Button craftButton;
    [Header("Parametrs")]
    [SerializeField] float timeToCraft = 1f;
    [SerializeField] int[] requiredItemsCount;
    [SerializeField] int itemToCraftCount;
    [SerializeField] Item itemToCraft;
    bool isCanCraft = true;
    public void EnableCraft()
    {
        craftButton.interactable = false;
        itemToCraftImage.sprite = itemToCraft.icon;
        for (int i = 0; i < requiredItems.Length; i++)
        {
            itemImages[i].sprite = requiredItems[i].icon;
            requiredItemsCountText[i].color = Color.green;
            if (ItemsManager.instance.GetItem(requiredItems[i].id).count < requiredItemsCount[i])
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
            slider.fillAmount += .01f;
            yield return new WaitForSeconds(timeToCraft/100);
        }
        ItemsManager.instance.GiveItem(itemToCraft.name, itemToCraftCount);
        slider.fillAmount = 0;
    }
}
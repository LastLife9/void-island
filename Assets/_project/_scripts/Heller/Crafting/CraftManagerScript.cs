using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class CraftManagerScript : MonoBehaviour
{
    #region Singleton
    public static CraftManagerScript instance;
    void Awake()
    {
        instance = this;
    }
    #endregion
    IEnumerator CraftItemIE(UnityAction unityAction, Item[] requiredItems, TextMeshProUGUI sliderText, Image slider, float timeToCraft, Item itemToCraft, int[] requiredItemsCount, int itemToCraftCount)
    {
        for (int i = 0; i < requiredItems.Length; i++)
        {
            ItemsManager.instance.GiveItem(requiredItems[i].name, requiredItemsCount[i]);
        }
        for (int i = 0; i < 100; i++)
        {
            sliderText.text = ((int)(slider.fillAmount * 100)) + "%";
            slider.fillAmount += .01f;
            yield return new WaitForSeconds(timeToCraft / 100);
        }
        ItemsManager.instance.TakeItem(itemToCraft.name, itemToCraftCount);
        slider.fillAmount = 0;
        unityAction.Invoke();
    }
    public void StartCraftItem(UnityAction unityAction, Item[] requiredItems, TextMeshProUGUI sliderText, Image slider,
        float timeToCraft, Item itemToCraft, int[] requiredItemsCount, int itemToCraftCount)
    {
        StartCoroutine(CraftItemIE(unityAction,requiredItems,sliderText,slider,timeToCraft,itemToCraft,requiredItemsCount,itemToCraftCount));
    }
}
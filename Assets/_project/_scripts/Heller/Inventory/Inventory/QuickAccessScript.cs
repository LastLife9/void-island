using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class QuickAccessScript : MonoBehaviour
{
    #region Singleton
    public static QuickAccessScript instance;
    void Awake()
    {
        instance = this;
        btnNumber = 0;
        for (int i = 0; i < btns.Length; i++)
        {
            btnsCount[i].text = "";
            inventoryPanelCount[i].text = "";
            btnsImage[i].enabled = false;
            inventoryPanelImages[i].enabled = false;
        }
        itemScripts = new ItemScript[btns.Length];
    }
    #endregion
    [SerializeField] Button[] btns;
    [SerializeField] Image[] inventoryPanelImages;
    [SerializeField] TextMeshProUGUI[] inventoryPanelCount;
    [SerializeField] TextMeshProUGUI[] btnsCount;
    [SerializeField] Image[] btnsImage;
    ItemScript[] itemScripts;
    int btnNumber = 0;
    public void AddItemToQuickAccess(UnityAction unityAction, ItemScript itemScript)
    {
        btnsImage[btnNumber].enabled = true;
        inventoryPanelImages[btnNumber].enabled = true;
        btnsImage[btnNumber].sprite = itemScript.icon;
        btnsCount[btnNumber].text = itemScript.count.ToString();
        inventoryPanelImages[btnNumber].sprite = itemScript.icon;
        inventoryPanelCount[btnNumber].text = itemScript.count.ToString();
        itemScripts[btnNumber] = itemScript;
        CheckButtonsForFunctionality().onClick.AddListener(unityAction);
    }
    Button CheckButtonsForFunctionality()
    {
        Button button = null;
        for (int i = 0; i < btns.Length; i++)
        {
            if (btns[i].onClick.GetPersistentEventCount() <= 0)
            {
                return btns[i];
            }
        }
        if(btnNumber >= btns.Length)
        {
            btnNumber = 0;
        }
        button = btns[btnNumber];
        btnNumber++;
        button.onClick.RemoveAllListeners();
        return button;
    }
    public void UpdatePanel()
    {
        for(int i = 0; i< btns.Length; i ++)
        {
            if (itemScripts[i] != null)
            {
                if(itemScripts[i].count <= 0)
                {
                    ClearPanelSlot(i);
                    continue;
                }
                ChangeCounter(i);
            }
        }
    }
    void ClearPanelSlot(int number)
    {
        btnsImage[number].enabled = false;
        inventoryPanelImages[number].enabled = false;
        btnsImage[number].sprite = null;
        btnsCount[number].text = "";
        inventoryPanelImages[number].sprite = null;
        inventoryPanelCount[number].text = "";
        itemScripts[number] = null;
        btns[number].onClick.RemoveAllListeners();
    }
    void ChangeCounter(int number)
    {
        btnsCount[number].text = itemScripts[number].count.ToString();
        inventoryPanelCount[number].text = itemScripts[number].count.ToString();
    }
}
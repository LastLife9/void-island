using TMPro;
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
    [SerializeField] Button[] inventoryPanelBtns;
    [SerializeField] Image[] inventoryPanelImages;
    [SerializeField] TextMeshProUGUI[] inventoryPanelCount;
    [SerializeField] TextMeshProUGUI[] btnsCount;
    [SerializeField] Image[] btnsImage;
    ItemScript[] itemScripts;
    int btnNumber = 0;
    public void AddItemToQuickAccess(UnityAction unityAction, UnityAction inventoryPanelUnityAction, ItemScript itemScript)
    {
        if(HasItem(itemScript.name))
        {
            return;
        }
        UpdatePanel();
        ClearPanelSlot(btnNumber);
        btnsImage[btnNumber].enabled = true;
        inventoryPanelImages[btnNumber].enabled = true;
        btnsImage[btnNumber].sprite = itemScript.icon;
        btnsCount[btnNumber].text = itemScript.count.ToString();
        inventoryPanelImages[btnNumber].sprite = itemScript.icon;
        inventoryPanelCount[btnNumber].text = itemScript.count.ToString();
        itemScripts[btnNumber] = itemScript;
        btns[btnNumber].onClick.AddListener(unityAction);
        inventoryPanelBtns[btnNumber].onClick.AddListener(inventoryPanelUnityAction);
        int num = btnNumber;
        inventoryPanelBtns[btnNumber].onClick.AddListener(delegate { ClearPanelSlot(num); });
    }
    public void UpdatePanel()
    {
        int counter = 0;
        int posibleBtnNumber = -1;
        for(int i = 0; i < btns.Length; i ++)
        {
            if (itemScripts[i] != null)
            {
                if(itemScripts[i].count <= 0)
                {
                    if (posibleBtnNumber == -1)
                    {
                        posibleBtnNumber = i;
                    }
                    ClearPanelSlot(i);
                    continue;
                }
                counter++;
                ChangeCounter(i);
                continue;
            }
            if(posibleBtnNumber == -1)
            {
                posibleBtnNumber = i;
            }
        }
        btnNumber = posibleBtnNumber;
        if(counter >= btns.Length)
        {
            btnNumber = 0;
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
        inventoryPanelBtns[number].onClick.RemoveAllListeners();
    }
    void ChangeCounter(int number)
    {
        btnsCount[number].text = itemScripts[number].count.ToString();
        inventoryPanelCount[number].text = itemScripts[number].count.ToString();
    }
    bool HasItem(string name)
    {
        for (int i = 0; i < btns.Length; i++)
        {
            if (itemScripts[i] != null && itemScripts[i].name == name)
            {
                return true;
            }
        }
        return false;
    }
}
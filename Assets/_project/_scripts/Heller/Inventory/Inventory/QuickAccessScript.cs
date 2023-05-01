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
    }
    #endregion
    [SerializeField] Button[] btns;
    [SerializeField] Image[] inventoryPanelImages;
    [SerializeField] TextMeshProUGUI[] inventoryPanelCount;
    [SerializeField] TextMeshProUGUI[] btnsCount;
    [SerializeField] Image[] btnsImage;
    int btnNumber = 0;
    private void Start()
    {
        for (int i = 0; i < btns.Length; i++)
        {
            btnsCount[i].text = "";
            inventoryPanelCount[i].text = "";
            btnsImage[i].enabled = false;
            inventoryPanelImages[i].enabled = false;
        }
    }
    public void AddItemToQuickAccess(UnityAction unityAction, ItemScript itemScript)
    {
        btnsImage[btnNumber].enabled = true;
        inventoryPanelImages[btnNumber].enabled = true;
        btnsImage[btnNumber].sprite = itemScript.icon;
        btnsCount[btnNumber].text = itemScript.count.ToString();
        inventoryPanelImages[btnNumber].sprite = itemScript.icon;
        inventoryPanelCount[btnNumber].text = itemScript.count.ToString();
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
        Debug.Log("button:" + button.name);
        btnNumber++;
        return button;
    }
}
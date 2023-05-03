using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class EquipmentPanel : MonoBehaviour
{
    public static EquipmentPanel instance;
    void Awake()
    {
        instance = this;
        for (int i = 0; i < btns.Length; i++)
        {
            btnsImage[i].enabled = false;
        }
    }
    [SerializeField] Button[] btns;
    [SerializeField] Image[] btnsImage;
    public void AddItemToEqiupPanel(UnityAction unityAction, EquipmentScript itemScript)
    {
        btnsImage[(int)itemScript.equipSlot].enabled = true;
        btnsImage[(int)itemScript.equipSlot].sprite = itemScript.icon;
        btns[(int)itemScript.equipSlot].onClick.AddListener(unityAction);
    }
}
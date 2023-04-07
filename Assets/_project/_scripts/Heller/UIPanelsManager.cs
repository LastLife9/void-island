using UnityEngine;
public class UIPanelsManager : MonoBehaviour
{
    [SerializeField] GameObject craftPanel;
    [SerializeField] GameObject inventoryUI;  // The entire UI
    [SerializeField] GameObject inGameUI;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryUI.SetActive(!inventoryUI.activeSelf);
            inGameUI.SetActive(!inGameUI.activeSelf);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            craftPanel.SetActive(!craftPanel.activeInHierarchy);
            inGameUI.SetActive(!inGameUI.activeSelf);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ItemsManager.instance.TakeItem("Leaf", 1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ItemsManager.instance.TakeItem("Wood", 1);
        }
    }
}

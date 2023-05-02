using UnityEngine;
public class UIPanelsManager : MonoBehaviour
{
    [SerializeField] GameObject craftPanel;
    [SerializeField] GameObject inventoryUI;  // The entire UI
    [SerializeField] GameObject inGameUI;
    [SerializeField] GameObject mainPanel;
    [SerializeField] GameObject playerUI;
    [SerializeField] PlayerInputState playerInputState;
    // Update is called once per frame
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.I))
    //    {
    //        inventoryUI.SetActive(!inventoryUI.activeSelf);
    //        inGameUI.SetActive(!inGameUI.activeSelf);
    //        if(inGameUI.activeSelf)
    //        {
    //            playerInputState.SetState(InputState.Main);
    //            return;
    //        }
    //        playerInputState.SetState(InputState.Lock);
    //        return;
    //    }
    //    if (Input.GetKeyDown(KeyCode.U))
    //    {
    //        craftPanel.SetActive(!craftPanel.activeInHierarchy);
    //        inGameUI.SetActive(!inGameUI.activeSelf);
    //        if (inGameUI.activeSelf)
    //        {
    //            playerInputState.SetState(InputState.Main);
    //            return;
    //        }
    //        playerInputState.SetState(InputState.Lock);
    //        return;
    //    }
    //    if (Input.GetKeyDown(KeyCode.Alpha1))
    //    {
    //        ItemsManager.instance.TakeItem("Leaf", 1);
    //        return;
    //    }
    //    if (Input.GetKeyDown(KeyCode.Alpha2))
    //    {
    //        ItemsManager.instance.TakeItem("Wood", 1);
    //        return;
    //    }
    //}
    public void EnableInventoryPanel()
    {
        mainPanel.SetActive(true);
        inventoryUI.SetActive(true);
        craftPanel.SetActive(false);
        inGameUI.SetActive(false);
        playerUI.SetActive(false);
        playerInputState.SetState(InputState.Lock);
    }
    public void EnableCraftPanel()
    {
        craftPanel.SetActive(true);
        mainPanel.SetActive(true);
        inventoryUI.SetActive(false);
        inGameUI.SetActive(false);
        playerUI.SetActive(false);
        playerInputState.SetState(InputState.Lock);
    }
    public void CloseAllPanels()
    {
        mainPanel.SetActive(false);
        playerInputState.SetState(InputState.Main);
        inGameUI.SetActive(true);
        playerUI.SetActive(true);
    }
}

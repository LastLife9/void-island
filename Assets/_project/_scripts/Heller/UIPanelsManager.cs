using UnityEngine;
public class UIPanelsManager : MonoBehaviour
{
    [SerializeField] GameObject craftPanel;
    [SerializeField] GameObject inventoryUI;  // The entire UI
    [SerializeField] GameObject inGameUI;
    [SerializeField] PlayerInputState playerInputState;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryUI.SetActive(!inventoryUI.activeSelf);
            inGameUI.SetActive(!inGameUI.activeSelf);
            if(inGameUI.activeSelf)
            {
                playerInputState.SetState(InputState.Main);
                return;
            }
            playerInputState.SetState(InputState.Lock);
            return;
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            craftPanel.SetActive(!craftPanel.activeInHierarchy);
            inGameUI.SetActive(!inGameUI.activeSelf);
            if (inGameUI.activeSelf)
            {
                playerInputState.SetState(InputState.Main);
                return;
            }
            playerInputState.SetState(InputState.Lock);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ItemsManager.instance.TakeItem("Leaf", 1);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ItemsManager.instance.TakeItem("Wood", 1);
            return;
        }
    }
}

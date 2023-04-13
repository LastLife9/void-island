using System;
using UnityEngine;

public class PlayerInputState : MonoBehaviour
{
    public static Action<InputState> OnStateChange;

    private InputState _currentState = InputState.MainWalk;

    [SerializeField] private GameObject moveInputPanel;
    [SerializeField] private GameObject flyInputPanel;

    private void Start()
    {
        SendState();
    }

    public void SetState(InputState newState)
    {
        _currentState = newState;

        UIUpdate();
        SendState();
    }

    public void SendState()
    {
        OnStateChange?.Invoke(_currentState);
    }

    private void UIUpdate()
    {
        switch (_currentState)
        {
            case InputState.MainWalk:
                DisableAllPanels();
                moveInputPanel.SetActive(true);
                break;
            case InputState.MainFly:
                DisableAllPanels();
                flyInputPanel.SetActive(true);
                break;
            case InputState.Lock:
                DisableAllPanels();
                break;
            default:
                break;
        }
    }
    private void DisableAllPanels()
    {
        moveInputPanel.SetActive(false);
        flyInputPanel.SetActive(false);
    }
}


public enum InputState
{
    MainWalk,
    MainFly,
    Lock 
}
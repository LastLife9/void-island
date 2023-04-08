using System;
using UnityEngine;

public class PlayerInputState : MonoBehaviour
{
    public static Action<InputState> OnStateChange;

    private InputState _currentState = InputState.MainMove;

    private void Start()
    {
        SendState();
    }

    //private void Update()
    //{
    //    if (Input.GetKeyUp(KeyCode.Space))
    //    {
    //        if (_currentState == InputState.Main)
    //        {
    //            SetState(InputState.Lock);
    //        }
    //        else
    //        {
    //            SetState(InputState.Main);
    //        }
    //        Debug.Log(_currentState);
    //    }
    //}

    public void SetState(InputState newState)
    {
        _currentState = newState;

        SendState();
    }

    public void SendState()
    {
        OnStateChange?.Invoke(_currentState);
    }
}


public enum InputState
{
    MainMove,
    MainFly,
    Lock 
}
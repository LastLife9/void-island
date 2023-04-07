﻿using System.Collections;
using UnityEngine;

public class VirualInputBase : MonoBehaviour
{
    protected bool _enable = true;
    protected InputState _inputState;

    protected virtual void OnEnable()
    {
        
    }

    protected virtual void OnDisable()
    {

    }

    protected virtual void OnInputStateChange(InputState newState)
    {
        _inputState = newState;

        switch (_inputState)
        {
            case InputState.Main:
                SetEnable();
                break;
            case InputState.Lock:
                SetDisable();
                break;
            default:
                break;
        }
    }

    protected virtual void SetEnable()
    {
        _enable = true;
    }

    protected virtual void SetDisable()
    {
        _enable = false;
    }
}
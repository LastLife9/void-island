using System.Collections;
using UnityEngine;

public class VirualButton : VirualInputBase
{
    [SerializeField] private CanvasGroup canvasGroup;

    protected override void OnEnable()
    {
        base.OnEnable();
        PlayerInputState.OnStateChange += OnInputStateChange;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        PlayerInputState.OnStateChange -= OnInputStateChange;
    }

    protected override void OnInputStateChange(InputState newState)
    {
        base.OnInputStateChange(newState);
    }

    protected override void SetEnable()
    {
        base.SetEnable();

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
    }

    protected override void SetDisable()
    {
        base.SetDisable();

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
    }
}
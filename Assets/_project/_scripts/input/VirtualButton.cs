using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class VirtualButton : InputBase, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }
    [System.Serializable]
    public class Event : UnityEvent { }

    [Header("Output")]
    public BoolEvent buttonStateOutputEvent;
    public Event buttonClickOutputEvent;

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

    public void OnPointerDown(PointerEventData eventData)
    {
        OutputButtonStateValue(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OutputButtonStateValue(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OutputButtonClickEvent();
    }

    void OutputButtonStateValue(bool buttonState)
    {
        buttonStateOutputEvent.Invoke(buttonState);
    }

    void OutputButtonClickEvent()
    {
        buttonClickOutputEvent.Invoke();
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
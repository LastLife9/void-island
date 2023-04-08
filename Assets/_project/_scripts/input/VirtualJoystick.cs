using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class VirtualJoystick : InputBase, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [System.Serializable]
    public class Event : UnityEvent<Vector2> { }

    [Header("Rect References")]
    public RectTransform containerRect;
    public RectTransform handleRect;

    [Header("Settings")]
    public float joystickRange = 50f;
    public float magnitudeMultiplier = 1f;
    public bool invertXOutputValue;
    public bool invertYOutputValue;

    [Header("Output")]
    public Event joystickOutputEvent;

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

        handleRect.gameObject.SetActive(true);
    }

    protected override void SetDisable()
    {
        base.SetDisable();

        UpdateHandleRectPosition(Vector2.zero);
        handleRect.gameObject.SetActive(false);
    }


    void Start()
    {
        SetupHandle();
    }

    private void SetupHandle()
    {
        if(handleRect.gameObject.activeSelf)
        {
            UpdateHandleRectPosition(Vector2.zero);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_enable) return;
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_enable)
        {
            OutputPointerEventValue(Vector2.zero);
            return;
        }
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            containerRect, eventData.position, eventData.pressEventCamera, out Vector2 position);
        
        position = ApplySizeDelta(position);

        Vector2 clampedPosition = ClampValuesToMagnitude(position);
        Vector2 outputPosition = ApplyInversionFilter(position);

        OutputPointerEventValue(outputPosition * magnitudeMultiplier);

        if (handleRect.gameObject.activeSelf)
        {
            UpdateHandleRectPosition(clampedPosition * joystickRange);
        }
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_enable)
        {
            OutputPointerEventValue(Vector2.zero);
            return;
        }
        OutputPointerEventValue(Vector2.zero);

        if(handleRect.gameObject.activeSelf)
        {
             UpdateHandleRectPosition(Vector2.zero);
        }
    }

    private void OutputPointerEventValue(Vector2 pointerPosition)
    {
        joystickOutputEvent.Invoke(ClampValuesToMagnitude(pointerPosition));
    }

    private void UpdateHandleRectPosition(Vector2 newPosition)
    {
        handleRect.anchoredPosition = newPosition;
    }

    Vector2 ApplySizeDelta(Vector2 position)
    {
        float x = (position.x/containerRect.sizeDelta.x) * 2.5f;
        float y = (position.y/containerRect.sizeDelta.y) * 2.5f;
        return new Vector2(x, y);
    }

    Vector2 ClampValuesToMagnitude(Vector2 position)
    {
        return Vector2.ClampMagnitude(position, 1);
    }

    Vector2 ApplyInversionFilter(Vector2 position)
    {
        if(invertXOutputValue)
        {
            position.x = InvertValue(position.x);
        }

        if(invertYOutputValue)
        {
            position.y = InvertValue(position.y);
        }

        return position;
    }

    float InvertValue(float value)
    {
        return -value;
    }
}
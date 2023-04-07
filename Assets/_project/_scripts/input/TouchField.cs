using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class TouchLocation
{
    public int touchId;

    public TouchLocation(int newTouchId)
    {
        touchId = newTouchId;
    }
}


public class TouchField : VirualInputBase
{
    public List<TouchLocation> touches = new List<TouchLocation>();

    [System.Serializable]
    public class Event : UnityEvent<Vector2> { }
    [Header("Output")]
    public Event touchFieldOutputEvent;
    public float Multiplier = 0.1f;

    public RectTransform containerRect;

    Vector2 pointerOld;
    Vector2 touchDist;

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
    protected override void SetDisable()
    {
        base.SetDisable();
        OutputPointerEventValue(new Vector2());
    }

    // Update is called once per frame
    void Update()
    {
        if (!_enable)
        {
            OutputPointerEventValue(new Vector2());
            return;
        }

        if (Input.touchCount == 0)
        {
            if (Input.GetMouseButton(0))
            {
                if (Input.mousePosition.x > Screen.width / 2f)
                {
                    touchDist = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - pointerOld;
                    pointerOld = Input.mousePosition;
                }
                else
                {
                    touchDist = new Vector2();
                }
            }
            else
            {
                touchDist = new Vector2();
            }
        }
        else
        {
            int i = 0;
            while (i < Input.touchCount)
            {
                Touch t = Input.GetTouch(i);
                if (t.position.x > Screen.width / 2f)
                {
                    if (t.phase == TouchPhase.Began)
                    {
                        touches.Add(new TouchLocation(t.fingerId));
                        pointerOld = t.position;
                    }
                    else if (t.phase == TouchPhase.Ended)
                    {
                        TouchLocation thisTouch = touches.Find(touchLocation => touchLocation.touchId == t.fingerId);
                        touches.RemoveAt(touches.IndexOf(thisTouch));
                        touchDist = new Vector2();
                    }
                    else if (t.phase == TouchPhase.Moved)
                    {
                        TouchLocation thisTouch = touches.Find(touchLocation => touchLocation.touchId == t.fingerId);
                        touchDist = t.position - pointerOld;
                        pointerOld = t.position;
                    }
                }
                ++i;
            }
        }
        
        OutputPointerEventValue(touchDist);
    }
    Vector2 getTouchPosition(Vector2 touchPosition)
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, transform.position.z));
    }

    void OutputPointerEventValue(Vector2 pointerPosition)
    {
        touchFieldOutputEvent.Invoke(pointerPosition * Multiplier);
    }
}
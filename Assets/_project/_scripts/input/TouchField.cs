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


public class TouchField : MonoBehaviour
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

    // Update is called once per frame
    void Update()
    {
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


//public class TouchField : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
//{
//    [System.Serializable]
//    public class Event : UnityEvent<Vector2> { }
//    [Header("Output")]
//    public Event touchFieldOutputEvent;

//    public float Multiplier = 0.3f;

//    [HideInInspector]
//    public Vector2 TouchDist;
//    [HideInInspector]
//    public Vector2 PointerOld;
//    [HideInInspector]
//    protected int PointerId;
//    [HideInInspector]
//    public bool Pressed;

//    void Update()
//    {
//        if (Pressed)
//        {
//            if (PointerId >= 0 && PointerId < Input.touches.Length)
//            {
//                TouchDist = Input.touches[PointerId].position - PointerOld;
//                PointerOld = Input.touches[PointerId].position;
//            }
//            else
//            {
//                TouchDist = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - PointerOld;
//                PointerOld = Input.mousePosition;
//            }
//        }
//        else
//        {
//            TouchDist = new Vector2();
//        }

//        OutputPointerEventValue(TouchDist);
//    }

//    public void OnPointerDown(PointerEventData eventData)
//    {
//        Pressed = true;
//        PointerId = eventData.pointerId;
//        PointerOld = eventData.position;
//    }


//    public void OnPointerUp(PointerEventData eventData)
//    {
//        Pressed = false;
//    }

//    void OutputPointerEventValue(Vector2 pointerPosition)
//    {
//        touchFieldOutputEvent.Invoke(pointerPosition * Multiplier);
//    }
//}

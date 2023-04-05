using UnityEngine;

public class ObjectDragAndRotate : MonoBehaviour
{
    private float _sensitivity;

    void Start()
    {
        _sensitivity = 0.4f;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            float XaxisRotation = Input.GetAxis("Mouse X") * _sensitivity;
            float YaxisRotation = Input.GetAxis("Mouse Y") * _sensitivity;
            Vector3 euler = new Vector3(-YaxisRotation, XaxisRotation, 0);
            transform.eulerAngles += euler;
        }
    }

    //void OnMouseDrag()
    //{
    //    float XaxisRotation = Input.GetAxis("Mouse X") * rotationSpeed;
    //    float YaxisRotation = Input.GetAxis("Mouse Y") * rotationSpeed;
    //    // select the axis by which you want to rotate the GameObject
    //    transform.Rotate(Vector3.down, XaxisRotation);
    //    transform.Rotate(Vector3.right, YaxisRotation);
    //}
}

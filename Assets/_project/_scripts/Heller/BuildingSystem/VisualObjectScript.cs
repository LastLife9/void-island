using UnityEngine;
public class VisualObjectScript : MonoBehaviour
{
    public bool isInOtherObject = false;
    private void OnTriggerEnter(Collider other)
    {
        isInOtherObject = true;
    }
    private void OnTriggerExit(Collider other)
    {
        isInOtherObject = false;
    }
}

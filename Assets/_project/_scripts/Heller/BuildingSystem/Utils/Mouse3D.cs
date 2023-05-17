using UnityEngine;
public class Mouse3D : MonoBehaviour
{
    public static Mouse3D Instance { get; private set; }
    [SerializeField] private LayerMask mouseColliderLayerMask = new LayerMask();
    [SerializeField] private Transform debugVisual;
    [SerializeField] Transform scope;
    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(scope.position);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, mouseColliderLayerMask))
        {
            transform.position = raycastHit.point;
        }
        if (debugVisual != null) debugVisual.position = GetMouseWorldPosition();
    }
    public static Vector3 GetMouseWorldPosition() => Instance.GetMouseWorldPosition_Instance();
    private Vector3 GetMouseWorldPosition_Instance()
    {
        Ray ray = Camera.main.ScreenPointToRay(scope.position);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 30f, mouseColliderLayerMask))
        {
            //if(raycastHit.transform != null)
            //{
            //    Debug.Log(raycastHit.transform.name);
            //}
            return raycastHit.point;
        }
        return Vector3.zero;
    }
}
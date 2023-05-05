using CodeMonkey.Utils;
using System.Collections.Generic;
using UnityEngine;
public class HouseBuildingGhost : MonoBehaviour 
{
    Transform visual;
    HouseBuildingSystem houseBuildingSystem;
    [SerializeField] GameObject acceptButton;
    [SerializeField] Material greenMat;
    [SerializeField] Material redMat;
    List<MeshRenderer> meshRenderers = new List<MeshRenderer>();
    private void Start() 
    {
        houseBuildingSystem = HouseBuildingSystem.instance;
        RefreshVisual();
        houseBuildingSystem.OnSelectedChanged += Instance_OnSelectedChanged;
    }
    private void Instance_OnSelectedChanged(object sender, System.EventArgs e) 
    {
        RefreshVisual();
    }
    private void LateUpdate() 
    {
        //if(UtilsClass.IsPointerOverUI())
        //{
        //    return;
        //}
        Vector3 targetPosition = Vector3.zero;
        CheckMatColor(houseBuildingSystem.HandleObjectPlacement(false));
        if (houseBuildingSystem.GetPlaceObjectType() == BuildingSystemPartType.EdgeObject)
        {
            FloorEdgePosition floorEdgePosition = houseBuildingSystem.GetMouseFloorEdgePosition();
            if (floorEdgePosition != null)
            {
                transform.position = Vector3.Lerp(transform.position, floorEdgePosition.transform.position, Time.deltaTime * 15f);
                transform.rotation = Quaternion.Lerp(transform.rotation, floorEdgePosition.transform.rotation, Time.deltaTime * 25f);
                return;
            }
            targetPosition = houseBuildingSystem.GetMouseWorldSnappedPosition();
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 15f);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime * 25f);
            return;
        }
        //if (houseBuildingSystem.GetPlaceObjectType() == BuildingSystemPartType.LooseObject)
        //{
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    if (Physics.Raycast(ray, out RaycastHit raycastHit))
        //    {
        //        transform.position = Vector3.Lerp(transform.position, raycastHit.point, Time.deltaTime * 15f);
        //        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, houseBuildingSystem.GetLooseObjectEulerY(), 0), Time.deltaTime * 25f);
        //    }
        //    return;
        //}
        if (houseBuildingSystem.GetPlaceObjectType() == BuildingSystemPartType.BaseObject)
        {
            targetPosition = houseBuildingSystem.GetMouseWorldSnappedPosition();
            //targetPosition.y += .1f;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 15f);
            transform.rotation = Quaternion.Lerp(transform.rotation, houseBuildingSystem.GetPlacedObjectRotation(), Time.deltaTime * 25f);
            return;
        }
    }
    private void CheckMatColor(bool isCanPlace)
    {
        if(meshRenderers.Count <= 0)
        {
            acceptButton.SetActive(false);
            return;
        }
        acceptButton.SetActive(isCanPlace);
        if (isCanPlace)
        {
            foreach(MeshRenderer meshRenderer in meshRenderers)
            {
                meshRenderer.sharedMaterial = greenMat;
            }
            return;
        }
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            meshRenderer.sharedMaterial = redMat;
        }
    }
    private void RefreshVisual()
    {
        if (visual != null)
        {
            Destroy(visual.gameObject);
            visual = null;
            meshRenderers.Clear();
        }
        BuildingSystemPartSO placedObjectTypeSO = null;
        placedObjectTypeSO = houseBuildingSystem.GetBuildingSystemPartSO();
        if (placedObjectTypeSO == null)
        {
            return;
        }
        visual = Instantiate(placedObjectTypeSO.visual, Vector3.zero, Quaternion.identity);
        visual.parent = transform;
        visual.localPosition = Vector3.zero;
        visual.localEulerAngles = Vector3.zero;
        if(visual.TryGetComponent<MeshRenderer>(out MeshRenderer meshRenderer))
        {
            meshRenderers.Add(meshRenderer);
        }
        SetLayerRecursive(visual.gameObject, 14);
    }
    private void SetLayerRecursive(GameObject targetGameObject, int layer) 
    {
        targetGameObject.layer = layer;
        foreach (Transform child in targetGameObject.transform) 
        {
            SetLayerRecursive(child.gameObject, layer);
            if (child.TryGetComponent<MeshRenderer>(out MeshRenderer meshRenderer))
            {
                meshRenderers.Add(meshRenderer);
            }
        }
    }
}
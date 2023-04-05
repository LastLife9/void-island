using System.Collections.Generic;
using UnityEngine;
public class PlacedObject : MonoBehaviour
{
    [System.Serializable]
    public class SaveObject
    {
        public string placedObjectTypeSOName;
        public Vector2Int origin;
        public BuildingSystemPartSO.Dir dir;
        public string floorPlacedObjectSave;
    }
    private BuildingSystemPartSO placedObjectTypeSO;
    private Vector2Int origin;
    private BuildingSystemPartSO.Dir dir;
    public static PlacedObject Create(Vector3 worldPosition, Vector2Int origin, BuildingSystemPartSO.Dir dir, BuildingSystemPartSO placedObjectTypeSO)
    {
        Transform placedObjectTransform = Instantiate(placedObjectTypeSO.prefab, worldPosition, Quaternion.Euler(0, placedObjectTypeSO.GetRotationAngle(dir), 0));
        PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();
        placedObject.placedObjectTypeSO = placedObjectTypeSO;
        placedObject.origin = origin;
        placedObject.dir = dir;
        placedObject.Setup();
        return placedObject;
    }
    protected virtual void Setup()
    {
        //Debug.Log("PlacedObject.Setup() " + transform);
    }
    public virtual void GridSetupDone()
    {
        //Debug.Log("PlacedObject.GridSetupDone() " + transform);
    }
    public Vector2Int GetGridPosition()
    {
        return origin;
    }
    public List<Vector2Int> GetGridPositionList()
    {
        return placedObjectTypeSO.GetGridPositionList(origin, dir);
    }
    public virtual void DestroySelf()
    {
        Destroy(gameObject);
    }
    public override string ToString()
    {
        return placedObjectTypeSO.nameString;
    }
    public SaveObject GetSaveObject()
    {
        return new SaveObject
        {
            placedObjectTypeSOName = placedObjectTypeSO.name,
            origin = origin,
            dir = dir,
            floorPlacedObjectSave = (this is FloorPlacedObject) ? ((FloorPlacedObject)this).Save() : "",
        };
    }
}
using UnityEngine;
public class FloorEdgePlacedObject : MonoBehaviour
{
    [SerializeField] private BuildingSystemPartSO floorEdgeObjectTypeSO;
    public string GetFloorEdgeObjectTypeSOName()
    {
        return floorEdgeObjectTypeSO.name;
    }
}
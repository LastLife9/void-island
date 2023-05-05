using System;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using TMPro;
public enum BuildingSystemPartType
{
    BaseObject,
    EdgeObject,
    LooseObject,
}
public class HouseBuildingSystem : MonoBehaviour
{
    public class GridObject
    {
        private GridXZ<GridObject> grid;
        private int x;
        private int y;
        public PlacedObject placedObject;
        public GridObject(GridXZ<GridObject> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
            placedObject = null;
        }
        public override string ToString()
        {
            return x + ", " + y + "\n" + placedObject;
        }
        public void TriggerGridObjectChanged()
        {
            grid.TriggerGridObjectChanged(x, y);
        }
        public void SetPlacedObject(PlacedObject placedObject)
        {
            this.placedObject = placedObject;
            TriggerGridObjectChanged();
        }
        public void ClearPlacedObject()
        {
            placedObject = null;
            TriggerGridObjectChanged();
        }
        public PlacedObject GetPlacedObject()
        {
            return placedObject;
        }
        public bool CanBuild()
        {
            return placedObject == null;
        }
    }
    [Serializable]
    public class SaveObject
    {
        public PlacedObjectSaveObjectArray[] placedObjectSaveObjectArrayArray;
        public LooseSaveObject[] looseSaveObjectArray;
    }
    [Serializable]
    public class PlacedObjectSaveObjectArray
    {
        public PlacedObject.SaveObject[] placedObjectSaveObjectArray;
    }
    [Serializable]
    public class LooseSaveObject
    {
        public string looseObjectSOName;
        public Vector3 position;
        public Quaternion quaternion;
    }
    public static HouseBuildingSystem instance { get; private set; }
    public event EventHandler OnActiveGridLevelChanged;
    public event EventHandler OnSelectedChanged;
    //public event EventHandler OnObjectPlaced;
    public float maxBuildDistance = 10f;
    [SerializeField] Transform playerTransform;
    [SerializeField] LayerMask objectEdgeColliderLayerMask;
    [SerializeField] List<BuildingSystemPartSO> buildingSystemPartSOList = null;
    [SerializeField] TextMeshProUGUI textMeshPro;
    [SerializeField] Transform scope;
    Vector3 mousePosition = Vector3.zero;
    private BuildingSystemPartSO buildingSystemPartSO;
    private float looseObjectEulerY;
    private List<GridXZ<GridObject>> gridList;
    private GridXZ<GridObject> selectedGrid;
    private BuildingSystemPartType buildingSystemPartType;
    private BuildingSystemPartSO.Dir dir;
    private bool isDemolishActive;
    private const float GRID_HEIGHT = 2.5f;
    private void Awake()
    {
        instance = this;
        //Application.targetFrameRate = 100;
        int gridWidth = 100;
        int gridHeight = 100;
        float cellSize = 2f;
        gridList = new List<GridXZ<GridObject>>();
        int gridVerticalCount = 5;
        float gridVerticalSize = GRID_HEIGHT;
        for (int i = 0; i < gridVerticalCount; i++)
        {
            GridXZ<GridObject> grid = new GridXZ<GridObject>(gridWidth, gridHeight, cellSize, new Vector3(0, gridVerticalSize * i, 0), (GridXZ<GridObject> g, int x, int y) => new GridObject(g, x, y));
            gridList.Add(grid);
        }
        selectedGrid = gridList[0];
        buildingSystemPartSO = null;
        //looseObjectTransformList = new List<Transform>();
        //textMeshPro = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        textMeshPro.text = "Grid Level " + 1;
        OnActiveGridLevelChanged += HouseBuildingSystem_OnActiveGridLevelChanged;
    }
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Alpha1)) SelectBuildingSystemPartSO(buildingSystemPartSOList[0]);
        //if (Input.GetKeyDown(KeyCode.Alpha2)) SelectBuildingSystemPartSO(buildingSystemPartSOList[1]);
        //if (Input.GetKeyDown(KeyCode.Alpha3)) SelectBuildingSystemPartSO(buildingSystemPartSOList[2]);
        //if (Input.GetKeyDown(KeyCode.Alpha4)) SelectBuildingSystemPartSO(buildingSystemPartSOList[3]);
        //if (Input.GetKeyDown(KeyCode.Alpha5)) SelectBuildingSystemPartSO(buildingSystemPartSOList[4]);
        //HandleGridSelectManual();
        HandleGridSelectAutomatic();
        //HandleTypeSelect();
        //HandleObjectPlacement();
        //HandleDirRotation();
        //HandleDemolish();
        //if (Input.GetMouseButtonDown(1))
        //{
        //    DeselectObjectType();
        //}
        //if (Input.GetKeyDown(KeyCode.O))
        //{
        //    Save();
        //}
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    Load();
        //}
    }
    #region UpdatedFunc
    public BuildingSystemPartSO GetPlacedObjectTypeSOFromName(string placedObjectTypeSOName)
    {
        foreach (BuildingSystemPartSO buildingSystemPartSO in buildingSystemPartSOList)
        {
            if (buildingSystemPartSO.name == placedObjectTypeSOName)
            {
                return buildingSystemPartSO;
            }
        }
        return null;
    }
    private void HouseBuildingSystem_OnActiveGridLevelChanged(object sender, System.EventArgs e)
    {
        textMeshPro.text = "Grid Level " + (HouseBuildingSystem.instance.GetActiveGridLevel() + 1);
    }
    public BuildingSystemPartSO GetBuildingSystemPartSO()
    {
        return buildingSystemPartSO;
    }
    public void SelectBuildingSystemPartSO(BuildingSystemPartSO placedObjectTypeSO)
    {
        buildingSystemPartType = placedObjectTypeSO.buildingSystemPartType;
        this.buildingSystemPartSO = placedObjectTypeSO;
        RefreshSelectedObjectType();
    }
    public float GetLooseObjectEulerY()
    {
        return looseObjectEulerY;
    }
    #endregion
    #region functionalCode
    public void AcceptBuilding()
    {
        HandleObjectPlacement(true);
    }
    public void SelectBuildingFromItem(string itemName)
    {
        for(int i = 0; i < buildingSystemPartSOList.Count; i++)
        {
            if (buildingSystemPartSOList[i].nameString == itemName)
            {
                SelectBuildingSystemPartSO(buildingSystemPartSOList[i]);
                return;
            }
        }        
    }
    private void HandleGridSelectAutomatic()
    {
        Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
        float gridHeight = GRID_HEIGHT;
        int newGridIndex = Mathf.Clamp(Mathf.RoundToInt(mousePosition.y / gridHeight), 0, gridList.Count - 1);
        selectedGrid = gridList[newGridIndex];
        OnActiveGridLevelChanged?.Invoke(this, EventArgs.Empty);
    }
    private void HandleGridSelectManual()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            int nextSelectedGridIndex = (gridList.IndexOf(selectedGrid) + 1) % gridList.Count;
            selectedGrid = gridList[nextSelectedGridIndex];
            OnActiveGridLevelChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    private void HandleTypeSelect()
    {
        //if (Input.GetKeyDown(KeyCode.Z))
        //{
        //    buildingSystemPartType = BuildingSystemPartType.BaseObject;
        //    buildingSystemPartSO = placedObjectTypeSOList[0];
        //    RefreshSelectedObjectType();
        //}
        if (Input.GetKeyDown(KeyCode.X)) { SetDemolishActive(); }
    }
    private void HandleDirRotation()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            dir = BuildingSystemPartSO.GetNextDir(dir);
            //    looseObjectEulerY += Time.deltaTime * 90f;
        }
    }
    private void HandleDemolish()
    {
        if (isDemolishActive && Input.GetMouseButtonDown(0) /*&& !UtilsClass.IsPointerOverUI()*/)
        {
            Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
            PlacedObject placedObject = selectedGrid.GetGridObject(mousePosition).GetPlacedObject();
            if (placedObject != null)
            {
                // Demolish
                placedObject.DestroySelf();
                List<Vector2Int> gridPositionList = placedObject.GetGridPositionList();
                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    selectedGrid.GetGridObject(gridPosition.x, gridPosition.y).ClearPlacedObject();
                }
            }
        }
    }
    private void DeselectObjectType()
    {
        buildingSystemPartSO = null;
        //floorEdgeObjectTypeSO = null;
        //looseObjectSO = null;
        isDemolishActive = false;
        RefreshSelectedObjectType();
    }
    private void RefreshSelectedObjectType()
    {
        OnSelectedChanged?.Invoke(this, EventArgs.Empty);
    }
    public bool IsDemolishActive()
    {
        return isDemolishActive;
    }
    public BuildingSystemPartType GetPlaceObjectType()
    {
        return buildingSystemPartType;
    }
    public int GetActiveGridLevel()
    {
        return gridList.IndexOf(selectedGrid);
    }
    public Quaternion GetPlacedObjectRotation()
    {
        if (buildingSystemPartSO != null)
        {
            return Quaternion.Euler(0, buildingSystemPartSO.GetRotationAngle(dir), 0);
        }
        return Quaternion.identity;
    }
    public void SetDemolishActive()
    {
        //placedObjectTypeSO = null;
        isDemolishActive = !isDemolishActive;
        //RefreshSelectedObjectType();
    }
    public FloorEdgePosition GetMouseFloorEdgePosition()
    {
        //if (!UtilsClass.IsPointerOverUI())
        {
            Ray ray = Camera.main.ScreenPointToRay(scope.position);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, objectEdgeColliderLayerMask))
            {
                // Raycast Hit Edge Object
                if (raycastHit.collider.TryGetComponent(out FloorEdgePosition floorEdgePosition))
                {
                    return floorEdgePosition;
                }
            }
        }
        return null;
    }
    public Vector3 GetMouseWorldSnappedPosition()
    {
        Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
        selectedGrid.GetXZ(mousePosition, out int x, out int z);
        if (buildingSystemPartSO != null)
        {
            Vector2Int rotationOffset = buildingSystemPartSO.GetRotationOffset(dir);
            Vector3 placedObjectWorldPosition = selectedGrid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * selectedGrid.GetCellSize();
            return placedObjectWorldPosition;
        }
        return mousePosition;
    }
    private bool IsInRange(Vector3 mousePosition)
    {
        if(mousePosition == Vector3.zero)
        {
            return false;
        }
        return Vector3.Distance(playerTransform.position, mousePosition) < maxBuildDistance;
    }
    #endregion
    #region rewrite
    public bool HandleObjectPlacement(bool isBuildProccess)
    {
        if (buildingSystemPartSO != null /*&& !UtilsClass.IsPointerOverUI() &&  Input.GetMouseButton(0) */ )
        {
            if(!isBuildProccess)
            {
                mousePosition = Mouse3D.GetMouseWorldPosition();
            }
            if (IsInRange(mousePosition))
            {
                if (buildingSystemPartType == BuildingSystemPartType.BaseObject)
                {
                    selectedGrid.GetXZ(mousePosition, out int x, out int z);
                    Vector2Int placedObjectOrigin = new Vector2Int(x, z);
                    if (TryPlaceObject(isBuildProccess, placedObjectOrigin, buildingSystemPartSO, dir, out PlacedObject placedObject))
                    {
                        return true;
                        // Object placed
                    }
                    return false;
                }
                if (buildingSystemPartType == BuildingSystemPartType.EdgeObject)
                {
                    Ray ray = Camera.main.ScreenPointToRay(/*Input.mousePosition*/scope.position);
                    if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, objectEdgeColliderLayerMask))
                    {
                        // Raycast Hit Edge Object
                        if (raycastHit.collider.TryGetComponent(out FloorEdgePosition floorEdgePosition))
                        {
                            if (raycastHit.collider.transform.parent.TryGetComponent(out FloorPlacedObject floorPlacedObject))
                            {
                                // Found parent FloorPlacedObject
                                // Place Object on Edge
                                if(isBuildProccess)
                                {
                                    floorPlacedObject.PlaceEdge(floorEdgePosition.edge, buildingSystemPartSO);
                                }
                                return true;
                            }
                        }
                    }
                    return false;
                }
                return false;
            }
            //return false;
            //if (buildingSystemPartType == BuildingSystemPartType.LooseObject)
            //{
            //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //    if (Physics.Raycast(ray, out RaycastHit raycastHit))
            //    {
            //        // Raycast Hit something
            //        Transform looseObjectTransform = Instantiate(buildingSystemPartSO.prefab, raycastHit.point, Quaternion.Euler(0, looseObjectEulerY, 0));
            //        //buildingSystemPartSOList.Add(buildingSystemPartSO);
            //    }
            //    return;
            //}
        }
        return false;
    }  
    void CheckIfHaveItem()
    {
        ItemsManager.instance.GiveItem(buildingSystemPartSO.nameString, 1);
        if(ItemsManager.instance.GetItem(buildingSystemPartSO.nameString).count <= 0)
        {
            DeselectObjectType();
        }
    }
    #endregion
    #region NeedFutureUpdates
    public bool TryPlaceObject(bool buildingProccess, int x, int y, BuildingSystemPartSO placedObjectTypeSO, BuildingSystemPartSO.Dir dir)
    {
        return TryPlaceObject(buildingProccess, new Vector2Int(x, y), placedObjectTypeSO, dir, out PlacedObject placedObject);
    }
    public bool TryPlaceObject(bool buildingProccess, Vector2Int placedObjectOrigin, BuildingSystemPartSO placedObjectTypeSO, BuildingSystemPartSO.Dir dir)
    {
        return TryPlaceObject(buildingProccess, placedObjectOrigin, placedObjectTypeSO, dir, out PlacedObject placedObject);
    }
    public bool TryPlaceObject(bool buildingProccess, Vector2Int placedObjectOrigin, BuildingSystemPartSO placedObjectTypeSO, BuildingSystemPartSO.Dir dir, out PlacedObject placedObject)
    {
        return TryPlaceObject(buildingProccess, selectedGrid, placedObjectOrigin, placedObjectTypeSO, dir, out placedObject);
    }
    public bool TryPlaceObject(bool buildingProccess, GridXZ<GridObject> grid, Vector2Int placedObjectOrigin, BuildingSystemPartSO placedObjectTypeSO, BuildingSystemPartSO.Dir dir)
    {
        return TryPlaceObject(buildingProccess, grid, placedObjectOrigin, placedObjectTypeSO, dir, out PlacedObject placedObject);
    }
    public bool TryPlaceObject(bool buildingProccess, GridXZ<GridObject> grid, Vector2Int placedObjectOrigin, BuildingSystemPartSO placedObjectTypeSO, BuildingSystemPartSO.Dir dir, out PlacedObject placedObject)
    {
        // Test Can Build
        List<Vector2Int> gridPositionList = placedObjectTypeSO.GetGridPositionList(placedObjectOrigin, dir);
        bool canBuild = true;
        foreach (Vector2Int gridPosition in gridPositionList)
        {
            //bool isValidPosition = grid.IsValidGridPositionWithPadding(gridPosition);
            bool isValidPosition = grid.IsValidGridPosition(gridPosition);
            if (!isValidPosition)
            {
                // Not valid
                canBuild = false;
                break;
            }
            if (!grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild())
            {
                canBuild = false;
                break;
            }
        }
        if (canBuild)
        {
            placedObject = null;
            if (buildingProccess)
            {
                Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
                Vector3 placedObjectWorldPosition = grid.GetWorldPosition(placedObjectOrigin.x, placedObjectOrigin.y) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();
                placedObject = PlacedObject.Create(placedObjectWorldPosition, placedObjectOrigin, dir, placedObjectTypeSO);
                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
                }
                CheckIfHaveItem();
                placedObject.GridSetupDone();
                return true;
            }
            //OnObjectPlaced?.Invoke(placedObject, EventArgs.Empty);
            return true;
        }
        // Cannot build here
        placedObject = null;
        return false;
    }
    public Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        selectedGrid.GetXZ(worldPosition, out int x, out int z);
        return new Vector2Int(x, z);
    }
    public Vector3 GetWorldPosition(Vector2Int gridPosition)
    {
        return selectedGrid.GetWorldPosition(gridPosition.x, gridPosition.y);
    }
    public GridObject GetGridObject(Vector2Int gridPosition)
    {
        return selectedGrid.GetGridObject(gridPosition.x, gridPosition.y);
    }
    public GridObject GetGridObject(Vector3 worldPosition)
    {
        return selectedGrid.GetGridObject(worldPosition);
    }
    public bool IsValidGridPosition(Vector2Int gridPosition)
    {
        return selectedGrid.IsValidGridPosition(gridPosition);
    }
    #endregion
    private void Save()
    {
        List<PlacedObjectSaveObjectArray> placedObjectSaveObjectArrayList = new List<PlacedObjectSaveObjectArray>();
        foreach (GridXZ<GridObject> grid in gridList)
        {
            List<PlacedObject.SaveObject> saveObjectList = new List<PlacedObject.SaveObject>();
            List<PlacedObject> savedPlacedObjectList = new List<PlacedObject>();
            for (int x = 0; x < grid.GetWidth(); x++)
            {
                for (int y = 0; y < grid.GetHeight(); y++)
                {
                    PlacedObject placedObject = grid.GetGridObject(x, y).GetPlacedObject();
                    if (placedObject != null && !savedPlacedObjectList.Contains(placedObject))
                    {
                        // Save object
                        savedPlacedObjectList.Add(placedObject);
                        saveObjectList.Add(placedObject.GetSaveObject());
                    }
                }
            }
            PlacedObjectSaveObjectArray placedObjectSaveObjectArray = new PlacedObjectSaveObjectArray { placedObjectSaveObjectArray = saveObjectList.ToArray() };
            placedObjectSaveObjectArrayList.Add(placedObjectSaveObjectArray);
        }
        //List<LooseSaveObject> looseSaveObjectList = new List<LooseSaveObject>();
        //foreach (Transform looseObjectTransform in looseObjectTransformList)
        //{
        //    if (looseObjectTransform == null) continue;
        //    looseSaveObjectList.Add(new LooseSaveObject
        //    {
        //        looseObjectSOName = looseObjectTransform.GetComponent<BuildingSystemPartSO>().name,
        //        position = looseObjectTransform.position,
        //        quaternion = looseObjectTransform.rotation
        //    });
        //}
        SaveObject saveObject = new SaveObject
        {
            placedObjectSaveObjectArrayArray = placedObjectSaveObjectArrayList.ToArray(),
            //looseSaveObjectArray = looseSaveObjectList.ToArray(),
        };
        string json = JsonUtility.ToJson(saveObject);
        PlayerPrefs.SetString("HouseBuildingSystemSave", json);
        SaveSystem.Save("HouseBuildingSystemSave", json, false);
        Debug.Log("Saved!");
    }
    private void Load()
    {
        if (PlayerPrefs.HasKey("HouseBuildingSystemSave"))
        {
            string json = PlayerPrefs.GetString("HouseBuildingSystemSave");
            //json = SaveSystem.Load("HouseBuildingSystemSave_15");
            SaveObject saveObject = JsonUtility.FromJson<SaveObject>(json);
            for (int i = 0; i < gridList.Count; i++)
            {
                GridXZ<GridObject> grid = gridList[i];
                foreach (PlacedObject.SaveObject placedObjectSaveObject in saveObject.placedObjectSaveObjectArrayArray[i].placedObjectSaveObjectArray)
                {
                    BuildingSystemPartSO placedObjectTypeSO = GetPlacedObjectTypeSOFromName(placedObjectSaveObject.placedObjectTypeSOName);
                    TryPlaceObject(true, grid, placedObjectSaveObject.origin, placedObjectTypeSO, placedObjectSaveObject.dir, out PlacedObject placedObject);
                    if (placedObject is FloorPlacedObject)
                    {
                        FloorPlacedObject floorPlacedObject = (FloorPlacedObject)placedObject;
                        floorPlacedObject.Load(placedObjectSaveObject.floorPlacedObjectSave);
                    }
                }
            }
            foreach (LooseSaveObject looseSaveObject in saveObject.looseSaveObjectArray)
            {
                Transform looseObjectTransform = Instantiate(
                    GetPlacedObjectTypeSOFromName(looseSaveObject.looseObjectSOName).prefab,
                    looseSaveObject.position,
                    looseSaveObject.quaternion
                );
                //looseObjectTransformList.Add(looseObjectTransform);
            }
        }
        Debug.Log("Load!");
    }
}
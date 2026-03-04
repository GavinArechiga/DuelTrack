using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    public static GridSystem Instance { get; private set; }
    public event Action OnObjectPlaced;
    public event Action OnObjectRemoved;
    
    [field: SerializeField] public GameObject CurrentlySelectedObject { get; private set; }
    [SerializeField] private Grid grid;
    [SerializeField] private GridObjectListSO gridObjectListSO;
    
    private Vector3Int currentCellPosition;
    private InputManager.Direction placementDirection;
    private readonly Dictionary<GameObject, List<Vector3Int>> placedObjects = new();
    
    [Header("Debug")] 
    [SerializeField]private bool showPlayerCellIndicator;
    [SerializeField] private GameObject cellIndicator;
    [SerializeField] private bool showOccupiedCellsDebug = true;
    [SerializeField] private bool showFrontCellDebug = true;
    
    private void Awake()
    {
        //Singleton pattern to make it easier for construction tool to access the grid
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }

    private void Start()
    {
        cellIndicator.SetActive(showPlayerCellIndicator);
    }

    private void Update()
    {
        if (showPlayerCellIndicator)
        {
            MoveCellIndicator();
        }
    }

    public PlacementData GetPlacementData()
    {
        Vector2Int gridSize = GetObjectGridSize();
        Vector3Int frontCell = GetFrontCell();
        Vector3 cellCenter = GetCellCenterWorldPosition(frontCell);
        Quaternion rotation = CalculateObjectRotation();
        bool hasOverlap = CheckForOverlap(frontCell, gridSize);

        return new PlacementData
        {
            GridSize = gridSize,
            FrontCell = frontCell,
            CellCenter = cellCenter,
            Rotation = rotation,
            HasOverlap = hasOverlap,
        };
    }
    public void UpdateGrid(Vector3 playerPosition, InputManager.Direction facingDirection)
    {
        currentCellPosition = grid.WorldToCell(playerPosition);
        currentCellPosition.y = 0;
        placementDirection = facingDirection;
    }
    
    private void MoveCellIndicator()
    {
        Vector3 worldPosition = grid.CellToWorld(currentCellPosition);
        worldPosition.y = 0.015f;
        cellIndicator.transform.position = worldPosition;
    }
    
    #region Object Placement
    public void PlaceObject()
    {
        PlacementData data = GetPlacementData();
        
        if (data.HasOverlap) { return; }
        
        List<Vector3Int> cellPositions = GetCellPositions(data.FrontCell, data.GridSize);
        
        GameObject placedObject = Instantiate(CurrentlySelectedObject, data.CellCenter, data.Rotation);
        placedObjects.Add(placedObject, cellPositions);
        
        OnObjectPlaced?.Invoke();
    }

    public void RemoveObject()
    {
        PlacementData data = GetPlacementData();
        GameObject objectToRemove = null;
        List<Vector3Int> cellPositions = GetCellPositions(data.FrontCell, data.GridSize);
        
        // using tuple deconstruction to make this more readable.
        // if you don't know what that is it just lets you give the key and value a variable name
        foreach ((GameObject prefab, List<Vector3Int> occupiedCells) in placedObjects)
        {
            if (cellPositions.Any(cellPosition => occupiedCells.Contains(cellPosition)))
            {
                objectToRemove = prefab;
            }
        }

        if (objectToRemove == null) { return; }

        placedObjects.Remove(objectToRemove);
        Destroy(objectToRemove);
        
        OnObjectRemoved?.Invoke();
    }

    private Vector3 GetCellCenterWorldPosition(Vector3Int cellPosition)
    {
        return grid.CellToWorld(cellPosition) + grid.cellSize / 2;
    }

    private Vector2Int GetObjectGridSize()
    {
        return gridObjectListSO.gridObjects.Find(gridObject => 
            gridObject.prefab == CurrentlySelectedObject).gridSize;
    }
    
    private bool CheckForOverlap(Vector3Int frontCell, Vector2Int gridSize)
    {
        List<Vector3Int> cellPositions = GetCellPositions(frontCell, gridSize);
        bool hasOverlap = false;

        foreach (List<Vector3Int> occupiedCells in placedObjects.Values)
        {
            hasOverlap = cellPositions.Any(cellPosition => occupiedCells.Contains(cellPosition));
            if (hasOverlap) { break; }
        }
        
        return hasOverlap;
    }

    // object rotation is limited to the four cardinal directions.
    // if player facing is at a diagonal then we collapse the axis clockwise so it looks natural.  
    private Quaternion CalculateObjectRotation()
    {
        // Object rotations
        Quaternion north = Quaternion.Euler(0, 0, 0);
        Quaternion south = Quaternion.Euler(0, 180, 0);
        Quaternion east = Quaternion.Euler(0, 90, 0);
        Quaternion west = Quaternion.Euler(0, -90, 0);
        
        // diagonals favor the cardinal direction that is clockwise from the diagonal
        return placementDirection switch
        {
            InputManager.Direction.North => north,
            InputManager.Direction.NorthEast => east,
            InputManager.Direction.East => east,
            InputManager.Direction.SouthEast => south,
            InputManager.Direction.South => south,
            InputManager.Direction.SouthWest => west,
            InputManager.Direction.West => west,
            InputManager.Direction.NorthWest => north,
            _ => Quaternion.identity,
        };
    }
    
    // Gets the cell in front of the player.
    // because the player can rotate we need to add the players current direction to get the correct cell
    private Vector3Int GetFrontCell()
    {
        Vector3Int direction = InputManager.DirectionToVector3Int(placementDirection);
        return currentCellPosition + direction;
    }

    // returns a list of cell positions that would be occupied if an object of a given size were to be placed down at the front cell position.
    // This function assumes that the pivot of the prefab is centered on the x and aligned to the back face on -z.
    private List<Vector3Int> GetCellPositions(Vector3Int frontCell, Vector2Int objectSize)
    {
        var cellPositions = new List<Vector3Int>();
        
        int halfWidth = objectSize.x / 2;
        int depth = objectSize.y;

        for (int x = -halfWidth; x <= halfWidth; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                var cellPosition = new Vector3Int(x, 0, z);
                Vector3Int rotatedCellPosition = RotateCellPosition(cellPosition);
                cellPositions.Add(frontCell + rotatedCellPosition);
            }
        }
        
        return cellPositions;
    }

    // rotates the passed cell position based on the placement direction.
    // cell position is a position so it is more complicated to rotate then an object since we cant just set the rotation to 90 deg.
    // Instead, we need to swap each axis depending on the rotation and make it positive or negative.
    private Vector3Int RotateCellPosition(Vector3Int cellPosition)
    {
        var east = new Vector3Int(cellPosition.z, 0, -cellPosition.x);
        var south = new Vector3Int(-cellPosition.x, 0, -cellPosition.z);
        var west = new Vector3Int(-cellPosition.z, 0, -cellPosition.x);
        
        // Northwest is north, but it is not included since we are already returning cell position for unhandled values.
        // diagonals favor the direction that is in the clockwise direction which matches the behavior in CalculateObjectRotation()
        return placementDirection switch
        {
            InputManager.Direction.North => cellPosition,
            InputManager.Direction.NorthEast => east,
            InputManager.Direction.East => east,
            InputManager.Direction.SouthEast => south,
            InputManager.Direction.South => south,
            InputManager.Direction.SouthWest => west,
            InputManager.Direction.West => west,
            _ => cellPosition,
        };
    }

    // debug function for visualizing things in the scene view. Useful for visually seeing the cell positions when debugging  
    private void OnDrawGizmos()
    {
        // draws the front cell position
        if (Application.isPlaying && showFrontCellDebug)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(GetCellCenterWorldPosition(GetFrontCell()), Vector3.one);
        }
        
        if (placedObjects.Count == 0 || !showOccupiedCellsDebug) { return; }
        
        // draws all the occupied cells in the scene view
        foreach (List<Vector3Int> cellPositions in placedObjects.Values)
        {
            foreach (Vector3Int cellPosition in cellPositions)
            {
                Vector3 worldPos = grid.CellToWorld(new Vector3Int(cellPosition.x, 0, cellPosition.z));
                worldPos += grid.cellSize / 2;
                
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(worldPos, Vector3.one);
            }
        }
    }

    #endregion
}

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    //TODO: refactor to remove circular dependency on player and optimize object preview performance.
    public static GridSystem Instance { get; private set; }
    public PlayerController PlayerController { private get; set; }
    
    [SerializeField] private Grid grid;
    [SerializeField] private GridObjectListSO gridObjectListSO;
    [SerializeField] private GameObject currentlySelectedObject;
    
    [Header("Object Preview")]
    [SerializeField] private Material previewMaterial;
    private GameObject previewObject;
    private Renderer[] previewRenderers;
    private Vector3Int lastFrontCellPosition;

    [Header("Debug")] 
    [SerializeField]private bool showPlayerCellIndicator;
    [SerializeField] private GameObject cellIndicator;
    [SerializeField] private bool showOccupiedCellsDebug = true;
    [SerializeField] private bool showFrontCellDebug = true;
    private Vector3Int currentCellPosition;
    private Dictionary<GameObject, List<Vector3Int>> placedObjects = new();
    
    
    private void Awake()
    {
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
        
        ShowObjectPreview();
    }
    public void UpdateCellPosition(Vector3 playerPosition)
    {
        currentCellPosition = grid.WorldToCell(playerPosition);
        currentCellPosition.y = 0;
    }
    
    private void MoveCellIndicator()
    {
        Vector3 worldPosition = grid.CellToWorld(currentCellPosition);
        worldPosition.y = 0.015f;
        cellIndicator.transform.position = worldPosition;
    }

    #region Object Preview
    private void ShowObjectPreview()
    {
        Vector3Int frontCell = GetFrontCell();
        
        if (CheckIfSameFrontCell(frontCell))
        {
            return;
        }
        
        Vector3 cellCenterWorldPosition = GetCellCenterWorldPosition(frontCell);
        previewObject = Instantiate(currentlySelectedObject, cellCenterWorldPosition, CalculateObjectRotation());
        previewObject.GetComponentInChildren<Collider>().enabled = false;
        
        FixPreviewZFighting();
        ChangePreviewMaterial(frontCell);
    }
    
    // Fixes Z fighting between a placed object and the preview by slightly shifting the previews position by 0.1f
    // on the upwards and backwards directions.
    private void FixPreviewZFighting()
    {
        Vector3 upOffset = Vector3.up * 0.01f;
        Vector3 backOffset = -previewObject.transform.forward * 0.01f;
        
        previewObject.transform.position += upOffset + backOffset;
    }
    private void ChangePreviewMaterial(Vector3Int frontCell)
    {
        Material previewMaterialInstance = Instantiate(previewMaterial);
        previewRenderers = previewObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in previewRenderers)
        {
            rend.material = previewMaterialInstance;
        }
        
        bool hasOverlap = CheckForOverlap(frontCell, out List<Vector3Int> _);
        ChangePreviewObjectColor(hasOverlap ? Color.red : Color.white, previewRenderers);
    }
    private bool CheckIfSameFrontCell(Vector3Int frontCell)
    {
        if (lastFrontCellPosition == frontCell) {
            return true;
        }

        lastFrontCellPosition = frontCell;
        
        if (previewObject)
        {
            Destroy(previewObject);
        }

        return false;
    }

    private void ChangePreviewObjectColor(Color color, Renderer[] renderers)
    {
        color.a = previewMaterial.color.a;
        foreach (Renderer rend in renderers)
        {
            rend.material.color = color;
        }
    }
    
    #endregion

    #region Object Placement
    public void PlaceObject()
    {
        Vector3Int frontCell = GetFrontCell();
        bool hasOverlap = CheckForOverlap(frontCell, out List<Vector3Int> cellPositions);

        if (hasOverlap) { return; }
        Vector3 cellCenter = GetCellCenterWorldPosition(frontCell);
        
        Destroy(previewObject);
        GameObject placedObject = Instantiate(currentlySelectedObject, cellCenter, CalculateObjectRotation());
        placedObjects.Add(placedObject, cellPositions);
    }

    public void RemoveObject()
    {
        Vector3Int frontCell = GetFrontCell();
        GameObject objectToRemove = null;
        
        // using tuple deconstruction to make this more readable.
        // if you don't know what that is it just lets you give the key and value a variable name
        foreach ((GameObject prefab, List<Vector3Int> occupiedCells) in placedObjects)
        {
            foreach (Vector3Int cellPosition in occupiedCells)
            {
                if (cellPosition == frontCell)
                {
                    objectToRemove = prefab;
                }
            }
        }

        if (objectToRemove == null) { return; }

        placedObjects.Remove(objectToRemove);
        Destroy(objectToRemove);
        ChangePreviewMaterial(frontCell);
    }

    private Vector3 GetCellCenterWorldPosition(Vector3Int cellPosition)
    {
        return grid.CellToWorld(cellPosition) + grid.cellSize / 2;
    }
    
    private bool CheckForOverlap(Vector3Int frontCell, out List<Vector3Int> cellPositions)
    {
        Vector2Int objectSize = gridObjectListSO.gridObjects.Find(gridObject => gridObject.prefab == currentlySelectedObject).gridSize;

        cellPositions = GetCellPositions(frontCell, objectSize);
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
        return PlayerController.CurrentFacingDirection switch
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
        Vector3Int direction = InputManager.DirectionToVector3Int(PlayerController.CurrentFacingDirection);
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
                Vector3Int cellPosition = new Vector3Int(x, 0, z);
                Vector3Int rotatedCellPosition = RotateCellPosition(cellPosition);
                cellPositions.Add(frontCell + rotatedCellPosition);
            }
        }
        
        return cellPositions;
    }

    // rotates the passed cell position based on the players facing direction.
    // cell position is a position so it is more complicated to rotate then an object since we cant just set the rotation to 90 deg.
    // Instead, we need to swap each axis depending on the rotation and make it positive or negative.
    private Vector3Int RotateCellPosition(Vector3Int cellPosition)
    {
        var east = new Vector3Int(cellPosition.z, 0, -cellPosition.x);
        var south = new Vector3Int(-cellPosition.x, 0, -cellPosition.z);
        var west = new Vector3Int(-cellPosition.z, 0, -cellPosition.x);
        
        // Northwest is north, but it is not included since we are already returning cell position for unhandled values.
        // diagonals favor the direction that is in the clockwise direction which matches the behavior in CalculateObjectRotation()
        return PlayerController.CurrentFacingDirection switch
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

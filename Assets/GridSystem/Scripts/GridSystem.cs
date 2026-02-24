using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    // TODO: add support for diagonals. 
    public static GridSystem Instance { get; private set; }
    public PlayerController PlayerController { private get; set; }
    
    [SerializeField] private Grid grid;
    [SerializeField] private GameObject cellIndicator;
    [SerializeField] private GridObjectListSO gridObjectListSO;
    [SerializeField] private GameObject currentlySelectedObject;
    
    [Header("Object Preview")]
    [SerializeField] private Material previewMaterial;
    private GameObject previewObject;
    private Renderer[] previewRenderers;
    private Vector3Int lastFrontCellPosition;
    
    [Header("Debug")]
    [SerializeField] private bool showOccupiedCellsDebug = true;

    private Vector3Int currentCellPosition;
    private List<Vector3Int> occupiedCells = new();
    
    
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

    private void Update()
    {
        MoveCellIndicator();
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
        Instantiate(currentlySelectedObject, cellCenter, CalculateObjectRotation());
        occupiedCells.AddRange(cellPositions);
    }

    private Vector3 GetCellCenterWorldPosition(Vector3Int cellPosition)
    {
        return grid.CellToWorld(cellPosition) + grid.cellSize / 2;
    }
    
    private bool CheckForOverlap(Vector3Int frontCell, out List<Vector3Int> cellPositions)
    {
        Vector2Int objectSize = gridObjectListSO.gridObjects.Find(gridObject => gridObject.prefab == currentlySelectedObject).gridSize;

        cellPositions = GetCellPositions(frontCell, objectSize);
        bool hasOverlap = cellPositions.Any(cellPosition => occupiedCells.Contains(cellPosition));
        return hasOverlap;
    }

    private Quaternion CalculateObjectRotation()
    {
        return PlayerController.CurrentFacingDirection switch
        {
            InputManager.Direction.Forward => Quaternion.Euler(0, 0, 0),
            InputManager.Direction.Backward => Quaternion.Euler(0, 180, 0),
            InputManager.Direction.Left => Quaternion.Euler(0, -90, 0),
            InputManager.Direction.Right => Quaternion.Euler(0, 90, 0),
            _ => Quaternion.identity,
        };
    }
    
    
    private Vector3Int GetFrontCell()
    {
        Vector3Int direction = InputManager.DirectionToVector3Int(PlayerController.CurrentFacingDirection);
        return currentCellPosition + direction;
    }

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

    private Vector3Int RotateCellPosition(Vector3Int cellPosition)
    {
        return PlayerController.CurrentFacingDirection switch
        {
            InputManager.Direction.Forward => cellPosition,
            InputManager.Direction.Backward => new Vector3Int(-cellPosition.x, 0, -cellPosition.z),
            InputManager.Direction.Left => new Vector3Int(-cellPosition.z, 0, -cellPosition.x),
            InputManager.Direction.Right => new Vector3Int(cellPosition.z, 0, -cellPosition.x),
            _ => cellPosition,
        };
    }

    private void OnDrawGizmos()
    {
        if (occupiedCells.Count == 0 || !showOccupiedCellsDebug) { return; }

        foreach (Vector3Int cellPosition in occupiedCells)
        {
            Vector3 worldPos = grid.CellToWorld(new Vector3Int(cellPosition.x, 0, cellPosition.z));
            worldPos += grid.cellSize / 2;
                
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(worldPos, Vector3.one);
        }
    }

    #endregion
}

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    // TODO: add support for diagonals. Create system for preventing object overlap 
    public static GridSystem Instance { get; private set; }
    public PlayerController PlayerController { private get; set; }
    
    [SerializeField] private Grid grid;
    [SerializeField] private GameObject cellIndicator;
    [SerializeField] private GridObjectListSO gridObjectListSO;

    private Vector3Int currentCellPosition;
    private List<Vector3Int> occupiedCells = new List<Vector3Int>();

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
    }
    
    public void UpdateCellPosition(Vector3 playerPosition)
    {
        currentCellPosition = grid.WorldToCell(playerPosition);
        currentCellPosition.y = 0;
    }

    #region Object Placement
    public void PlaceObject(GameObject prefab)
    {
        Vector3Int frontCell = GetFrontCell();
        Vector2Int objectSize = gridObjectListSO.gridObjects.Find(gridObject => gridObject.prefab == prefab).gridSize;

        List<Vector3Int> cellPositions = GetCellPositions(frontCell, objectSize);
        bool hasOverlap = cellPositions.Any(cellPosition => occupiedCells.Contains(cellPosition));
        
        if (hasOverlap) {return;}
        
        Vector3 cellCorner = grid.CellToWorld(frontCell);
        Vector3 cellCenter = cellCorner + grid.cellSize / 2;
        

        Instantiate(prefab, cellCenter, CalculateObjectRotation());
        occupiedCells.AddRange(GetCellPositions(frontCell, objectSize));
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
    
    /// <summary>
    /// Calculates the position of the cell that is +1 cell in front of the player
    /// based on the players forward vector.
    /// </summary>
    /// <returns>
    ///<see cref="Vector3Int"/> containing the calculated position
    /// </returns>
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
        if (occupiedCells.Count != 0)
        {
            foreach (Vector3Int cellPosition in occupiedCells)
            {
                Vector3 worldPos = grid.CellToWorld(new Vector3Int(cellPosition.x, 0, cellPosition.z));
                worldPos += grid.cellSize / 2;
                
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(worldPos, Vector3.one);
            }
        }
    }

    #endregion
    
    private void MoveCellIndicator()
    {
        Vector3 worldPosition = grid.CellToWorld(currentCellPosition);
        worldPosition.y = 0.015f;
        cellIndicator.transform.position = worldPosition;
    }
}

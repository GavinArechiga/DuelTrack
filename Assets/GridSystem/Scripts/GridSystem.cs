using System;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    // TODO: add support for diagonals. Create system for preventing object overlap 
    public static GridSystem Instance { get; private set; }
    public Transform PlayerTransform { private get; set; }
    
    [SerializeField] private Grid grid;
    [SerializeField] private GameObject cellIndicator;
    [SerializeField] private GridObjectListSO gridObjectListSO;

    private Vector3Int currentCellPosition;

    private enum FacingDirection
    {
        Forward,
        Backward,
        Left,
        Right,
    }
    
    private FacingDirection currentDirection;

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
        UpdateFacingDirection();
        MoveCellIndicator();
    }
    
    public void UpdateCellPosition(Vector3 playerPosition)
    {
        currentCellPosition = grid.WorldToCell(playerPosition);
    }

    #region Object Placement
    public void PlaceObject(GameObject prefab)
    {
        Vector3Int cellPosition = CalculateCellPosition();
        Vector2Int objectSize = gridObjectListSO.gridObjects.Find(gridObject => gridObject.prefab == prefab).gridSize;
        Vector3Int offset = GetRotationOffset(objectSize);
        Vector3 worldPosition = grid.CellToWorld(cellPosition + offset);
        
        Debug.Log(currentDirection);
        
        Instantiate(prefab, worldPosition, CalculateObjectRotation());
    }

    
    private void UpdateFacingDirection()
    {
        Vector3 forward = PlayerTransform.forward;

        // Vector3.Dot() returns the dot product of two vectors which is a float you get from multiplying the two vectors
        // if the dot product is 1 then both vectors face the same direction. if the dot product is -1 then they face opposite directions.
        // if the dot product is 0 then they are at a 90-degree angle
        // There are also in between values but for this function we only care if its positive so we know what direction the player is facing 
        if (Vector3.Dot(forward, Vector3.forward) > 0.5f)
        {
            currentDirection = FacingDirection.Forward;
        }
        else if (Vector3.Dot(forward, Vector3.back) > 0.5f)
        {
            currentDirection = FacingDirection.Backward;
        }
        else if (Vector3.Dot(forward, Vector3.right) > 0.5f)
        {
            currentDirection = FacingDirection.Right;
        }
        else if (Vector3.Dot(forward, Vector3.left) > 0.5f)
        {
            currentDirection = FacingDirection.Left;
        }
    }

    private Quaternion CalculateObjectRotation()
    {
        return currentDirection switch
        {
            FacingDirection.Forward => Quaternion.Euler(0, 0, 0),
            FacingDirection.Backward => Quaternion.Euler(0, 180, 0),
            FacingDirection.Left => Quaternion.Euler(0, -90, 0),
            FacingDirection.Right => Quaternion.Euler(0, 90, 0),
            _ => Quaternion.identity,
        };

    }

    private Vector3Int GetRotationOffset(Vector2Int size)
    {
        int width = size.x;
        int length = size.y;
        
        // Calculates the number of cells and on what axis the object needs to be moved after rotation to maintain the same grid position it had before rotation.
        // Each grid objects pivot is in the bottom left corner so that it aligns with the cell.
        // This makes rotation tricky because we are rotating around a pivot so we need to change which axis to offset based on the direction.  
        // The max function is used to guarantee that the object moves by at least 1 cell on that axis,
        // this fixes issues where the length or width is 1 so doing width - 1 or length -1 would get you 0.  
        return currentDirection switch
        {
            FacingDirection.Forward  => Vector3Int.zero,
            FacingDirection.Backward => new Vector3Int(Mathf.Max(1, width - 1), 0, Mathf.Max(1, length - 1)),
            FacingDirection.Right    => new Vector3Int(0, 0, Mathf.Max(1, length - 1)),
            FacingDirection.Left     => new Vector3Int(Mathf.Max(1, length - 1), 0, 0),
            _ => Vector3Int.zero,
        };
    }
    
    /// <summary>
    /// Calculates the position of the cell that is +1 cell in front of the player
    /// based on the players forward vector.
    /// </summary>
    /// <returns>
    ///<see cref="Vector3Int"/> containing the calculated position
    /// </returns>
    private Vector3Int CalculateCellPosition()
    {
        Vector3Int directionVector = currentDirection switch
        {
            FacingDirection.Forward  => new Vector3Int(0, 0, 1),
            FacingDirection.Backward => new Vector3Int(0, 0, -1),
            FacingDirection.Left     => new Vector3Int(-1, 0, 0),
            FacingDirection.Right    => new Vector3Int(1, 0, 0),
            _ => Vector3Int.zero,
        };

        return currentCellPosition + directionVector;
    }
    #endregion
    
    private void MoveCellIndicator()
    {
        Vector3 worldPosition = grid.CellToWorld(currentCellPosition);
        worldPosition.y = 0.015f;
        cellIndicator.transform.position = worldPosition;
    }
}

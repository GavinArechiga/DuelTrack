using System;
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
        Vector3Int cellInFront = GetFrontCell();
        // This might look confusing,
        // but it is basically just a for loop that checks if each elements prefab field matches the current prefab. If it does we get that element and access the grid size.  
        Vector2Int objectSize = gridObjectListSO.gridObjects.Find(gridObject => gridObject.prefab == prefab).gridSize;
        
        Vector3 cellCorner = grid.CellToWorld(cellInFront);
        Vector3 cellCenter = cellCorner + grid.cellSize / 2;

        // Calculates offset based on object size to correct for rotation.
        //object size is a vector2 so the y is actually the z
        float offsetAmount = (objectSize.y - 1) * grid.cellSize.z / 2f;
        Vector3 offsetDirection = -InputManager.DirectionToVector3(PlayerController.CurrentFacingDirection);
        Vector3 spawnPosition = cellCenter + offsetDirection * offsetAmount;
        
        Instantiate(prefab, spawnPosition, CalculateObjectRotation());
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
    #endregion
    
    private void MoveCellIndicator()
    {
        Vector3 worldPosition = grid.CellToWorld(currentCellPosition);
        worldPosition.y = 0.015f;
        cellIndicator.transform.position = worldPosition;
    }
}

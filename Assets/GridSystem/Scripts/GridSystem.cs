using System;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    public static GridSystem Instance { get; private set; }
    
    [SerializeField] private Grid grid;
    [SerializeField] private GameObject cellIndicator;

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
    
    public void UpdateCellPosition(Vector3 worldPosition)
    {
        currentCellPosition = grid.WorldToCell(worldPosition);
    }

    public void PlaceObject(GameObject prefab, Transform playerTransform)
    {
        Vector3Int cellPosition = CalculateCellPosition(playerTransform);
        Vector3 worldPosition = grid.CellToWorld(cellPosition);
        
        Debug.Log(Vector3Int.RoundToInt(playerTransform.forward));
        
        Instantiate(prefab, worldPosition, Quaternion.identity);
    }
    
    /// <summary>
    /// Calculates the position of the cell that is +1 cell in front of the player
    /// based on the players forward vector.
    /// </summary>
    /// <param name="playerTransform">
    /// Reference to the players transform component 
    /// </param>
    /// <returns>
    ///<see cref="Vector3Int"/> containing the calculated position
    /// </returns>
    private Vector3Int CalculateCellPosition(Transform playerTransform)
    {
        int cellShiftZ = Mathf.RoundToInt(1 * playerTransform.forward.z);
        int cellShiftX = Mathf.RoundToInt(1 * playerTransform.forward.x);
        
        int cellX = currentCellPosition.x + cellShiftX;
        int cellZ = currentCellPosition.z + cellShiftZ;
        
        return new Vector3Int(cellX, 0, cellZ);
    }
    
    private void MoveCellIndicator()
    {
        Vector3 worldPosition = grid.CellToWorld(currentCellPosition);
        worldPosition.y = 0.015f;
        cellIndicator.transform.position = worldPosition;
    }
}

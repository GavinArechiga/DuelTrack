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
    
    private void MoveCellIndicator()
    {
        Vector3 worldPosition = grid.CellToWorld(currentCellPosition);
        worldPosition.y = 0.015f;
        cellIndicator.transform.position = worldPosition;
    }
}

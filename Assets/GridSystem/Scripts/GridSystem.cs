using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    public static GridSystem Instance { get; private set; }
    public PlayerController PlayerController { private get; set; }
    
    [SerializeField] private Grid grid;
    [SerializeField] private PlacementSystem placementSystem;
    [SerializeField] private PreviewSystem previewSystem;
    [SerializeField] public GameObject currentlySelectedObject;

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
        
        previewSystem.ShowObjectPreview
            (frontCell,
            GetCellCenterWorldPosition(frontCell),
            currentlySelectedObject, 
            CalculateObjectRotation());
    }
    
    #endregion

    #region Object Placement
    public void PlaceObject()
    {
        Vector3Int frontCell = GetFrontCell();
        
        placementSystem.PlaceObject
            (frontCell,
            GetCellCenterWorldPosition(frontCell),
            currentlySelectedObject,
            CalculateObjectRotation());
    }

    public void RemoveObject()
    {
        placementSystem.RemoveObject(GetFrontCell());
    }
    
    // Gets the cell in front of the player.
    // because the player can rotate we need to add the players current direction to get the correct cell
    private Vector3Int GetFrontCell()
    {
        Vector3Int direction = InputManager.DirectionToVector3Int(PlayerController.CurrentFacingDirection);
        return currentCellPosition + direction;
    }

    private Vector3 GetCellCenterWorldPosition(Vector3Int cellPosition)
    {
        return grid.CellToWorld(cellPosition) + grid.cellSize / 2;
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

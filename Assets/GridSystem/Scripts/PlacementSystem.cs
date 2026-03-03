using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    public static event Action OnObjectPlaced;
    public static event Action OnObjectRemoved;
    
    public PlayerController PlayerController { private get; set; }
    [SerializeField] private GridObjectListSO gridObjectListSO;

    private readonly Dictionary<GameObject, List<Vector3Int>> placedObjects = new();


    public void PlaceObject(Vector3Int frontCell, Vector3 worldCellCenter, GameObject prefab, Quaternion rotation)
    {
        List<Vector3Int> cellPositions = GetCellPositions(frontCell, gridObjectListSO.gridObjects.Find(
            gridObject => gridObject.prefab == prefab).gridSize);
        
        bool hasOverlap = CheckForOverlap(cellPositions);
        if (hasOverlap) { return; }
        
        GameObject placedObject = Instantiate(prefab, worldCellCenter, rotation);
        placedObjects.Add(placedObject, cellPositions);
        
        OnObjectPlaced?.Invoke();
    }

    public void RemoveObject(Vector3Int frontCell)
    {
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
        
        OnObjectRemoved?.Invoke();
    }
    
    private bool CheckForOverlap(List<Vector3Int> cellPositions)
    {
        bool hasOverlap = false;

        foreach (List<Vector3Int> occupiedCells in placedObjects.Values)
        {
            hasOverlap = cellPositions.Any(cellPosition => occupiedCells.Contains(cellPosition));
            if (hasOverlap) { break; }
        }
        
        return hasOverlap;
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
}

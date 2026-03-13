using UnityEngine;

public struct PlacementData
{
    public Vector2Int GridSize;
    public Vector3Int FrontCell;
    public Vector3 CellCenter;
    public Quaternion Rotation;
    public bool HasOverlap;
}

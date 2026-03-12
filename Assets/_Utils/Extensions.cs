using UnityEngine;

public static class Extensions
{
    public static Vector3 ToVector3( this Direction direction)
    {
        return direction switch
        {
            Direction.North => Vector3.forward,
            Direction.NorthEast => Vector3.forward + Vector3.right,
            Direction.East => Vector3.right,
            Direction.SouthEast => Vector3.back + Vector3.right,
            Direction.South => Vector3.back,
            Direction.SouthWest => Vector3.back + Vector3.left,
            Direction.West => Vector3.left,
            Direction.NorthWest => Vector3.forward + Vector3.left,
            _ => Vector3.zero,
        };
    }
    
    public static Vector3Int ToVector3Int( this Direction direction)
    {
        return direction switch
        {
            Direction.North => Vector3Int.forward,
            Direction.NorthEast => Vector3Int.forward + Vector3Int.right,
            Direction.East => Vector3Int.right,
            Direction.SouthEast => Vector3Int.back + Vector3Int.right,
            Direction.South => Vector3Int.back,
            Direction.SouthWest => Vector3Int.back + Vector3Int.left,
            Direction.West => Vector3Int.left,
            Direction.NorthWest => Vector3Int.forward + Vector3Int.left,
            _ => Vector3Int.zero,
        };
    }
}

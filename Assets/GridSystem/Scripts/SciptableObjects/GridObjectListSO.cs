using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GridObjectListSO", menuName = "Scriptable Objects/GridObjectListSO")]
public class GridObjectListSO : ScriptableObject
{
    public List<GridObjectData> gridObjects;
}

[Serializable]
public class GridObjectData
{
    public string name;
    public Vector2Int gridSize;
    public GameObject prefab;
}

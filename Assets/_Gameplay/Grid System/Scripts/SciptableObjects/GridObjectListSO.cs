using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "GridObjectListSO", menuName = "Scriptable Objects/GridObjectListSO")]
public class GridObjectListSO : ScriptableObject
{
    public List<GridObjectData> GridObjects;
}
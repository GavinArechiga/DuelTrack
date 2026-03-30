using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "GridObjectListSO", menuName = "Scriptable Objects/GridObjectListSO")]
public class ThemeObjectListSO : ScriptableObject
{
    [field: SerializeField] public string ThemeName { get; private set; }
    [field: SerializeField] public List<GridObjectData> GridObjects { get; private set; }
}
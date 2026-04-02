using System;
using UnityEngine;

[Serializable]
public class GridObjectData : MonoBehaviour
{
    public string Name;
    public Vector2Int GridSize;
    public GameObject Prefab;
    public Sprite Sprite;
}

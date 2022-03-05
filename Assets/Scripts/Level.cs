using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Level : ScriptableObject
{
    public string MapName;
    public List<Vector3Int> TilePos, BorderPos;
    public List<TileBase> Tile, Border;
    public int MaxColor = 6;
}
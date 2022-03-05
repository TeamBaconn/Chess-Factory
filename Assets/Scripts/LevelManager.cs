using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private Tilemap _Tile, _Border;
    
    public static LevelManager Instance { private set; get; }

    public int CurrentLevel {private set; get;}
    private int _CurrentLevelMaxColor;

    private void Awake()
    {
        Instance = this;
        LevelSelector selector = FindObjectOfType<LevelSelector>();
        if (selector == null || selector.SelectedLevel == null)
        {
            SetLevel(0);
            return;
        }
        LoadCurrentMap(selector.SelectedLevel);
    }

    public int GetMaxColor()
    {
        return _CurrentLevelMaxColor;
    }
    public void SetLevel(int k)
    {
        Level level = GetLevelList()[k];
        CurrentLevel = k; 
        LoadCurrentMap(level);
    }
    public static List<Level> GetLevelList()
    {
        Level[] levels = Resources.LoadAll<Level>("Levels/"); 
        return new List<Level>(levels);
    }
    public void LoadCurrentMap(Level level)
    {
        if (level == null) return;
        _CurrentLevelMaxColor = level.MaxColor;
        _Tile.ClearAllTiles();
        _Border.ClearAllTiles();
        for (int i = 0; i < level.Tile.Count; i++)
            _Tile.SetTile(level.TilePos[i], level.Tile[i]);

        for (int i = 0; i < level.Border.Count; i++)
            _Border.SetTile(level.BorderPos[i], level.Border[i]);
    }
#if UNITY_EDITOR
    public void SaveCurrentMap(string name, int maxcolor)
    {
        if (name == null || name.Length == 0) return;
        Level asset = ScriptableObject.CreateInstance<Level>();
        asset.MaxColor = maxcolor;
        asset.Tile = new List<TileBase>();
        asset.TilePos = new List<Vector3Int>();
        foreach (var position in _Tile.cellBounds.allPositionsWithin)
        {
            if (!_Tile.HasTile(position)) continue;
            asset.Tile.Add(_Tile.GetTile(position));
            asset.TilePos.Add(position);
        }
        asset.MapName = name; 
        asset.Border = new List<TileBase>();
        asset.BorderPos = new List<Vector3Int>();
        foreach (var position in _Border.cellBounds.allPositionsWithin)
        {
            if (!_Border.HasTile(position)) continue;
            asset.Border.Add(_Border.GetTile(position));
            asset.BorderPos.Add(position);
        }
        AssetDatabase.CreateAsset(asset, $"Assets/Resources/Levels/{name}.asset");
        AssetDatabase.SaveAssets();
    }
    public void DeleteLevel(Level level)
    {
        AssetDatabase.DeleteAsset($"Assets/Resources/Levels/{level.name}.asset");
    }
#endif
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChessSpawner : CameraLock
{
    [SerializeField]
    private GameObject _ChessPrefab;
    [SerializeField]
    private GameObject _ShadowPrefab;

    [SerializeField]
    private Tilemap _Tile;

    private Animator _Animator;
    
    private Dictionary<Vector2, GameObject> _Entities;

    private List<Vector2> _Shadows;
    private bool _NextTurn = true;

    public static ChessSpawner Instance { private set; get; }

    private void Awake()
    {
        Instance = this;
        _Animator = GetComponent<Animator>();
        _Entities = new Dictionary<Vector2, GameObject>();
        _Shadows = new List<Vector2>();
    }

    protected override void Start()
    {
        base.Start();
        InitChessPiece(4);
    }

    private void InitChessPiece(int piece)
    { 
        List<Vector2> goodTiles = new List<Vector2>();
        foreach (var position in _Tile.cellBounds.allPositionsWithin)
        {
            if (!_Tile.HasTile(position)) continue;
            Vector2 location = new Vector2(position.x + 0.5f, position.y + 0.5f);
            if (_Entities.ContainsKey(location)) continue;
            goodTiles.Add(location);
        }
        if (piece >= goodTiles.Count)
        {
            Debug.Log("Not enough space");

            ScoreCounter.Instance.EndGame();
            return;
        } 
        while(piece > 0)
        {
            int index = Random.Range(0, goodTiles.Count);
            Vector2 target = goodTiles[index];
            goodTiles.RemoveAt(index);
            GameObject chess = Instantiate(_ChessPrefab, target, transform.rotation, null);
            chess.name = "Ready Piece";
            _Entities.Add(target, chess);
            piece--;
        }
        GenerateShadows();  
    }
    public bool LegitTile(Vector2 vec)
    {
        return _Tile.GetTile(_Tile.WorldToCell(vec)) != null;
    }
    private void GenerateShadows()
    {
        _Shadows.Clear();
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Shadow")) Destroy(obj);

        List<Vector2> goodTiles = new List<Vector2>();
        foreach (var position in _Tile.cellBounds.allPositionsWithin)
        {
            if (!_Tile.HasTile(position)) continue;
            Vector2 location = new Vector2(position.x + 0.5f, position.y + 0.5f);
            if (_Entities.ContainsKey(location)) continue;
            goodTiles.Add(location);
        }

        if (goodTiles.Count == 0)
        {
            ScoreCounter.Instance.EndGame();
            return;
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform holder = transform.GetChild(i);
            //Set holder back to conveyor belt 
            Vector2 currentPos = holder.transform.localPosition;
            holder.transform.localPosition = (Vector2)holder.transform.localPosition + new Vector2(0, 6);
            _Animator.SetBool("Toggle", true);

            //Move holder
            StartCoroutine(RunConveyor(holder, (Vector2)currentPos + new Vector2(0, 6), currentPos));

            GameObject obj = Instantiate(_ChessPrefab, holder);
            obj.GetComponent<ChessPiece>().PieceObject.GetComponent<SpriteRenderer>().sortingLayerName = "Air";
            obj.name = "Not ready piece";

            ChessPiece chess = holder.transform.GetChild(0).GetComponent<ChessPiece>();

            int index = Random.Range(0, goodTiles.Count);
            Vector2 target = goodTiles[index];
            goodTiles.RemoveAt(index);
            GameObject shadow = Instantiate(_ShadowPrefab, target, transform.rotation, null);
            shadow.GetComponent<SpriteRenderer>().color = chess.InitColor;

            shadow.name = "Ready Shadow";
            _Shadows.Add(target); 

            if (goodTiles.Count == 0)
            {
                ScoreCounter.Instance.EndGame();
                return;
            }
        }
    }
    IEnumerator RunConveyor(Transform transform, Vector2 start, Vector2 location)
    {
        float f = 0; 
        while(f <= 1)
        {
            f += 0.01f;
            transform.localPosition = Vector2.Lerp(start, location, f);
            yield return new WaitForSeconds(0.01f); 
        } 
        _Animator.SetBool("Toggle", false);
    }
    private void CheckPiece()
    {
        List<ChessPiece> check = new List<ChessPiece>();
        foreach(Vector2 pos in _Entities.Keys)
        {
            ChessPiece chess = _Entities[pos].GetComponent<ChessPiece>();
            List<int[]> x = new List<int[]>() { new int[]{ 1, 2, -1, -2}, new int[] { 0, 0, 0, 0 }, new int[] { -1, -2, 1, 2 }, new int[] { 1, 2, -1, -2 }, };
            List<int[]> y = new List<int[]>() { new int[]{ 0, 0, 0, 0 }, new int[] { 1, 2, -1, -2 }, new int[] { -1, -2, 1, 2 }, new int[] { -1, -2, 1, 2 }, };
            for (int i = 0; i < x.Count; i++)
            {
                bool flag2 = false;
                for (int j = 0; j < x[i].Length; j++)
                {
                    Vector2 checkPos = pos + new Vector2(x[i][j], y[i][j]);
                    if (!_Entities.ContainsKey(checkPos) || _Entities[checkPos].GetComponent<ChessPiece>().InitColor != chess.InitColor)
                    {
                        flag2 = true;
                        break;
                    }
                }
                if (flag2) continue;
                //Add
                check.Add(chess);
                for (int j = 0; j < x[i].Length; j++)
                {
                    Vector2 checkPos = pos + new Vector2(x[i][j], y[i][j]);
                    check.Add(_Entities[checkPos].GetComponent<ChessPiece>());
                }
                break;
            }
        }
        List<ChessPiece> destroyed = new List<ChessPiece>();
        foreach(ChessPiece chess in check)
        {
            if (destroyed.Contains(chess)) continue;
            _Entities.Remove(chess.transform.position);
            chess.Explode(null);
            destroyed.Add(chess);
            //Add score
            ScoreCounter.Instance.AddScore(10);
        }
    }
    private List<Vector2> GetPath(ChessPiece chess, Vector2 oldPosition, Vector2 position)
    {
        MoveSet set = ChessPiece.GetPieceMoveSet(chess.Type);
        if (set == null) return null; //Horse
        int[] x = set.x;
        int[] y = set.y;
        List<Vector2> result = new List<Vector2>();
        List<PathPoint> go = new List<PathPoint>();
        List<Vector2> went = new List<Vector2>();
        go.Add(new PathPoint(oldPosition, null));
        while (true)
        {
            if (go.Count == 0) return null; 
            for(int i = go.Count-1; i >= 0; i--)
            {
                if (go[i].position.Equals(position))
                {
                    //Done 
                    PathPoint retrieve = go[i];
                    while (true)
                    {
                        if (retrieve.prev == null) return result;
                        result.Add(retrieve.position);
                        retrieve = retrieve.prev;
                    }
                }
                for (int j = 0; j < x.Length; j++)
                {
                    Vector2 pos = new Vector2(go[i].position.x + x[j], go[i].position.y + y[j]);
                    if (GetEntity(pos) != null || _Tile.GetTile(_Tile.WorldToCell(pos)) == null)
                        continue;

                    if (went.Contains(pos)) continue;

                    go.Add(new PathPoint(pos, go[i]));
                    went.Add(pos);
                }

                went.Add(go[i].position);
                go.RemoveAt(i);
            } 
        }
    }
    public bool MoveEntity(Vector2 oldPosition, Vector2 position)
    {
        if (!_NextTurn) return false;
        if (_Shadows.Contains(position)) return false;
        GameObject entity = GetEntity(oldPosition);
        List<Vector2> path = GetPath(entity.GetComponent<ChessPiece>(),oldPosition, position);
        if (path == null && entity.GetComponent<ChessPiece>().Type != PieceType.HORSE) return false;
        _NextTurn = false;
            if (entity == null) return false;
            _Entities.Remove(oldPosition);
            _Entities.Add(position, entity);

            entity.GetComponent<ChessPiece>().Move(position, path, () =>
            {

                //Start to drop to shadow
                for (int i = 0; i < transform.childCount; i++)
                {
                    Transform holder = transform.GetChild(i);
                    if (holder.childCount == 0) continue;
                    GameObject obj = holder.transform.GetChild(0).gameObject;
                    ChessPiece chess = holder.transform.GetChild(0).GetComponent<ChessPiece>();
                    obj.name = "Ready Piece";
                    obj.transform.SetParent(null);
                    
                    _Entities.Add(_Shadows[i], obj);
                    bool flag = i == transform.childCount - 1;
                    chess.Move(_Shadows[i], null, () =>
                    {
                        //Finish
                        obj.GetComponent<ChessPiece>().PieceObject.GetComponent<SpriteRenderer>().sortingLayerName = "Entity";

                        if (flag)
                        {
                            CheckPiece();
                            _NextTurn = true;
                            GenerateShadows();
                        }
                    }); 
                }
            });
            return true; 
    } 
    public GameObject GetEntity(Vector2 position)
    {
        if (_Entities.ContainsKey(position)) return _Entities[position];
        return null;
    }
    
}

class PathPoint
{
    public Vector2 position;
    public PathPoint prev;
    public PathPoint(Vector2 position, PathPoint prev)
    {
        this.position = position;
        this.prev = prev;
    }
}
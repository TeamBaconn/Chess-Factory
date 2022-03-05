using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField]
    private GameObject _Pointer;

    public GameObject _SelectedPiece;

    private Vector2? mousePosition;
    void Start()
    {
        
    }

    void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 tilePosition = new Vector2(0.5f,-0.5f) + new Vector2((int)mousePosition.x - (mousePosition.x < 0 ? 1 : 0), (int)mousePosition.y + (mousePosition.y > 0 ? 1 : 0));
        ChessSpawner spawner = ChessSpawner.Instance;
        if(spawner.LegitTile(tilePosition))
        _Pointer.transform.position = tilePosition;
        if (Input.GetMouseButtonDown(0))
        {
            GameObject target = spawner.GetEntity(tilePosition);
            //Move piece
            if (target != null)
            {
                ToggleGlow(_SelectedPiece, false);
                _SelectedPiece = _SelectedPiece == target ? null : target;
                ToggleGlow(_SelectedPiece, true);
            }
            else if (_SelectedPiece != null)
            {
                //Move piece to target location;
                spawner.MoveEntity(_SelectedPiece.transform.position, tilePosition);
                ToggleGlow(_SelectedPiece, false);
                _SelectedPiece = null;
            }
            else this.mousePosition = mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            if (this.mousePosition != null)
            {
                Camera.main.transform.position = Camera.main.transform.position + (Vector3)(-mousePosition + this.mousePosition); 
            }
        }
        if (Input.GetMouseButtonUp(0)) this.mousePosition = null;
    }

    private void ToggleGlow(GameObject obj, bool toggle)
    {
        if (!obj) return;
        _SelectedPiece.GetComponent<ChessPiece>().Glow(toggle);
    }
}

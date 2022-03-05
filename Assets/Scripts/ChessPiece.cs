using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PieceType
{
    NORMAL, HORSE, BISHOP, QUEEN 
}

public class MoveSet
{
    public int[] x, y;
    public MoveSet(int[] x, int[] y)
    {
        this.x = x;
        this.y = y;
    }
}

public class ChessPiece : Entity
{
    public GameObject PieceObject;


    [SerializeField]
    private List<Sprite> _Sprite;
    public PieceType Type { private set; get; }  

    [SerializeField]
    private GameObject _DustParticle;
    [SerializeField]
    private GameObject _ExplosionParticle;
    private SpriteRenderer _Renderer;

    public AnimationCurve _Curve;

    protected override void Awake()
    {
        base.Awake();
        _Renderer = PieceObject.GetComponent<SpriteRenderer>();
        _Renderer.material.SetColor("OverlayColor", InitColor);
        _DustParticle.GetComponent<ParticleSystem>().startColor = InitColor;

        Type = PieceType.NORMAL;
        //10% of special piece will spawn
        if (UnityEngine.Random.Range(0, 100) < 10)
        {
            Type = (PieceType)UnityEngine.Random.Range(1, Enum.GetNames(typeof(PieceType)).Length);
        }
        _Renderer.sprite = _Sprite[(int)Type];
    }

    public static MoveSet GetPieceMoveSet(PieceType type)
    {
        switch (type)
        {
            case PieceType.NORMAL:
                int[] x = { 0, 1, 0, -1 };
                int[] y = { 1, 0, -1, 0 };
                return new MoveSet(x,y);
            case PieceType.HORSE:
                return null;
            case PieceType.BISHOP: 
                int[] x1 = { 1, -1, -1, 1 };
                int[] y1 = { 1, 1, -1, -1 };
                return new MoveSet(x1, y1);
            case PieceType.QUEEN:
                int[] x2 = { 1, -1, -1, -1, 0, 1, 0, -1 };
                int[] y2 = { 1, 1, -1, 1, 1, 0, -1, 0 };
                return new MoveSet(x2, y2); 
            default:
                return null;
        }
    }
    public void Update()
    {
        _Renderer.sortingOrder = -10000 - (int)transform.position.y;
    }
    public void Glow(bool toggle)
    {
        _Renderer.material.SetFloat("Stroke", toggle ? 0.008f : 0f);
    }

    public void Explode(Action onFinish)
    {
        StartCoroutine(StartExploding(onFinish));
    }

    IEnumerator StartExploding(Action onFinish)
    {
        _Animator.SetBool("Shake", true);
        _DustParticle.GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(0.3f);
        _Animator.SetBool("Shake", false);
        _DustParticle.GetComponent<ParticleSystem>().Stop();

        SoundManager.Instance.Explode.Play();
        yield return new WaitForSeconds(0.1f);
        Instantiate(_ExplosionParticle, transform.position, transform.rotation, null);

        Destroy(gameObject);
        if (onFinish != null) onFinish.Invoke();
    }

    public void Move(Vector2 location, List<Vector2> path, Action onDestinationReach)
    {
        if (path == null) StartCoroutine(StartHopping(location, onDestinationReach));
        else StartCoroutine(StartMoving(location, path, onDestinationReach));
    }

    IEnumerator StartHopping(Vector2 location, Action onDestinationReach)
    {
        _Animator.SetBool("Shake", true);
        _DustParticle.GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(0.15f);
        _Animator.SetBool("Shake", false);
        _DustParticle.GetComponent<ParticleSystem>().Stop();
        float f = 0;
        Vector2 currentLocation = transform.position;
        while (f <= 1)
        {
            transform.position = Vector2.Lerp(currentLocation, location, f);
            PieceObject.transform.localPosition = new Vector2(0, 1) * 2 * _Curve.Evaluate(f);
            yield return new WaitForSeconds(0.01f);
            f += 0.02f;
        }
        SoundManager.Instance.Hit.Play();
        _DustParticle.GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(0.2f); 
        _DustParticle.GetComponent<ParticleSystem>().Stop(); 
        //Round the number
        transform.position = location;
        if(onDestinationReach != null) onDestinationReach.Invoke();
    }

    IEnumerator StartMoving(Vector2 location, List<Vector2> path, Action onDestinationReach)
    {
        _Animator.SetBool("Shake", true);
        _DustParticle.GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(0.4f);
        _Animator.SetBool("Shake", false);
        while (path.Count > 0)
        {
            float f = 0;
            Vector2 currentLocation = transform.position;
            Vector2 target = path[path.Count - 1];
            while (f <= 1)
            {
                transform.position = Vector2.Lerp(currentLocation, target, f);
                yield return new WaitForSeconds(0.01f);
                f += 0.05f;
            }
            path.RemoveAt(path.Count - 1);
        }
        _DustParticle.GetComponent<ParticleSystem>().Stop();
        //Round the number
        transform.position = location;
        if (onDestinationReach != null)
            onDestinationReach.Invoke();
    }
}

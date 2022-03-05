using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public static Color[] color = {
        new Color32(255,228,120,255),
        new Color32(255,255,235,255),
        new Color32(143,222,93,255),
        new Color32(77,166,255,255), //Cyan
        new Color32(255,107,151,255),//Pink 
        new Color32(194,31,255,255),
    };

    public Color InitColor { get; private set; }

    protected Animator _Animator;
    protected virtual void Awake()
    {
        int max = Mathf.Min(color.Length, LevelManager.Instance.GetMaxColor());
        _Animator = GetComponent<Animator>();
        InitColor = color[Random.Range(0, max)];
    }
}

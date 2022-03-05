using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource Hit;
    public AudioSource Explode;

    public static SoundManager Instance { private set; get; }

    private void Start()
    {
        Instance = this;
    }
}

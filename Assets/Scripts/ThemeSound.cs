using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThemeSound : MonoBehaviour
{ 
    void Start()
    {
        foreach (ThemeSound selector in FindObjectsOfType<ThemeSound>()) if (selector != this)
            {
                Destroy(gameObject);
                return;
            }
        GetComponent<AudioSource>().Play();
        DontDestroyOnLoad(gameObject);
    } 
}

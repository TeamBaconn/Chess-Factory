using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLock : MonoBehaviour
{
    public Vector2 OffSet;
     
    protected virtual void Start()
    { 
        Vector2 corner = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width , Screen.height) * OffSet);
        transform.position = corner;
    } 
}

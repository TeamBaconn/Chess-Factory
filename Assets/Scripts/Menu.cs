using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Menu : MonoBehaviour
{
    public GameObject LevelSelector;
    public GameObject MainMenu;
     
    public void Play()
    {
        MainMenu.SetActive(false);
        LevelSelector.SetActive(true);
    }
}

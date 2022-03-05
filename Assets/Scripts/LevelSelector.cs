using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    public Level SelectedLevel;

    public GameObject ButtonPrefab;
    public Transform Panel;

    void Start()
    {
        foreach (LevelSelector selector in FindObjectsOfType<LevelSelector>()) if (selector != this) Destroy(selector.gameObject);
        DontDestroyOnLoad(gameObject);
        List<Level> levels = LevelManager.GetLevelList();
        foreach(Level level in levels)
        {
            GameObject obj = Instantiate(ButtonPrefab, Panel);
            obj.transform.GetChild(0).GetComponent<Text>().text = level.name;
            obj.GetComponent<Button>().onClick.AddListener(() =>
            {
                SelectedLevel = level;
                SceneManager.LoadScene(1);
            });
        }
    }
}

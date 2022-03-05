using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreCounter : MonoBehaviour
{
    public static ScoreCounter Instance { private set; get; }
    private int _Score = 0;
    private float _Timer = 0f;
    [SerializeField]
    private Text _ScoreText;
    [SerializeField]
    private Text _TimerText;
    [SerializeField]
    private GameObject _ReplayPanel;

    void Awake()
    {
        Instance = this;
        UpdateScore(0);
    }
    public void UpdateScore(int k)
    {
        _Score = k;
        _ScoreText.text = ""+ _Score;
    }
    public void AddScore(int k)
    {
        _Score += k;
        UpdateScore(_Score);
    }
    public int GetScore()
    {
        return _Score;
    }
    public void EndGame()
    {
        _ReplayPanel.SetActive(true);
        _ReplayPanel.GetComponent<ReplayPanel>().Active(_Score, _Timer);
    }
    void FixedUpdate()
    {
        _Timer += Time.fixedDeltaTime;
        _TimerText.text = (_Timer / 60 >= 10 ? "" : "0") + (int)(_Timer / 60) + ":" + (_Timer % 60 >= 10 ? "" : "0") + (int)(_Timer%60);
    }
}

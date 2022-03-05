using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReplayPanel : MonoBehaviour
{
    [SerializeField]
    private Text _InformationText;
    
    public void Active(int score, float time)
    {
        string s = "";
        if (PlayerPrefs.GetInt("Highscore") < score)
        {
            PlayerPrefs.SetInt("Highscore", score);
            s = "NEW HIGHSCORE!!!\n";
        }
        _InformationText.text = s+"Highscore: "+ PlayerPrefs.GetInt("Highscore")+"\n\nScore: "+score+"\nTime: "+
            (time / 60 >= 10 ? "" : "0") + (int)(time / 60) + ":" + (time % 60 >= 10 ? "" : "0") + (int)(time % 60);

    }
    public void GoToMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

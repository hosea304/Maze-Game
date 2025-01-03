// WinScript.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinScript : MonoBehaviour
{
    public Text pointsText;
    private bool hasTransitioned = false;

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        DisplayBestTime();
    }

    void DisplayBestTime()
    {
        Difficulty currentDifficulty = (Difficulty)PlayerPrefs.GetInt("CurrentDifficulty", 0);
        float bestTime = PlayerPrefs.GetFloat("BestTime_" + currentDifficulty.ToString(), Mathf.Infinity);

        if (bestTime < Mathf.Infinity)
        {
            int minutes = Mathf.FloorToInt(bestTime / 60);
            int seconds = Mathf.FloorToInt(bestTime % 60);
            pointsText.text = string.Format("BEST TIME: {0:00}:{1:00}\nDifficulty: {2}",
                minutes, seconds, currentDifficulty.ToString());
        }
        else
        {
            pointsText.text = "NO BEST TIME YET\nDifficulty: " + currentDifficulty.ToString();
        }
    }

    public void Setup(int score)
    {
        if (!hasTransitioned)
        {
            hasTransitioned = true;
            SceneManager.LoadScene("Win");
        }
    }

    public void RestartButton()
    {
        SceneManager.LoadScene("GameplayEasy");
    }

    public void ExitButton()
    {
        SceneManager.LoadScene("Flow1");
    }
}
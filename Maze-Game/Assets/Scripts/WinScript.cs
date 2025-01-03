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
        // Ambil kesulitan saat ini
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

    public void LoadScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            Debug.Log("Loading scene: " + sceneName);
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Scene name is empty or null.");
        }
    }

    public void RestartButton()
    {
        // Ambil kesulitan dari PlayerPrefs
        Difficulty currentDifficulty = (Difficulty)PlayerPrefs.GetInt("CurrentDifficulty", 0);

        Debug.Log("Current Difficulty: " + currentDifficulty.ToString());

        // Tentukan scene berdasarkan kesulitan
        switch (currentDifficulty)
        {
            case Difficulty.Easy:
                LoadScene("GameplayEasy");
                break;
            case Difficulty.Medium:
                LoadScene("GameplayMedium");
                break;
            case Difficulty.Hard:
                LoadScene("GameplayHard");
                break;
            case Difficulty.Extreme:
                LoadScene("GameplayExtreme");
                break;
            default:
                Debug.LogError("Unknown difficulty level!");
                break;
        }
    }

    public void ExitButton()
    {
        // Kembali ke menu utama
        LoadScene("Flow1");
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinExtreme : MonoBehaviour
{
    public Text pointsText;
    public Text scoreText; // Tambahkan referensi untuk menampilkan skor
    private bool hasTransitioned = false;

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        DisplayBestTime();
        DisplayScore(); // Panggil fungsi untuk menampilkan skor
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

    void DisplayScore()
    {
        int score = PlayerPrefs.GetInt("PlayerScore", 0); // Ambil skor dari PlayerPrefs
        scoreText.text = "SCORE: " + score.ToString();    // Tampilkan skor di UI
    }

    public void Setup(int score)
    {
        if (!hasTransitioned)
        {
            PlayerPrefs.SetInt("PlayerScore", score); // Simpan skor ke PlayerPrefs
            PlayerPrefs.Save();                      // Simpan semua perubahan ke PlayerPrefs
            hasTransitioned = true;
            SceneManager.LoadScene("Win");
        }
    }

    public void RestartButton()
    {
        SceneManager.LoadScene("GameplayExtreme");
    }

    public void ExitButton()
    {
        SceneManager.LoadScene("Flow1");
    }
}

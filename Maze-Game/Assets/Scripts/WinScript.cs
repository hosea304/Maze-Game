using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScript : MonoBehaviour
{
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
            Debug.Log($"BEST TIME: {minutes:00}:{seconds:00}\nDifficulty: {currentDifficulty}");
        }
        else
        {
            Debug.Log($"NO BEST TIME YET\nDifficulty: {currentDifficulty}");
        }
    }

    public void Setup()
    {
        if (!hasTransitioned)
        {
            hasTransitioned = true;
            SceneManager.LoadScene("Win");
        }
    }

    public void RestartButton()
    {
        SceneManager.LoadScene("GameplayEasy"); // Ubah sesuai dengan nama scene Anda
    }

    public void ExitButton()
    {
        SceneManager.LoadScene("Flow1"); // Ubah sesuai dengan nama scene menu utama Anda
    }

    public void Setup(int maxPlatform)
    {
        if (!hasTransitioned)
        {
            Debug.Log($"Setup called with maxPlatform: {maxPlatform}");
            hasTransitioned = true;
            SceneManager.LoadScene("Win Easy");
        }
    }

}

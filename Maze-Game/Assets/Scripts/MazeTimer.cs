using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MazeTimer : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text timerText;
    public TMP_Text bestTimeText;

    [Header("References")]
    public MazeWinUI mazeWinUI;
    public WinScript winScript;  // Add reference to WinScript

    private float startTime;
    private bool isRunning = false;
    private float bestTime = Mathf.Infinity;

    private Color runningColor = Color.white;
    private Color completedColor = Color.green;
    private Difficulty currentDifficulty = Difficulty.Easy;

    void Start()
    {
        ResetTimer();
        LoadBestTime();
        UpdateBestTimeUI();
    }

    void Update()
    {
        if (isRunning)
        {
            float elapsed = Time.time - startTime;
            int minutes = Mathf.FloorToInt(elapsed / 60);
            int seconds = Mathf.FloorToInt(elapsed % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public void StartTimer()
    {
        startTime = Time.time;
        isRunning = true;
        timerText.color = runningColor;
    }

    public void StopTimer()
    {
        if (isRunning)
        {
            isRunning = false;
            float elapsed = Time.time - startTime;
            timerText.color = completedColor;

            if (elapsed < bestTime)
            {
                bestTime = elapsed;
                SaveBestTime();
                UpdateBestTimeUI();
            }

            // Load Win scene
            SceneManager.LoadScene("Win");
        }
    }

    public void ResetTimer()
    {
        isRunning = false;
        timerText.text = "00:00";
        timerText.color = runningColor;
    }

    public void ResetGame()
    {
        ResetTimer();
        MazeTrigger[] triggers = FindObjectsOfType<MazeTrigger>();
        foreach (MazeTrigger trigger in triggers)
        {
            trigger.ResetTrigger();
        }
        StartTimer(); // Start the timer when game resets
    }
    public void SetDifficulty(Difficulty difficulty)
    {
        currentDifficulty = difficulty;
        PlayerPrefs.SetInt("CurrentDifficulty", (int)difficulty);
        LoadBestTime();
        UpdateBestTimeUI();
    }

    private void SaveBestTime()
    {
        string key = GetBestTimeKey();
        PlayerPrefs.SetFloat(key, bestTime);
        PlayerPrefs.Save();
    }

    private void LoadBestTime()
    {
        string key = GetBestTimeKey();
        bestTime = PlayerPrefs.HasKey(key) ? PlayerPrefs.GetFloat(key) : Mathf.Infinity;
    }

    private void UpdateBestTimeUI()
    {
        if (bestTime < Mathf.Infinity)
        {
            int minutes = Mathf.FloorToInt(bestTime / 60);
            int seconds = Mathf.FloorToInt(bestTime % 60);
            bestTimeText.text = string.Format("Best: {0:00}:{1:00}", minutes, seconds);
        }
        else
        {
            bestTimeText.text = "Best: --:--";
        }
    }

    private string GetBestTimeKey()
    {
        return "BestTime_" + currentDifficulty.ToString();
    }
}
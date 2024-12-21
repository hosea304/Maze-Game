using UnityEngine;
using TMPro; // Pastikan TextMeshPro diimpor

public class MazeTimer : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text timerText; // Referensi ke TextMeshProUGUI untuk menampilkan waktu
    public TMP_Text bestTimeText; // Referensi ke TextMeshProUGUI untuk menampilkan highscore

    private float startTime;
    private bool isRunning = false;
    private float bestTime = Mathf.Infinity;

    // Warna teks
    private Color runningColor = Color.white;
    private Color completedColor = Color.green;

    // Current difficulty
    private Difficulty currentDifficulty = Difficulty.Easy; // Default ke Easy

    void Start()
    {
        ResetTimer(); // Menginisialisasi timer dengan "00:00" dan warna putih
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

    public void SetDifficulty(Difficulty difficulty)
    {
        currentDifficulty = difficulty;
        LoadBestTime();
        UpdateBestTimeUI();
    }

    public void StartTimer()
    {
        startTime = Time.time;
        isRunning = true;
        timerText.color = runningColor; // Pastikan warna saat timer berjalan adalah putih
    }

    public void StopTimer()
    {
        if (isRunning)
        {
            isRunning = false;
            float elapsed = Time.time - startTime;

            // Ubah warna teks menjadi hijau saat timer berhenti
            timerText.color = completedColor;

            // Cek apakah waktu saat ini lebih baik dari highscore
            if (elapsed < bestTime)
            {
                bestTime = elapsed;
                SaveBestTime();
                UpdateBestTimeUI();
            }
        }
    }

    public void ResetTimer()
    {
        isRunning = false;
        timerText.text = "00:00";
        timerText.color = runningColor; // Reset warna teks ke putih
    }

    public void ResetGame()
    {
        ResetTimer();
        MazeTrigger[] triggers = FindObjectsOfType<MazeTrigger>();
        foreach (MazeTrigger trigger in triggers)
        {
            trigger.ResetTrigger();
        }
    }

    void SaveBestTime()
    {
        string key = GetBestTimeKey();
        PlayerPrefs.SetFloat(key, bestTime);
        PlayerPrefs.Save();
    }

    void LoadBestTime()
    {
        string key = GetBestTimeKey();
        if (PlayerPrefs.HasKey(key))
        {
            bestTime = PlayerPrefs.GetFloat(key);
        }
        else
        {
            bestTime = Mathf.Infinity;
        }
    }

    void UpdateBestTimeUI()
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

    string GetBestTimeKey()
    {
        return "BestTime_" + currentDifficulty.ToString();
    }
}

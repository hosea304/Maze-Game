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

    void Start()
    {
        LoadBestTime();
        UpdateBestTimeUI();
        timerText.color = runningColor; // Set warna awal saat game dimulai
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

    void SaveBestTime()
    {
        PlayerPrefs.SetFloat("BestTime", bestTime);
        PlayerPrefs.Save();
    }

    void LoadBestTime()
    {
        if (PlayerPrefs.HasKey("BestTime"))
        {
            bestTime = PlayerPrefs.GetFloat("BestTime");
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
}

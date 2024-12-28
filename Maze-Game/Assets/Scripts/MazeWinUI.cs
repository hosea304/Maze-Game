using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MazeWinUI : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject winPanel; // Panel untuk "You Win"
    public Button playAgainButton;
    public Button backToMenuButton;

    [Header("References")]
    public MazeTimer mazeTimer; // Referensi ke skrip MazeTimer

    void Start()
    {
        // Pastikan panel dalam keadaan tidak aktif
        winPanel.SetActive(false);

        // Tambahkan listener pada tombol
        playAgainButton.onClick.AddListener(PlayAgain);
        backToMenuButton.onClick.AddListener(BackToMenu);
    }

    public void ShowWinUI()
    {
        // Hentikan timer
        mazeTimer.StopTimer();

        // Tampilkan panel
        winPanel.SetActive(true);

        // Menampilkan skor dan waktu terbaik
        Debug.Log($"Current Time: {mazeTimer.timerText.text}");
        Debug.Log($"Best Time: {mazeTimer.bestTimeText.text}");
    }

    void PlayAgain()
    {
        // Restart maze
        mazeTimer.ResetGame();
        winPanel.SetActive(false);
    }

    void BackToMenu()
    {
        // Load menu scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
    }
}

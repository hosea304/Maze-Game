using UnityEngine;
using UnityEngine.UI;

public class MazeWinUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Canvas gameplayCanvas; // Canvas untuk timer
    public Canvas winCanvas;      // Canvas untuk UI menang
    public Button playAgainButton;
    public Button backToMenuButton;

    [Header("References")]
    public MazeTimer mazeTimer;

    void Start()
    {
        // Pastikan win Canvas tidak aktif saat mulai
        if (winCanvas != null)
            winCanvas.gameObject.SetActive(false);

        // Pastikan gameplay Canvas aktif saat mulai
        if (gameplayCanvas != null)
            gameplayCanvas.gameObject.SetActive(true);

        // Tambahkan listener pada tombol
        if (playAgainButton != null)
            playAgainButton.onClick.AddListener(PlayAgain);

        if (backToMenuButton != null)
            backToMenuButton.onClick.AddListener(BackToMenu);
    }

    public void ShowWinUI()
    {
        // Sembunyikan gameplay Canvas
        if (gameplayCanvas != null)
            gameplayCanvas.gameObject.SetActive(false);

        // Tampilkan win Canvas
        if (winCanvas != null)
            winCanvas.gameObject.SetActive(true);

        Debug.Log($"Current Time: {mazeTimer.timerText.text}");
        Debug.Log($"Best Time: {mazeTimer.bestTimeText.text}");
    }

    void PlayAgain()
    {
        // Sembunyikan win Canvas
        if (winCanvas != null)
            winCanvas.gameObject.SetActive(false);

        // Tampilkan gameplay Canvas
        if (gameplayCanvas != null)
            gameplayCanvas.gameObject.SetActive(true);

        // Reset game melalui MazeTimer
        if (mazeTimer != null)
            mazeTimer.ResetGame();
    }

    void BackToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
    }
}
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
        SceneManager.LoadScene("Gameplay");
    }

    public void ExitButton()
    {
        SceneManager.LoadScene("Flow1");
    }
}
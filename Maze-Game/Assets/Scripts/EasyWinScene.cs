using UnityEngine;
using TMPro;

public class WinEasySceneUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text bestTimeText;
    public TMP_Text currentTimeText;

    void Start()
    {
        DisplayTimes();
    }

    private void DisplayTimes()
    {
        string difficultyKey = "BestTime_Easy";
        float bestTime = PlayerPrefs.GetFloat(difficultyKey, Mathf.Infinity);
        float currentTime = PlayerPrefs.GetFloat("CurrentTime_Easy", 0);

        if (bestTime < Mathf.Infinity)
        {
            int bestMinutes = Mathf.FloorToInt(bestTime / 60);
            int bestSeconds = Mathf.FloorToInt(bestTime % 60);
            bestTimeText.text = string.Format("Best Time: {0:00}:{1:00}", bestMinutes, bestSeconds);
        }
        else
        {
            bestTimeText.text = "Best Time: --:--";
        }

        int currentMinutes = Mathf.FloorToInt(currentTime / 60);
        int currentSeconds = Mathf.FloorToInt(currentTime % 60);
        currentTimeText.text = string.Format("Current Time: {0:00}:{1:00}", currentMinutes, currentSeconds);
    }
}

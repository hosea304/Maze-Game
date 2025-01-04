using UnityEngine;
using TMPro;

public class EasyWinScene : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text bestTimeText;

    private void Start()
    {
        DisplayBestTime();
    }

    private void DisplayBestTime()
    {
        string key = "BestTime_Easy";
        if (PlayerPrefs.HasKey(key))
        {
            float bestTime = PlayerPrefs.GetFloat(key);
            int minutes = Mathf.FloorToInt(bestTime / 60);
            int seconds = Mathf.FloorToInt(bestTime % 60);
            bestTimeText.text = string.Format("Best Time : {0:00}:{1:00}", minutes, seconds);
        }
        else
        {
            bestTimeText.text = "Best Time : --:--";
        }
    }
}

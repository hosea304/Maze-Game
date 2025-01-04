using UnityEngine;

public class WinMedium : MonoBehaviour
{
    private bool hasTransitioned = false;

    public void Setup()
    {
        if (!hasTransitioned)
        {
            hasTransitioned = true;
            // Gunakan SceneManagerController untuk berpindah ke Win Medium
            SceneManagerController.selectedDifficulty = SceneManagerController.Difficulty.Medium;
            SceneManagerController controller = FindObjectOfType<SceneManagerController>();
            if (controller != null)
            {
                controller.LoadScene("Win Medium");
            }
            else
            {
                Debug.LogError("SceneManagerController not found in the scene!");
            }
        }
    }

    public void RestartButton()
    {
        // Gunakan SceneManagerController untuk berpindah ke GameplayMedium
        SceneManagerController.selectedDifficulty = SceneManagerController.Difficulty.Medium;
        SceneManagerController controller = FindObjectOfType<SceneManagerController>();
        if (controller != null)
        {
            controller.LoadScene("GameplayMedium");
        }
        else
        {
            Debug.LogError("SceneManagerController not found in the scene!");
        }
    }

    public void ExitButton()
    {
        // Gunakan SceneManagerController untuk berpindah ke Flow1
        SceneManagerController controller = FindObjectOfType<SceneManagerController>();
        if (controller != null)
        {
            controller.LoadScene("Flow1");
        }
        else
        {
            Debug.LogError("SceneManagerController not found in the scene!");
        }
    }
}

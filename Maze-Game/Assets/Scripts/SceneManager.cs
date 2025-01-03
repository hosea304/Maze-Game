using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerController : MonoBehaviour
{
    // Enum to define difficulty levels
    public enum Difficulty
    {
        Easy,
        Medium,
        Hard,
        Extreme
    }

    // Static field to hold the selected difficulty
    public static Difficulty selectedDifficulty = Difficulty.Easy;

    // Method to load a scene by scene name
    public void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    // Method to load a scene by build index
    public void LoadSceneByIndex(int sceneIndex)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
    }

    // Method to load the next scene in the build order
    public void LoadNextScene()
    {
        int currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        UnityEngine.SceneManagement.SceneManager.LoadScene(currentSceneIndex + 1);
    }

    // Method to reload the current scene
    public void ReloadCurrentScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    // Method to load a scene with difficulty setting
    public void LoadSceneWithDifficulty(string sceneName, Difficulty difficulty)
    {
        selectedDifficulty = difficulty;
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}

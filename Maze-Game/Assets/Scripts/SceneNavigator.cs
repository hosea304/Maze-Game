using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigator : MonoBehaviour
{
    [Header("Scene Settings")]
    public string sceneName = "Gameplay"; // Nama scene tujuan default

    /// <summary>
    /// Load scene baru berdasarkan properti yang sudah diatur di Inspector.
    /// </summary>
    public void LoadNewGame()
    {
        LoadSceneByName(sceneName);
    }

    /// <summary>
    /// Load scene baru berdasarkan nama scene yang diberikan sebagai parameter.
    /// </summary>
    /// <param name="newSceneName">Nama scene tujuan.</param>
    public void LoadSceneByName(string newSceneName)
    {
        // Validasi apakah nama scene diberikan
        if (string.IsNullOrEmpty(newSceneName))
        {
            Debug.LogError("Scene name is not set! Please provide a valid scene name.");
            return;
        }

        // Simpan data scene saat ini menggunakan PlayerPrefs
        PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();

        // Navigasi ke scene baru
        Debug.Log("Navigating to scene: " + newSceneName);
        SceneManager.LoadScene(newSceneName);
    }

    /// <summary>
    /// Kembali ke scene sebelumnya berdasarkan data yang tersimpan di PlayerPrefs.
    /// </summary>
    public void LoadPreviousScene()
    {
        if (PlayerPrefs.HasKey("LastScene"))
        {
            string lastScene = PlayerPrefs.GetString("LastScene");
            Debug.Log("Returning to previous scene: " + lastScene);
            SceneManager.LoadScene(lastScene);
        }
        else
        {
            Debug.LogWarning("No previous scene found in PlayerPrefs!");
        }
    }
}

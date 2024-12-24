using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Diperlukan untuk SceneManager

public class SceneNavigator : MonoBehaviour
{
    [Header("Scene Settings")]
    public string sceneName = "GamePlay"; // Nama scene tujuan

    public void LoadNewGame()
    {
        // Validasi apakah nama scene sudah diatur
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name is not set! Please set the sceneName property in the Inspector.");
            return;
        }

        // Simpan data scene saat ini menggunakan PlayerPrefs
        PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();

        // Navigasi ke scene baru
        Debug.Log("Navigating to scene: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }

    public void LoadPreviousScene()
    {
        // Cek apakah data LastScene tersedia
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance; // Singleton Instance
    public string SelectedDifficulty = "Easy"; // Default Difficulty
    public string[] DifficultyOptions = { "Easy", "Medium", "Hard", "Extreme" }; // Semua pilihan difficulty

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Tetap hidup saat pindah scene
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetDifficulty(string difficulty)
    {
        SelectedDifficulty = difficulty;
    }
}
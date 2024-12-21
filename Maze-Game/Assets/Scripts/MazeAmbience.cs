using UnityEngine;
using System.Collections.Generic;

public class MazeAmbience : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource ambienceSource;
    [SerializeField] private AudioSource musicSource;

    [Header("Background Music")]
    [SerializeField] private AudioClip backgroundMusic;

    [Header("Single Ambience Sound")]
    [SerializeField] private AudioClip ambienceSound1;
    [SerializeField] private AudioClip ambienceSound2;
    [SerializeField] private AudioClip ambienceSound3;
    // Tambahkan lebih banyak fields jika diperlukan

    [Header("Volume Settings")]
    [SerializeField]
    [Range(0f, 1f)]
    private float ambienceVolume = 0.3f;
    [SerializeField]
    [Range(0f, 1f)]
    private float musicVolume = 0.2f;

    [Header("Ambience Settings")]
    [SerializeField] private bool useRandomAmbience = true;
    [SerializeField] private float minTimeBetweenSounds = 5f;
    [SerializeField] private float maxTimeBetweenSounds = 15f;

    private List<AudioClip> ambienceSoundsList = new List<AudioClip>();
    private float nextAmbienceTime;

    void Start()
    {
        Debug.Log("MazeAmbience: Starting initialization");

        InitializeAudioSources();
        InitializeAmbienceSounds();
        StartBackgroundMusic();
        SetNextAmbienceTime();
    }

    void InitializeAudioSources()
    {
        if (ambienceSource == null)
        {
            Debug.LogWarning("Creating new ambience source");
            ambienceSource = gameObject.AddComponent<AudioSource>();
            ambienceSource.spatialBlend = 0f;
            ambienceSource.loop = false;
            ambienceSource.volume = ambienceVolume;
        }

        if (musicSource == null)
        {
            Debug.LogWarning("Creating new music source");
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.spatialBlend = 0f;
            musicSource.loop = true;
            musicSource.volume = musicVolume;
        }
    }

    void InitializeAmbienceSounds()
    {
        ambienceSoundsList.Clear();

        // Add non-null ambience sounds to list
        if (ambienceSound1 != null) ambienceSoundsList.Add(ambienceSound1);
        if (ambienceSound2 != null) ambienceSoundsList.Add(ambienceSound2);
        if (ambienceSound3 != null) ambienceSoundsList.Add(ambienceSound3);

        Debug.Log($"Loaded {ambienceSoundsList.Count} ambience sounds");
    }

    void StartBackgroundMusic()
    {
        if (backgroundMusic != null)
        {
            Debug.Log("Starting background music");
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
    }

    void Update()
    {
        if (useRandomAmbience && ambienceSoundsList.Count > 0)
        {
            if (Time.time >= nextAmbienceTime)
            {
                PlayRandomAmbience();
                SetNextAmbienceTime();
            }
        }
    }

    void PlayRandomAmbience()
    {
        if (!ambienceSource.isPlaying && ambienceSoundsList.Count > 0)
        {
            int randomIndex = Random.Range(0, ambienceSoundsList.Count);
            Debug.Log($"Playing ambience sound {randomIndex}");
            ambienceSource.clip = ambienceSoundsList[randomIndex];
            ambienceSource.Play();
        }
    }

    void SetNextAmbienceTime()
    {
        nextAmbienceTime = Time.time + Random.Range(minTimeBetweenSounds, maxTimeBetweenSounds);
    }

    public void SetAmbienceVolume(float volume)
    {
        ambienceVolume = volume;
        if (ambienceSource != null)
        {
            ambienceSource.volume = volume;
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        if (musicSource != null)
        {
            musicSource.volume = volume;
        }
    }
}
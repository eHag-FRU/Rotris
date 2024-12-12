using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// References
// https://gamedevbeginner.com/singletons-in-unity-the-right-way/
// https://github.com/ashaydave/AudioManagerUnity

public class MusicManager : MonoBehaviour
{
    // ========== Singleton ==========
    public static MusicManager Instance { get; private set; }

    // ========== Serialized Fields ==========
    [SerializeField] private AudioSource musicSource; // The audio source for playing music
    [SerializeField] private AudioClip menuMusic; // Music for the menu
    [SerializeField] private AudioClip gameMusic; // Music for the game

    // ========== Private Fields ==========
    private bool isPlayingMenuMusic = true; // Track if currently playing menu music

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // ========== Start Method ==========
    private void Start()
    {
        // Play menu music at the start
        PlayMenuMusic();
    }

    // ========== Play Menu Music ==========
    public void PlayMenuMusic()
    {
        if (musicSource != null && menuMusic != null && !isPlayingMenuMusic)
        {
            musicSource.clip = menuMusic;
            musicSource.loop = true;
            musicSource.Play();
            isPlayingMenuMusic = true;
        }
    }

    // ========== Play Game Music ==========
    public void PlayGameMusic()
    {
        if (musicSource != null && gameMusic != null && isPlayingMenuMusic)
        {
            musicSource.clip = gameMusic;
            musicSource.loop = true;
            musicSource.Play();
            isPlayingMenuMusic = false;
        }
    }

    // ========== Adjust Volume ==========
    public void AdjustVolume(float newVolume)
    {
        if (musicSource != null)
        {
            musicSource.volume = newVolume;
        }
    }
}

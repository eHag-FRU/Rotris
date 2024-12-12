using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [SerializeField] private List<AudioClip> mainGameMusic = new List<AudioClip>(); // List of main game music tracks
    [SerializeField] private AudioClip gameOverMusic; // Music for the Game Over screen

    // ========== Private Fields ==========
    private Queue<AudioClip> shuffledMainGamePlaylist = new Queue<AudioClip>(); // Shuffled playlist
    private bool isPlayingMenuMusic = false; // Track if currently playing menu music
    private bool isPlayingGameOverMusic = false; // Track if currently playing Game Over music
    private float basePitch = 1f; // Base pitch of the music

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to scene loaded event
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe to prevent memory leaks
        }
    }

    // ========== Start Method ==========
    private void Start()
    {
        // Play menu music at the start
        PlayMenuMusic();
    }

    // ========== Scene Loaded Callback ==========
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "TitleScreen":
                PlayMenuMusic();
                break;
            case "Game":
                PlayGameMusic();
                break;
            case "GameOver":
                PlayGameOverMusic();
                break;
            default:
                // Optionally handle other scenes
                break;
        }
    }

    // ========== Play Menu Music ==========
    public void PlayMenuMusic()
    {
        if (musicSource != null && menuMusic != null && !isPlayingMenuMusic)
        {
            StartCoroutine(FadeMusic(menuMusic));
            isPlayingMenuMusic = true;
            isPlayingGameOverMusic = false;
            ResetPitch(); // Reset pitch when switching to menu music
        }
    }

    // ========== Play Random Main Game Music with Shuffle ==========
    public void PlayGameMusic()
    {
        if (musicSource != null && mainGameMusic.Count > 0)
        {
            if (shuffledMainGamePlaylist.Count == 0)
            {
                List<AudioClip> tempList = new List<AudioClip>(mainGameMusic);
                ShuffleList(tempList);
                foreach (var clip in tempList)
                {
                    shuffledMainGamePlaylist.Enqueue(clip);
                }
            }

            AudioClip selectedClip = shuffledMainGamePlaylist.Dequeue();
            StartCoroutine(FadeMusic(selectedClip));
            isPlayingMenuMusic = false;
            isPlayingGameOverMusic = false;
            ResetPitch(); // Reset pitch when switching to main game music
        }
        else
        {
            Debug.LogWarning("[MusicManager] Main game music list is empty or not assigned!");
        }
    }

    // Helper method to shuffle a list
    private void ShuffleList(List<AudioClip> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int rnd = Random.Range(0, i + 1);
            AudioClip temp = list[i];
            list[i] = list[rnd];
            list[rnd] = temp;
        }
    }

    // ========== Play Game Over Music ==========
    public void PlayGameOverMusic()
    {
        if (musicSource != null && gameOverMusic != null && !isPlayingGameOverMusic)
        {
            StartCoroutine(FadeMusic(gameOverMusic));
            isPlayingGameOverMusic = true;
            isPlayingMenuMusic = false;
            ResetPitch(); // Reset pitch when switching to Game Over music
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

    // ========== Reset Pitch ==========
    private void ResetPitch()
    {
        musicSource.pitch = basePitch;
    }

    // ========== Increase Pitch ==========
    public void IncreasePitch(float increment)
    {
        musicSource.pitch += increment;
        // Optional: Clamp the pitch to prevent it from getting too high
        musicSource.pitch = Mathf.Clamp(musicSource.pitch, basePitch, basePitch + 0.1f);
        Debug.Log($"[MusicManager] Music pitch increased to {musicSource.pitch}");
    }

    // ========== Fade Music Coroutine ==========
    private IEnumerator FadeMusic(AudioClip newClip)
    {
        if (musicSource.isPlaying)
        {
            // Fade out current music
            float startVolume = musicSource.volume;
            for (float t = 0f; t < 1f; t += Time.deltaTime)
            {
                musicSource.volume = Mathf.Lerp(startVolume, 0f, t);
                yield return null;
            }
        }

        // Change the clip and play
        musicSource.clip = newClip;
        musicSource.Play();

        // Fade in new music
        for (float t = 0f; t < 1f; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }

        musicSource.volume = 1f;

        // If main game music, set up to play next track when current one ends
        if (mainGameMusic.Contains(newClip))
        {
            musicSource.loop = false;
            StartCoroutine(WaitForTrackEnd(newClip.length));
        }
        else
        {
            musicSource.loop = true; // Loop menu and Game Over music
        }
    }

    // ========== Wait For Track End and Play Next Track ==========
    private IEnumerator WaitForTrackEnd(float trackLength)
    {
        yield return new WaitForSeconds(trackLength);
        // Play the next random main game track
        PlayGameMusic();
    }
}


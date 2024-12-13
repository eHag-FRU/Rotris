using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// References
// https://gamedevbeginner.com/singletons-in-unity-the-right-way/
// https://github.com/ashaydave/AudioManagerUnity
// I believe these helped me figure out the best way to implement dontDestroyOnLoad

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    // ========== Serialized Fields ==========
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private List<AudioClip> mainGameMusic = new List<AudioClip>();
    [SerializeField] private AudioClip gameOverMusic;

    // ========== Private Fields ==========
    private Queue<AudioClip> shuffledPlaylist = new Queue<AudioClip>();
    private bool isPlayingMenuMusic = false;
    private bool isPlayingGameOverMusic = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);                  // Persist across scenes (NOTE TO SELF REMOVE ANY COPIES)
            SceneManager.sceneLoaded += OnSceneLoaded;
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
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void Start()
    {
        PlayMenuMusic();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "TitleScreen":
            case "CreditScreen":
            case "Controls":
            case "Gameplay":
            case "InfoMenu":
                PlayMenuMusic();
                break;
            case "Game":
                PlayGameMusic();
                break;
            case "GameOver":
                PlayGameOverMusic();
                break;
        }
    }

    public void PlayMenuMusic()
    {
        if (musicSource != null && menuMusic != null && !isPlayingMenuMusic)
        {
            StartCoroutine(FadeMusic(menuMusic));
            isPlayingMenuMusic = true;
            isPlayingGameOverMusic = false;
        }
    }

    public void PlayGameMusic()
    {
        if (shuffledPlaylist.Count == 0)
        {
            List<AudioClip> tempList = new List<AudioClip>(mainGameMusic);
            ShuffleList(tempList);
            foreach (var clip in tempList)
            {
                shuffledPlaylist.Enqueue(clip);
            }
        }

        if (shuffledPlaylist.Count > 0)
        {
            AudioClip selectedClip = shuffledPlaylist.Dequeue();
            StartCoroutine(FadeMusic(selectedClip));
            isPlayingMenuMusic = false;
            isPlayingGameOverMusic = false;
        }
    }

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

    public void PlayGameOverMusic()
    {
        if (musicSource != null && gameOverMusic != null && !isPlayingGameOverMusic)
        {
            StartCoroutine(FadeMusic(gameOverMusic));
            isPlayingGameOverMusic = true;
            isPlayingMenuMusic = false;
        }
    }

    public void AdjustVolume(float newVolume)
    {
        if (musicSource != null)
        {
            musicSource.volume = newVolume;
        }
    }

    private IEnumerator FadeMusic(AudioClip newClip)
    {
        if (musicSource.isPlaying)
        {
            float startVolume = musicSource.volume;
            for (float t = 0f; t < 1f; t += Time.deltaTime)
            {
                musicSource.volume = Mathf.Lerp(startVolume, 0f, t);
                yield return null;
            }
        }

        musicSource.clip = newClip;
        musicSource.Play();

        for (float t = 0f; t < 1f; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }

        musicSource.volume = 1f;

        if (mainGameMusic.Contains(newClip))
        {
            musicSource.loop = false;
            StartCoroutine(WaitForTrackEnd(newClip.length));
        }
        else
        {
            musicSource.loop = true;
        }
    }

    private IEnumerator WaitForTrackEnd(float trackLength)
    {
        yield return new WaitForSeconds(trackLength);
        PlayGameMusic();
    }
}

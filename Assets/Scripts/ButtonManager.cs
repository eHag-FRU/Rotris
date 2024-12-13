using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TitleScreen()
    {
        SceneManager.LoadScene("TitleScreen");
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void DisplayInfo()
    {
        SceneManager.LoadScene("InfoMenu");
    }

    public void DisplayControls()
    {
        SceneManager.LoadScene("Controls");
    }

    public void DisplayGameplay()
    {
        SceneManager.LoadScene("Gameplay");
    }

    public void CreditScreen()
    {
        SceneManager.LoadScene("CreditScreen");
    }

    public void ReturnToMainMenu()
    {
        // Reset Score 
        PlayerPrefs.SetInt("FinalScore", 0);
        PlayerPrefs.Save();

        // Play music
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayMenuMusic();
        }

        // Main Menu Scene
        SceneManager.LoadScene("TitleScreen");
    }

    public void RestartGame()
    {
        // Reset Score
        PlayerPrefs.SetInt("FinalScore", 0);
        PlayerPrefs.Save();
        // Play music
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayGameMusic();
        }

        // Game scene
        SceneManager.LoadScene("Game");
    }

        // Quit
    public void QuitGame()
    {
        Application.Quit();
    }
}

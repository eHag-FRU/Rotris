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
        // Optionally, reset the score
        PlayerPrefs.SetInt("FinalScore", 0);
        PlayerPrefs.Save();

        // Play menu music
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayMenuMusic();
        }

        // Load the Main Menu scene
        SceneManager.LoadScene("TitleScreen");
    }

    public void RestartGame()
    {
        // Optionally, reset the score
        PlayerPrefs.SetInt("FinalScore", 0);
        PlayerPrefs.Save();

        // Play game music
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayGameMusic();
        }

        // Load the Game scene
        SceneManager.LoadScene("Game");
    }




    public void QuitGame()
    {
        Application.Quit();
    }
}

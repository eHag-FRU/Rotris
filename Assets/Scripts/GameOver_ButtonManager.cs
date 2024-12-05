using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver_ButtonManager : MonoBehaviour
{
    // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }


    public void retryButton() {
        SceneManager.LoadScene("Game");
    }

    public void mainMenuButton() {
        SceneManager.LoadScene("MainMenu");
    }

    public void quitButton() {
        Application.Quit();
    }
}

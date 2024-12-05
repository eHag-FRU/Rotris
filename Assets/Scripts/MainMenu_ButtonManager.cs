using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu_ButtonManager : MonoBehaviour
{
    // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    public void playButton() {
        SceneManager.LoadScene("Game");
    }

    public void creditsButton() {
        SceneManager.LoadScene("Credits");
    }


    public void quitButton() {
        Application.Quit();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Import TextMeshPro namespace

public class GameOverManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text gameOverText; // Assign in Inspector

    private void Start()
    {
        // Set initial game over text to blank or a default message
        if (gameOverText != null)
        {
            gameOverText.text = "Game Over!"; // Optional customization
        }
    }
}

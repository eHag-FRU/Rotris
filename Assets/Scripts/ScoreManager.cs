using UnityEngine;
using TMPro; // Import TextMeshPro namespace

public class ScoreManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text currentScoreText; // Assign in Inspector

    private void Update()
    {
        currentScoreText.text = "Score: " + Tetris.Score;
    }
}

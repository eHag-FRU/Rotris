using UnityEngine;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text finalScoreText; // Assign in Inspector

    private void Start()
    {
        if (finalScoreText != null)
        {
            int finalScore = PlayerPrefs.GetInt("FinalScore", 0);
            finalScoreText.text = "Final Score: " + finalScore.ToString();
        }
        else
        {
            Debug.LogError("[GameOverUI] finalScoreText is not assigned!");
        }
    }
}

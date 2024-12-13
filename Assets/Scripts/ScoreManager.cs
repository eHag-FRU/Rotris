using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text currentScoreText;

    private void Update()
    {
        currentScoreText.text = "Score: " + Tetris.Score;
    }
}

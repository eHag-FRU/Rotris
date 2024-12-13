using UnityEngine;

public class TopLine : MonoBehaviour
{
    [Tooltip("Tag that should trigger gameover")] // In hindsight I should have made more of these
    public string blockTag = "Block";
    private Tetris tetrisInstance;
    private void Start()
    {
        tetrisInstance = FindObjectOfType<Tetris>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(blockTag))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                if (rb.simulated && rb.gravityScale > 0)
                {
                    if (tetrisInstance != null)
                    {
                        tetrisInstance.TriggerGameOver();
                    }
                }
            }
        }
    }
}

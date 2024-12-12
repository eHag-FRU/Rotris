// TopLine.cs
using UnityEngine;

public class TopLine : MonoBehaviour
{
    [Tooltip("The tag of the blocks that can trigger game over.")]
    public string blockTag = "Block";

    // Reference to Tetris for direct communication
    private Tetris tetrisInstance;

    private void Start()
    {
        // Find the Tetris instance in the scene
        tetrisInstance = FindObjectOfType<Tetris>();
        if (tetrisInstance == null)
        {
            // Optionally, handle the absence of a Tetris instance
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(blockTag))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Ensure the block is locked before checking
                if (rb.simulated && rb.gravityScale > 0)
                {
                    if (tetrisInstance != null)
                    {
                        tetrisInstance.TriggerGameOver();
                    }
                    else
                    {
                        // Optionally, handle the absence of a Tetris instance
                    }
                }
            }
            else
            {
                // Optionally, handle blocks without Rigidbody2D
            }
        }
    }
}

// TopLine.cs
using UnityEngine;

public class TopLine : MonoBehaviour
{
    [Tooltip("The tag of the blocks that can trigger game over.")]
    public string blockTag = "Block";

    // Reference to JellyTetris for direct communication
    private JellyTetris jellyTetris;

    private void Start()
    {
        // Find the JellyTetris instance in the scene
        jellyTetris = FindObjectOfType<JellyTetris>();
        if (jellyTetris == null)
        {
            Debug.LogError("[TopLine] No JellyTetris instance found in the scene!");
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
                    Debug.LogError("[TopLine] Block has reached the Top Line. Game Over!");

                    if (jellyTetris != null)
                    {
                        jellyTetris.TriggerGameOver();
                    }
                    else
                    {
                        Debug.LogError("[TopLine] JellyTetris instance not found. Cannot trigger Game Over.");
                    }
                }
            }
            else
            {
                Debug.LogWarning("[TopLine] Block without Rigidbody2D collided with Top Line.");
            }
        }
    }
}

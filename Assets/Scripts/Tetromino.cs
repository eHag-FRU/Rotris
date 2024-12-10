// Tetromino.cs
using UnityEngine;
using System.Linq;
using System.Collections;

public class Tetromino : MonoBehaviour
{
    public Transform[] Blocks { get; private set; }
    public bool IsLocked { get; private set; } = false;

    [Header("Jelly Drop Effect Settings")]
    public float jellyRotationRange = 5f;
    public float jellyScaleRange = 0.05f;
    public float lockDelay = 0.2f;

    private BoxCollider2D[] blockColliders;
    private bool collided = false;
    private bool lockTimerStarted = false;

    private void Awake()
    {
        Blocks = GetComponentsInChildren<Transform>()
            .Where(t => t != transform)
            .ToArray();

        blockColliders = Blocks.Select(b => b.GetComponent<BoxCollider2D>())
                               .Where(col => col != null).ToArray();
    }

    public void Initialize(float gravityScale, PhysicsMaterial2D blockPhysicsMaterial = null)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();

        rb.isKinematic = false;
        rb.gravityScale = gravityScale;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.bodyType = RigidbodyType2D.Dynamic;
        if (blockPhysicsMaterial != null) rb.sharedMaterial = blockPhysicsMaterial;

        Debug.Log("[Tetromino] Initialized with properties.");
    }

    public void TryMove(Vector3 direction)
    {
        if (IsLocked) return;

        // Apply movement
        Vector3 newPosition = transform.position + direction;

        // Clamp the new position within the boundaries
        newPosition.x = Mathf.Clamp(newPosition.x, -4f, 4f);
        newPosition.y = Mathf.Clamp(newPosition.y, -9f, 9f);

        // Update position
        transform.position = newPosition;
    }

    public void TryRotate(float angle)
    {
        if (IsLocked) return;

        // Save the original rotation
        Quaternion originalRotation = transform.rotation;
        transform.Rotate(0, 0, angle);

        if (!IsValidPosition())
        {
            // If rotation is invalid, try shifting horizontally
            if (TryShift(Vector3.right) || TryShift(Vector3.left))
            {
                return; // If shifting works, keep the rotation
            }
            transform.rotation = originalRotation; // Revert rotation if no shift works
        }
    }

    private bool TryShift(Vector3 direction)
    {
        transform.position += direction;
        if (IsValidPosition()) return true;

        transform.position -= direction; // Revert shift
        return false;
    }

    private bool IsValidPosition()
    {
        foreach (Transform block in Blocks)
        {
            Vector3 pos = block.position;

            // Check if block is within valid boundaries
            if (pos.x < -4f || pos.x > 4f || pos.y < -9f || pos.y > 9f)
            {
                return false;
            }
        }
        return true;
    }

    public void EnableAllBlockColliders()
    {
        foreach (var col in blockColliders)
        {
            col.enabled = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsLocked) return;

        if (collision.gameObject.CompareTag("BottomGrid") || collision.gameObject.CompareTag("NewPiece") || collision.gameObject.CompareTag("TopGrid"))
        {
            if (collision.gameObject.CompareTag("TopGrid"))
            {
                Debug.LogError("GameOver: Touched the top grid boundary!");
                return;
            }

            if (!collided)
            {
                collided = true;
                ApplyJellyDropEffect();
                StartLockDelayCountdown();
            }
        }

        // New Logic: Lock block if it touches another block
        if (collision.gameObject.CompareTag("Block"))
        {
            Debug.Log("[Tetromino] Touched another block, locking.");
            LockPiece();
        }
    }

    private void ApplyJellyDropEffect()
    {
        Debug.Log("[Tetromino] Applying jelly drop effect.");
        float randomRotation = Random.Range(-jellyRotationRange, jellyRotationRange);
        transform.Rotate(0, 0, randomRotation);

        float scaleChange = 1.0f + Random.Range(-jellyScaleRange, jellyScaleRange);
        transform.localScale *= scaleChange;
    }

    private void StartLockDelayCountdown()
    {
        if (lockTimerStarted) return;
        lockTimerStarted = true;
        StartCoroutine(LockAfterDelay(lockDelay));
    }

    private IEnumerator LockAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        LockPiece();
    }

    private void LockPiece()
    {
        Debug.Log("[Tetromino] Locking piece after lock delay.");
        IsLocked = true;

        foreach (Transform block in Blocks)
        {
            block.rotation = transform.rotation;
            block.parent = null;

            Rigidbody2D blockRb = block.GetComponent<Rigidbody2D>();
            if (blockRb != null)
            {
                blockRb.velocity = Vector2.zero;
                blockRb.angularVelocity = 0f;
                blockRb.simulated = true; // Enable simulation after locking
                blockRb.gravityScale = 1f;
            }

            block.tag = "Block";
        }

        Destroy(gameObject);

        JellyTetris jt = FindObjectOfType<JellyTetris>();
        if (jt != null)
        {
            jt.LockTetromino(this);
        }
        else
        {
            Debug.LogError("[Tetromino] No JellyTetris found, cannot lock.");
        }
    }
}

// Tetromino.cs
using UnityEngine;
using System.Collections;

public class Tetromino : MonoBehaviour
{
    public Transform[] Blocks { get; private set; }
    public bool IsLocked { get; private set; } = false;

    [Header(" Drop Effect Settings")]
    public float RotationRange = 5f;
    public float ScaleRange = 0.05f;
    public float lockDelay = 0.2f;

    [Header("Rotation Settings")]
    [Tooltip("Time in seconds to complete a rotation.")]
    public float rotationDuration = 0.3f;

    [Header(" Joint Settings")]
    [Tooltip("Spring constant for the joints.")]
    public float spring = 50f;

    [Tooltip("Damping ratio for the joints.")]
    public float damping = 5f;

    [Tooltip("Maximum distance allowed between connected blocks.")]
    public float distance = 0.5f;

    private BoxCollider2D[] blockColliders;
    private bool collided = false;
    private bool lockTimerStarted = false;
    private bool isRotating = false; // Flag to prevent multiple rotations

    private Rigidbody2D rb;

    private void Awake()
    {
        // Initialize Blocks array excluding the parent transform
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        int blockCount = 0;
        foreach (Transform child in allChildren)
        {
            if (child != this.transform)
                blockCount++;
        }

        Blocks = new Transform[blockCount];
        int index = 0;
        foreach (Transform child in allChildren)
        {
            if (child != this.transform)
            {
                Blocks[index] = child;
                index++;
            }
        }

        // Initialize blockColliders array
        blockColliders = new BoxCollider2D[Blocks.Length];
        for (int i = 0; i < Blocks.Length; i++)
        {
            BoxCollider2D col = Blocks[i].GetComponent<BoxCollider2D>();
            if (col != null)
            {
                blockColliders[i] = col;
            }
            else
            {
                // Optionally, add a BoxCollider2D if missing
                col = Blocks[i].gameObject.AddComponent<BoxCollider2D>();
                blockColliders[i] = col;
            }
        }

        // Initialize Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        // Initialize joints between blocks
        InitializeJoints();
    }

    public void Initialize(float gravityScale, PhysicsMaterial2D blockPhysicsMaterial = null)
    {
        rb.isKinematic = false;
        rb.gravityScale = gravityScale;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.freezeRotation = false; // Allow rotation

        if (blockPhysicsMaterial != null)
            rb.sharedMaterial = blockPhysicsMaterial;

        Debug.Log("[Tetromino] Initialized with properties.");
    }

    private void InitializeJoints()
    {
        // Ensure all blocks have Rigidbody2D components
        foreach (var block in Blocks)
        {
            Rigidbody2D blockRb = block.GetComponent<Rigidbody2D>();
            if (blockRb == null)
            {
                blockRb = block.gameObject.AddComponent<Rigidbody2D>();
                blockRb.bodyType = RigidbodyType2D.Dynamic;
                blockRb.gravityScale = rb.gravityScale;
                blockRb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                blockRb.freezeRotation = false;
            }
        }

        // Connect each block to its immediate neighbors
        for (int i = 0; i < Blocks.Length; i++)
        {
            for (int j = i + 1; j < Blocks.Length; j++)
            {
                float currentDistance = Vector3.Distance(Blocks[i].localPosition, Blocks[j].localPosition);
                if (currentDistance <= 1.0f) // Adjust this threshold based on block arrangement
                {
                    SpringJoint2D joint = Blocks[i].gameObject.AddComponent<SpringJoint2D>();
                    Rigidbody2D connectedRb = Blocks[j].GetComponent<Rigidbody2D>();

                    joint.connectedBody = connectedRb;
                    joint.autoConfigureDistance = false;
                    joint.distance = distance;
                    joint.frequency = spring;
                    joint.dampingRatio = damping;

                    // Optionally, adjust joint break force or other properties
                     joint.breakForce = 5000;
                     joint.breakTorque = 1000;
                }
            }
        }
    }

    public void TryMove(Vector3 direction)
    {
        if (IsLocked) return;

        // Apply movement using Rigidbody2D for physics-based movement
        rb.MovePosition(rb.position + (Vector2)direction);
    }

    public void TryRotate(float angle)
    {
        if (IsLocked || isRotating) return;

        // Start a coroutine to rotate smoothly
        StartCoroutine(RotateOverTime(angle));
    }

    private IEnumerator RotateOverTime(float angle)
    {
        isRotating = true;

        float elapsed = 0f;
        float startRotation = rb.rotation;
        float endRotation = startRotation + angle;

        while (elapsed < rotationDuration)
        {
            float newRotation = Mathf.Lerp(startRotation, endRotation, elapsed / rotationDuration);
            rb.MoveRotation(newRotation);
            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.MoveRotation(endRotation);

        // Check if the new rotation is valid
        if (!IsValidPosition())
        {
            // If invalid, revert rotation
            rb.MoveRotation(startRotation);
        }

        isRotating = false;
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

            // Additional collision checks can be implemented here
            // For example, check if the block overlaps with existing blocks
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
                ApplyDropEffect();
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

    private void ApplyDropEffect()
    {
        Debug.Log("[Tetromino] Applying  drop effect.");
        float randomRotation = Random.Range(-RotationRange, RotationRange);
        StartCoroutine(RotateDrop(randomRotation));

        float scaleChange = 1.0f + Random.Range(-ScaleRange, ScaleRange);
        StartCoroutine(ScaleDrop(scaleChange));
    }

    private IEnumerator RotateDrop(float angle)
    {
        float elapsed = 0f;
        float duration = 0.2f; // Duration for  rotation effect
        float startRotation = rb.rotation;
        float endRotation = startRotation + angle;

        while (elapsed < duration)
        {
            float newRotation = Mathf.Lerp(startRotation, endRotation, elapsed / duration);
            rb.MoveRotation(newRotation);
            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.MoveRotation(endRotation);
    }

    private IEnumerator ScaleDrop(float scaleChange)
    {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * scaleChange;
        float elapsed = 0f;
        float duration = 0.2f;

        while (elapsed < duration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;

        // Revert to original scale
        elapsed = 0f;
        while (elapsed < duration)
        {
            transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;
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

        Tetris jt = FindObjectOfType<Tetris>();
        if (jt != null)
        {
            jt.LockTetromino(this);
        }
        else
        {
            Debug.LogError("[Tetromino] No Tetris found, cannot lock.");
        }
    }
}

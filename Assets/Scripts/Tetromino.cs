using UnityEngine;
using System.Collections;

// Resources:
// https://docs.unity3d.com/6000.0/Documentation/ScriptReference/MonoBehaviour.StartCoroutine.html
// https://stackoverflow.com/questions/70538568/unity-startcoroutine
// https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnDrawGizmos.html
public class Tetromino : MonoBehaviour
{
    public Transform[] Blocks { get; private set; }
    public bool IsLocked { get; private set; } = false;

    [Header(" Drop Effect Settings")]
    public float RotationRange = 5f;
    public float ScaleRange = 0.05f;
    public float lockDelay = 0.2f;

    [Header("Rotation Settings")]
    public float rotationDuration = 0.3f;

    [Header(" Joint Settings")]
    public float spring = 50f;

    public float damping = 5f;

    public float distance = 0.5f;

    private BoxCollider2D[] blockColliders;
    private bool collided = false;
    private bool lockTimerStarted = false;
    private bool isRotating = false; // prevent multiple rotations

    private Rigidbody2D rb;

    private void Awake()
    {
        // Initialize Blocks, major step in using the nested blocks
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
        blockColliders = new BoxCollider2D[Blocks.Length];
        for (int i = 0; i < Blocks.Length; i++)
        {
            BoxCollider2D col = Blocks[i].GetComponent<BoxCollider2D>();
            if (col != null)
            {
                blockColliders[i] = col;
            }
            else // Safety Measure Block
            {
                col = Blocks[i].gameObject.AddComponent<BoxCollider2D>();
                blockColliders[i] = col;
            }
        }

        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        // joints between blocks
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
    }

    private void InitializeJoints()
    {
        // Ensure all blocks start with rigidb
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

        // Connect each block
        for (int i = 0; i < Blocks.Length; i++)
        {
            for (int j = i + 1; j < Blocks.Length; j++)
            {
                float currentDistance = Vector3.Distance(Blocks[i].localPosition, Blocks[j].localPosition);
                if (currentDistance <= 1.0f) 
                {
                    SpringJoint2D joint = Blocks[i].gameObject.AddComponent<SpringJoint2D>();
                    Rigidbody2D connectedRb = Blocks[j].GetComponent<Rigidbody2D>();

                    joint.connectedBody = connectedRb;
                    joint.autoConfigureDistance = false;
                    joint.distance = distance;
                    joint.frequency = spring;
                    joint.dampingRatio = damping;

                    // Helpful Unity Link: https://docs.unity3d.com/Manual/class-SpringJoint.html
                    // This allows joints to be broken
                     joint.breakForce = 5000;
                     joint.breakTorque = 1000;
                }
            }
        }
    }

    public void TryMove(Vector3 direction)
    {
        if (IsLocked) return;
        rb.MovePosition(rb.position + (Vector2)direction);
    }

    public void TryRotate(float angle)
    {
        if (IsLocked || isRotating) return;
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
                return;
            }

            if (!collided)
            {
                collided = true;
                ApplyDropEffect();
                StartLockDelayCountdown();
            }
        }

        if (collision.gameObject.CompareTag("Block"))
        {
            Debug.Log("Touched another block, locking.");
            LockPiece();
        }
    }

    private void ApplyDropEffect()
    {
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
    }
}

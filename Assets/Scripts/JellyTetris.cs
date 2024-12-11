// JellyTetris.cs
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management

public class JellyTetris : MonoBehaviour
{
    [Header("Tetromino Prefabs")]
    public Tetromino[] tetrominoPrefabs;
    public Transform spawnPoint;

    [Header("Physics")]
    public float gravityScale = 0.3f;
    public PhysicsMaterial2D blockPhysicsMaterial;

    [Header("Row Settings")]
    [Tooltip("Total number of rows in the playable area.")]
    public int totalRows = 20; // e.g., 20 for the grid height
    [Tooltip("Total vertical space allocated to each row.")]
    public float rowHeight = 0.5f;
    [Tooltip("Height used for actual block overlap checks. Should be smaller than rowHeight to create a gap.")]
    public float rowCheckHeight = 0.4f;
    [Tooltip("Number of blocks required per row to clear it.")]
    public int blocksPerRow = 10;

    [Header("Grid Bounds")]
    [Tooltip("Bottom-most Y position of the grid.")]
    public float startY = -9f;
    [Tooltip("Top-most Y position of the grid (for reference).")]
    public float endY = 9f;
    [Tooltip("Horizontal width of each row for collision checking.")]
    public float rowWidth = 10f;

    [Header("Timing")]
    [Tooltip("Delay before checking and clearing rows after a tetromino locks in place.")]
    public float settleDelay = 1f;

    [Header("Glow Settings")]
    [Tooltip("Duration for which blocks glow before being cleared.")]
    public float glowDuration = 0.5f;

    [Tooltip("Color to apply to blocks when they glow.")]
    public Color glowColor = Color.yellow;

    [Header("Fail Line Settings")]
    [Tooltip("Transform of the Fail Line.")]
    public Transform failLine;

    [Tooltip("Collider component of the Fail Line.")]
    public Collider2D failLineCollider;

    [Header("Fail Line Movement Settings")]
    [Tooltip("Amount the Fail Line moves down when moving down.")]
    [SerializeField]
    [Range(0.1f, 5f)]
    private float failLineMoveDownAmount = 1f; // Adjustable in Inspector

    private int failLineRowIndex = 0; // Tracks the current row index of the Fail Line
    private Collider2D[][] rowBlockArrays; // Each row contains an array of colliders
    private bool isGameOver = false;
    private bool isCheckingRows = false;

    // Scoring
    public static int Score { get; private set; } = 0;

    private void Start()
    {
        Debug.Log("[JellyTetris] Initializing...");
        InitializeRows();
        SpawnTetromino();
        InitializeFailLine(); // Initialize Fail Line
    }

    private void InitializeRows()
    {
        rowBlockArrays = new Collider2D[totalRows][];
        for (int i = 0; i < totalRows; i++)
        {
            rowBlockArrays[i] = new Collider2D[0];
        }
    }

    public void AddBlockToRow(Collider2D blockCollider)
    {
        // Determine which row this block belongs to.
        int rowIndex = Mathf.FloorToInt((blockCollider.transform.position.y - startY) / rowHeight);

        if (rowIndex >= 0 && rowIndex < totalRows)
        {
            // Create a new array with one additional slot
            Collider2D[] newRow = new Collider2D[rowBlockArrays[rowIndex].Length + 1];
            for (int i = 0; i < rowBlockArrays[rowIndex].Length; i++)
            {
                newRow[i] = rowBlockArrays[rowIndex][i];
            }
            newRow[newRow.Length - 1] = blockCollider;
            rowBlockArrays[rowIndex] = newRow;

            // If the row is immediately full, we won't clear it right away.
            // It will be cleared during the scheduled settle check.
        }
    }

    private IEnumerator ClearRowCoroutine(Collider2D[] rowBlocks)
    {
        // Apply glow to all blocks in the row
        foreach (var block in rowBlocks)
        {
            if (block != null && block.gameObject.CompareTag("Block"))
            {
                SpriteRenderer sr = block.GetComponent<SpriteRenderer>();
                if (sr != null && sr.material.HasProperty("_EmissionColor"))
                {
                    sr.material.EnableKeyword("_EMISSION");
                    sr.material.SetColor("_EmissionColor", glowColor);
                }
                else if (sr != null)
                {
                    sr.color = glowColor; // Fallback if emission is not available
                }
                else
                {
                    Debug.LogWarning("Block missing SpriteRenderer for glow.");
                }
            }
        }

        // Wait for the glow duration
        yield return new WaitForSeconds(glowDuration);

        // Proceed to destroy the blocks
        float clearedRowY = rowBlocks[0].transform.position.y;

        foreach (var block in rowBlocks)
        {
            if (block != null && block.gameObject.CompareTag("Block"))
            {
                Destroy(block.gameObject);
            }
        }

        // Update score
        Score += 100; // Award 100 points per cleared row
        Debug.Log($"[JellyTetris] Row cleared. Score: {Score}");

        EnableSimulationAboveRow(clearedRowY);
        Debug.Log("[JellyTetris] Row cleared.");
    }

    private void InitializeFailLine()
    {
        if (failLine == null)
        {
            Debug.LogError("[JellyTetris] Fail Line is not assigned!");
            return;
        }

        if (failLineCollider == null)
        {
            failLineCollider = failLine.GetComponent<Collider2D>();
            if (failLineCollider == null)
            {
                Debug.LogError("[JellyTetris] Fail Line does not have a Collider2D!");
                return;
            }
        }

        // Initialize failLineRowIndex based on its starting position
        failLineRowIndex = Mathf.FloorToInt((failLine.position.y - startY) / rowHeight);
        Debug.Log($"[JellyTetris] Initialized Fail Line at row index: {failLineRowIndex}");
    }

    private void MoveFailLineUp()
    {
        if (failLine == null) return;

        failLineRowIndex = Mathf.Clamp(failLineRowIndex + 1, 0, totalRows - 1);
        Vector3 newPosition = new Vector3(
            failLine.position.x,
            startY + failLineRowIndex * rowHeight, // Moving up by rowHeight
            failLine.position.z
        );
        failLine.position = newPosition;
        Debug.Log($"[JellyTetris] Moved Fail Line up to row index: {failLineRowIndex}");

        // Check if Fail Line has reached the top
        if (failLineRowIndex >= totalRows - 1)
        {
            Debug.LogError("[JellyTetris] Fail Line has reached the top. Game Over!");
            isGameOver = true;
            HandleGameOver();
        }
    }

    private void MoveFailLineDown()
    {
        if (failLine == null) return;

        failLineRowIndex = Mathf.Clamp(failLineRowIndex - 1, 0, totalRows - 1);
        Vector3 newPosition = new Vector3(
            failLine.position.x,
            startY + failLineRowIndex * rowHeight - failLineMoveDownAmount, // Adjusted downward movement
            failLine.position.z
        );
        failLine.position = newPosition;
        Debug.Log($"[JellyTetris] Moved Fail Line down by {failLineMoveDownAmount} to row index: {failLineRowIndex}");

        // Check if Fail Line has reached the top
        if (failLineRowIndex >= totalRows - 1)
        {
            Debug.LogError("[JellyTetris] Fail Line has reached the top. Game Over!");
            isGameOver = true;
            HandleGameOver();
        }
    }

    private IEnumerator SettleAndClearRows()
    {
        isCheckingRows = true;
        yield return new WaitForSeconds(settleDelay);

        Debug.Log($"[JellyTetris] Blocks required per row to clear: {blocksPerRow}");

        bool anyRowCleared = false;

        // Iterate through each row to check for clearing
        for (int rowIndex = 0; rowIndex < totalRows; rowIndex++)
        {
            float rowY = startY + rowIndex * rowHeight;

            // Collect all colliders in the current row's check range
            Collider2D[] allRowColliders = Physics2D.OverlapBoxAll(
                new Vector2(0, rowY),
                new Vector2(rowWidth, rowCheckHeight),
                0f
            );

            // Manually filter colliders to include only those with the "Block" tag
            int blockCount = 0;
            Collider2D[] rowBlocksTemp = new Collider2D[allRowColliders.Length];
            int tempIndex = 0;

            for (int i = 0; i < allRowColliders.Length; i++)
            {
                if (allRowColliders[i].CompareTag("Block"))
                {
                    rowBlocksTemp[tempIndex] = allRowColliders[i];
                    blockCount++;
                    tempIndex++;
                }
            }

            // Resize the array to the actual number of blocks
            Collider2D[] rowBlocks = new Collider2D[blockCount];
            for (int i = 0; i < blockCount; i++)
            {
                rowBlocks[i] = rowBlocksTemp[i];
            }

            Debug.Log($"[JellyTetris] Row {rowIndex} at {rowY} contains {rowBlocks.Length} blocks with tag 'Block'.");

            // Clear the row if the block count meets or exceeds the threshold
            if (rowBlocks.Length >= blocksPerRow)
            {
                Debug.Log($"[JellyTetris] Clearing row at height: {rowY}");
                yield return StartCoroutine(ClearRowCoroutine(rowBlocks));
                anyRowCleared = true;
                MoveFailLineUp(); // Move Fail Line up when a row is cleared
                // After clearing, blocks above will settle due to gravity
            }
        }

        // After checking all rows, decide whether to move the Fail Line down
        if (!anyRowCleared)
        {
            MoveFailLineDown(); // Move Fail Line down if no rows were cleared
        }

        // If the game isn't over, spawn a new Tetromino
        if (!isGameOver)
            SpawnTetromino();

        isCheckingRows = false;
    }

    private void EnableSimulationAboveRow(float clearedRowY)
    {
        Rigidbody2D[] allRigidbodies = FindObjectsOfType<Rigidbody2D>();
        foreach (Rigidbody2D rb in allRigidbodies)
        {
            if (rb.CompareTag("Block") && rb.transform.position.y > clearedRowY)
            {
                rb.simulated = true;
            }
        }
    }

    private Tetromino GetActiveTetromino()
    {
        Tetromino[] allTetrominoes = FindObjectsOfType<Tetromino>();
        foreach (Tetromino t in allTetrominoes)
        {
            if (!t.IsLocked)
                return t;
        }
        return null;
    }

    private void Update()
    {
        if (isGameOver) return;

        Tetromino activeTetromino = GetActiveTetromino();
        if (activeTetromino == null) return;

        HandleInput(activeTetromino);
    }

    private void HandleInput(Tetromino activeTetromino)
    {
        if (activeTetromino.IsLocked) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            activeTetromino.TryMove(Vector3.left);

        if (Input.GetKeyDown(KeyCode.RightArrow))
            activeTetromino.TryMove(Vector3.right);

        if (Input.GetKeyDown(KeyCode.Q))
            activeTetromino.TryRotate(-90);

        if (Input.GetKeyDown(KeyCode.E))
            activeTetromino.TryRotate(90);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Rigidbody2D rb = activeTetromino.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = gravityScale * 10f;
            }
        }
    }

    public void SpawnTetromino()
    {
        if (tetrominoPrefabs == null || tetrominoPrefabs.Length == 0)
        {
            Debug.LogError("[JellyTetris] No Tetromino prefabs assigned!");
            return;
        }

        int index = Random.Range(0, tetrominoPrefabs.Length);
        Debug.Log($"[JellyTetris] Selected Tetromino prefab index: {index}");

        Vector3 spawnPosition = spawnPoint.position;
        spawnPosition.x = Mathf.Clamp(spawnPosition.x, -4f, 4f);
        spawnPosition.y = Mathf.Clamp(spawnPosition.y, 5f, 9f); // Adjusted y-clamp

        Debug.Log($"[JellyTetris] Spawning Tetromino at: {spawnPosition}");

        Tetromino newTetromino = Instantiate(tetrominoPrefabs[index], spawnPosition, Quaternion.identity);
        if (newTetromino != null)
        {
            Debug.Log("[JellyTetris] Successfully instantiated Tetromino.");
            newTetromino.Initialize(gravityScale, blockPhysicsMaterial);
        }
        else
        {
            Debug.LogError("[JellyTetris] Failed to instantiate Tetromino.");
        }
    }

    public void LockTetromino(Tetromino tetromino)
    {
        if (tetromino == null || isGameOver) return;

        Debug.Log("[JellyTetris] Locking Tetromino.");
        tetromino.EnableAllBlockColliders();

        foreach (Transform block in tetromino.Blocks)
        {
            if (block.position.y > endY)
            {
                Debug.LogError("GameOver: A block locked out of bounds!");
                isGameOver = true;
                HandleGameOver();
                return;
            }
        }

        // Instead of clearing rows immediately, we wait for the settle delay
        if (!isCheckingRows)
        {
            StartCoroutine(SettleAndClearRows());
        }
    }

    public void TriggerGameOver()
    {
        if (!isGameOver)
        {
            isGameOver = true;
            HandleGameOver();
        }
    }

    private void HandleGameOver()
    {
        Debug.Log("[JellyTetris] Game Over!");

        // Save the score before switching scenes
        PlayerPrefs.SetInt("FinalScore", Score);
        PlayerPrefs.Save();

        // Load the Game Over scene (ensure it's added to Build Settings)
        SceneManager.LoadScene("GameOverScene");
    }

    private void OnDrawGizmos()
    {
        if (rowBlockArrays == null) return; // Exit if rows are not initialized

        // Draw a gizmo for each row. Use rowCheckHeight to show the actual detection area.
        for (int rowIndex = 0; rowIndex < totalRows; rowIndex++)
        {
            float rowY = startY + rowIndex * rowHeight;

            // Determine color based on row fullness
            if (rowBlockArrays[rowIndex] != null && rowBlockArrays[rowIndex].Length >= blocksPerRow)
                Gizmos.color = Color.green;  // Full row
            else
                Gizmos.color = Color.gray;   // Incomplete row

            // Draw the detection zone for this row as a smaller box than the full row height
            // to reflect the gap.
            Gizmos.DrawWireCube(
                new Vector3(0, rowY, 0),
                new Vector3(rowWidth, rowCheckHeight, 0)
            );
        }
    }
}
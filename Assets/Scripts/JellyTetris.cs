// JellyTetris.cs
using System.Collections;
using UnityEngine;

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

    private Collider2D[][] rowBlockArrays; // Each row contains an array of colliders
    private bool isGameOver = false;
    private bool isCheckingRows = false;

    private void Start()
    {
        Debug.Log("[JellyTetris] Initializing...");
        InitializeRows();
        SpawnTetromino();
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

        EnableSimulationAboveRow(clearedRowY);
        Debug.Log("[JellyTetris] Row cleared.");
    }

    private IEnumerator SettleAndClearRows()
    {
        isCheckingRows = true;
        yield return new WaitForSeconds(settleDelay);

        Debug.Log($"[JellyTetris] Blocks required per row to clear: {blocksPerRow}");

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
                // After clearing, blocks above will settle due to gravity
            }
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

        Vector3 spawnPosition = spawnPoint.position;
        spawnPosition.x = Mathf.Clamp(spawnPosition.x, -4f, 4f);
        spawnPosition.y = Mathf.Clamp(spawnPosition.y, -9f, 9f);

        Tetromino newTetromino = Instantiate(tetrominoPrefabs[index], spawnPosition, Quaternion.identity);
        newTetromino.Initialize(gravityScale, blockPhysicsMaterial);

        Debug.Log("[JellyTetris] Spawned a new Tetromino at: " + spawnPosition);
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

    private void HandleGameOver()
{
    Debug.Log("[JellyTetris] Game Over!");

    // Optionally, display a Game Over UI, stop the game, etc.
    // Example: Time.timeScale = 0f;

    // Destroy all active blocks from rowBlockArrays
    foreach (Collider2D[] row in rowBlockArrays)
    {
        if (row != null)
        {
            foreach (Collider2D collider in row)
            {
                if (collider != null)
                {
                    Destroy(collider.gameObject);
                }
            }
        }
    }

    // Additionally, destroy all blocks tagged as "Block" in the scene
    Collider2D[] allBlocks = FindObjectsOfType<Collider2D>();
    foreach (Collider2D block in allBlocks)
    {
        if (block.CompareTag("Block"))
        {
            Destroy(block.gameObject);
        }
    }
}


    private void OnDrawGizmos()
    {
        if (rowBlockArrays == null) return; // Exit if rows are not initialized

        // Draw a gizmo for each row. Use rowCheckHeight to show the actual detection area.
        for (int rowIndex = 0; rowIndex < totalRows; rowIndex++)
        {
            float rowY = startY + rowIndex * rowHeight;

            // Determine color based on row fullness
            if (rowBlockArrays[rowIndex] != null && rowBlockArrays[rowIndex].Length == blocksPerRow)
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

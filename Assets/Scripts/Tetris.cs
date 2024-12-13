using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class Tetris : MonoBehaviour
{
    [Header("Tetromino Prefabs")]
    public Tetromino[] tetrominoPrefabs;
    public Transform spawnPoint;

    [Header("Physics")]
    public float gravityScale = 0.3f;
    public PhysicsMaterial2D blockPhysicsMaterial;

    [Header("Row Settings")]
    public int totalRows = 20;
    public float rowHeight = 0.5f;
    public float rowCheckHeight = 0.4f;
    public int blocksPerRow = 10;

    [Header("Grid Bounds")]
    public float startY = -9f;
    public float endY = 9f;
    public float rowWidth = 10f;

    [Header("Timing")]
    public float settleDelay = 1f;

    [Header("Glow Settings")]
    public float glowDuration = 0.5f;
    public Color glowColor = Color.yellow;

    [Header("Line Settings")]
    public Transform failLine;

    public Collider2D failLineCollider;

    [Header("Line Movement: Down")]
    [SerializeField]
    private float LineMovemovementDown = 1f;                // Adjustable

    [Header("Line Movement: Up")]
    [SerializeField]
    private float LineMovemovementUp = 1f;                  // Adjustable (Could change to 2 for easy play)
    private Collider2D[][] rowBlockArrays;                    // Each row contains an array of colliders, Line up 10 = clear
    private bool isGameOver = false;
    private bool isCheckingRows = false;

    // Scoring - I am not going crazy on this part. Clear = 100
    public static int Score { get; private set; } = 0;

    private void Start()
    {
        InitializeRows();
        SpawnTetromino();
        InitializeFailLine(); 
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
        // Determine which row a block belongs to.
        int rowIndex = Mathf.FloorToInt((blockCollider.transform.position.y - startY) / rowHeight);

        if (rowIndex >= 0 && rowIndex < totalRows)
        {
            // Create array
            Collider2D[] newRow = new Collider2D[rowBlockArrays[rowIndex].Length + 1];
            for (int i = 0; i < rowBlockArrays[rowIndex].Length; i++)
            {
                newRow[i] = rowBlockArrays[rowIndex][i];
            }
            newRow[newRow.Length - 1] = blockCollider;
            rowBlockArrays[rowIndex] = newRow;

            // If row is full wait for check
        }
    }

    private IEnumerator ClearRowCoroutine(Collider2D[] rowBlocks)
    {
        // Apply glow - Satisfying effects. Fun fact, I put a similar version in to debug at first
          // Apply glow to all blocks in the row
        foreach (var block in rowBlocks)
        {
            if (block != null && block.gameObject.CompareTag("Block"))
            {
                SpriteRenderer sr = block.GetComponent<SpriteRenderer>();
                sr.color = glowColor;
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
        Debug.Log($"[Tetris] Row cleared. Score: {Score}");

        EnableSimulationAboveRow(clearedRowY);
        Debug.Log("[Tetris] Row cleared.");
    }

    private void InitializeFailLine()
    {
        if (failLine == null)
        {
            Debug.LogError("[Tetris] Fail Line is not assigned!");
            return;
        }

        if (failLineCollider == null)
        {
            failLineCollider = failLine.GetComponent<Collider2D>();
            if (failLineCollider == null)
            {
                Debug.LogError("[Tetris] Fail Line does not have a Collider2D!");
                return;
            }
        }
    }

    private void MoveFailLineUp()
    {
        Vector3 newPosition = new Vector3(
            failLine.position.x,
            failLine.position.y + LineMovemovementUp, // Up
            failLine.position.z
        );
        failLine.position = newPosition;
    }

    private void MoveFailLineDown()
    {
        Vector3 newPosition = new Vector3(
            failLine.position.x,
            failLine.position.y - LineMovemovementDown, //Down
            failLine.position.z
        );
        failLine.position = newPosition;

    }

    private IEnumerator SettleAndClearRows()
    {
        isCheckingRows = true;
        yield return new WaitForSeconds(settleDelay);

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

            Debug.Log($"[Tetris] Row {rowIndex} at {rowY} contains {rowBlocks.Length} blocks with tag 'Block'.");

            // Clear the row if the block count meets or exceeds the threshold
            if (rowBlocks.Length >= blocksPerRow)
            {
                Debug.Log($"[Tetris] Clearing row at height: {rowY}");
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
            return;
        }
        int index = Random.Range(0, tetrominoPrefabs.Length);

        Vector3 spawnPosition = spawnPoint.position;
        spawnPosition.x = Mathf.Clamp(spawnPosition.x, -4f, 4f);
        spawnPosition.y = Mathf.Clamp(spawnPosition.y, 5f, 9f); 

        Debug.Log($"[Tetris] Spawning Tetromino at: {spawnPosition}");

        Tetromino newTetromino = Instantiate(tetrominoPrefabs[index], spawnPosition, Quaternion.identity);
        if (newTetromino != null)
        {
            Debug.Log("[Tetris] Successfully instantiated Tetromino."); // Ensuring that the piece actually spawns
            newTetromino.Initialize(gravityScale, blockPhysicsMaterial);
        }
        else
        {
            Debug.LogError("[Tetris] Failed to instantiate Tetromino.");
        }
    }

    public void LockTetromino(Tetromino tetromino)
    {
        if (tetromino == null || isGameOver) return;

        Debug.Log("[Tetris] Locking Tetromino.");
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

        // Instead of clearing rows immediately, wait to settle
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
        Debug.Log("[Tetris] Game Over!"); // Make sure that the game actually ended as it prepares to load a new scene and save the score.
        PlayerPrefs.SetInt("FinalScore", Score);
        PlayerPrefs.Save();
        SceneManager.LoadScene("GameOver");
    }
}
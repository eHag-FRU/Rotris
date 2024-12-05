using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Board : MonoBehaviour
{
    //Boards code representation
    public static int[,] board = {
        {0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0}
    };

    public static int rows = 20;
    public static int cols = 10;

    public static int score = 0;
    public static int level = 0;
    public static int lines = 0;

    private int scoreReal = 0;
    private int levelReal = 0;
    private int linesReal = 0;

    [SerializeField]
    private TextMeshProUGUI scoreText;

    [SerializeField]
    private TextMeshProUGUI linesText;

    [SerializeField]
    private TextMeshProUGUI levelText;


    // Start is called before the first frame update
    void Start()
    {
        //printBoardVectorState();    
    }

    // Update is called once per frame
    void Update()
    {
        //Update the score
        scoreReal = score;

        //Update the level
        levelReal = level;

        //Update the lines
        linesReal = lines;

        //Update the score text
        updateScoreText();

        //Update the level text
        updateLevelText();

        //Update the lines text
        updateLinesText();

        //Grab the piece
        GameObject[] testBackBoard = GameObject.FindGameObjectsWithTag("Backboard_Piece");

        //Loop through and find what is there
        for (int i = 0; i < testBackBoard.Count(); ++i) {
            //print($"[{i}]: " + testBackBoard[i].name + $" {{Col: {BackBoardPiece.getVectorColumnIndex(testBackBoard[i].GetComponent<BackBoardPiece>())}, Row: {BackBoardPiece.getVectorRowIndex(testBackBoard[i].GetComponent<BackBoardPiece>())}}}");

            //print("Board Index: " + BackBoardPiece.getVectorIndex(testBackBoard[i]));

            board[BackBoardPiece.getVectorRowIndex(testBackBoard[i].GetComponent<BackBoardPiece>()), BackBoardPiece.getVectorColumnIndex(testBackBoard[i].GetComponent<BackBoardPiece>())] = BackBoardPiece.getPiecePresent(testBackBoard[i].GetComponent<BackBoardPiece>());
 
           //printBoardVectorState();

           //Now need to add that as a 
        }



        

    }


    public static void checkForGameOver() {
        //Check the top row for a single block
        //Top row is 0
        print("Game over? (Row 3 have any in the row?)" + checkGameOverRow());


        if (checkGameOverRow()) {
            print("GAME OVER!");
            SceneManager.LoadScene("GameOver");
        }

    }

    public static int getScore() {
        return score;
    }

    public static void checkBoardForLineClears() {
        List<int> rowsToClear = new List<int>();

        //Will hold the current level to ensure that at level 0 
        //some score gets added
        int localLevelHolder = level;

        if (localLevelHolder == 0) {
            localLevelHolder += 1;
        }

        //Check the actual board vector for line clears
        for (int i = 0; i < rows; ++i) {
            if (checkFullRow(i)) {
                //Row is full
                print("ROW " + i + " IS FULL!!!");

                //Add to the rows to clear out
                rowsToClear.Add(i);
            }
        }

        //Figrue out the score
        switch (rowsToClear.Count()) {
            case 4:
            
                print("Score to add: " + (800*level));
                updateScore((800 * level));
                break;
            case 3:
                updateScore((500 * level));
                break;
            case 2:
                updateScore((300 * level));
                break;
            default:
                //1 line
                updateScore((100 * level));
                break;
        }

        //Now clear those lines
        for (int lineRow = 0; lineRow < rowsToClear.Count; lineRow++) {
            //print("In line " + lineRow);
            for (int lineCol = 0; lineCol < cols; lineCol++) {
                print("In line col " + lineCol);
                //Now in the individual back board piece

                //Grab the backboard piece
                BackBoardPiece currentBackBoardPiece = GetBackBoardPiece(rowsToClear[lineRow], lineCol);

                //Now grab the piece part with it
                GameObject currentPiece = currentBackBoardPiece.GetPiecePartOnBackBoard();

                //currentPiece.SetActive(false);

                // print("Piece part name: " + currentPiece.name);

                //Now remove the part
                Destroy(currentPiece);
                //print("Destroyed the current piece");

                //Set the piece present switch to 0 (no piece is now present)
                currentBackBoardPiece.piecePresent = 0;
            }
        }

        //Update the line cleared counter
        //lines += rowsToClear.Count; 

        //Grab the lowest number line cleared (This is the highest up line!!)
        //Bottom of the board is 19 and the top is 0
        //int highestLineCleared = 20;

        //for (int i = 0; i < rowsToClear.Count; i++) {
            // if (rowsToClear[i] < highestLineCleared) {
                // highestLineCleared = rowsToClear[i];
            // }
        // }

        //print("Highest line cleared: " + highestLineCleared);
        //Now need to move all the pieces down a line
        //Board.moveLinesDownAfterClear(highestLineCleared, rowsToClear.Count);

    }

    public static void moveLinesDownAfterClear(int highestRowToClear, int numberOfRowsCleared) {
        print("moveLinesDownAfterClear: Moving the lines down after clear!!!");
        print("moveLinesDownAfterClear: highestRowToClear - 1: " + (highestRowToClear - 1));
        //Now need to grab each and move each piece part down by the number of rows cleared * 2 Unity Units per step
        for (int currentRow = highestRowToClear - 1; currentRow >= 0; currentRow--) {
            //print("moveLinesDownAfterClear: CurrentRow: " + currentRow);
            //Grab each piece part
            for (int currentCol = 0; currentCol < cols; currentCol++) {
                //print("moveLinesDownAfterClear: currentCol: " + currentCol);

                //Grab the backboard piece
                BackBoardPiece currentBackBoardPiece = GetBackBoardPiece(currentRow, currentCol);

                //print("moveLinesDownAfterClear: Piece present at [" + currentRow + "," + currentCol + "]?: " + currentBackBoardPiece.piecePresent);

                if (currentBackBoardPiece.piecePresent == 1) {
                    //print("moveLinesDownAfterClear: There is a piece present at [" + currentRow + "," + currentCol + "]");
                    //Now grab the piece part with it
                    print("moveLinesDownAfterClear: Grabbing the current back board piece, line 213!");
                    GameObject currentPiece = currentBackBoardPiece.GetPiecePartOnBackBoard();

                    //Set the old location to have no piece present
                    currentBackBoardPiece.piecePresent = 0;

                    //Now we can move the piece down

                    //The new position will be the current position - (2 Unity Units per step * number of Rows cleared)
                    float newYPosition = currentPiece.transform.position.y - (2 * numberOfRowsCleared);
                    print("moveLinesDownAfterClear: New y position: " + newYPosition);
                    currentPiece.transform.position = new UnityEngine.Vector3(currentPiece.transform.position.x, newYPosition, currentPiece.transform.position.z );
                    
                    //Update the piece part location
                    //currentPiece.GetComponent<PiecePart_Location>().updatePartLocation((currentRow - numberOfRowsCleared), currentCol);

                    //Set the backboard piece that the piece part is at to now have a piece present
                    //currentBackBoardPiece = GetBackBoardPiece((currentRow - numberOfRowsCleared), currentCol);
                    //currentBackBoardPiece.piecePresent = 1;
                }
            }
        }

        //Now set the last row that was moved from (the highest one up the board) to have no pieces
        int rowtoSetToNoPiecesPresent = highestRowToClear - numberOfRowsCleared;

        print("moveLinesDownAfterClear: Row to set to no pieces present: " + rowtoSetToNoPiecesPresent);

        for (int currentCol = 0; currentCol < cols; currentCol++) {
            BackBoardPiece currentBackBoardPiece = GetBackBoardPiece(rowtoSetToNoPiecesPresent, currentCol);

            currentBackBoardPiece.piecePresent = 0;
        }
    }


    public static String printBoardVectorState() {
        StringBuilder boardString = new StringBuilder();

        //print("====Tetris Board====");

        for (int i = 0; i < rows; ++i) {
            for (int j = 0; j < cols; ++j){
                boardString.Append(Board.board[i,j]);
            }
            boardString.Append("\n");
        }

        //print(boardString);
        return boardString.ToString();
    }

    public static int getPiecePresent(int row, int col) {
        //Grabs the piece present value based on the row and column
        return (GameObject.Find(("Tile_"+row+"-"+col))).GetComponent<BackBoardPiece>().piecePresent;
    }

    public static bool checkFullRow(int row) {
        //Go through all columns of that row to check
        for (int i = 0; i < cols; ++i) {
            if (getPiecePresent(row, i) == 0) {
                //No piece present, NOT FULL!
                return false;
            }
        }

        //Passed the checks, so row is full
        return true;
    }

    public static bool checkGameOverRow() {
        //Go through all columns of that row to check
        for (int i = 0; i < cols; ++i) {
            if (getPiecePresent(2, i) == 1) {
                //Piece present, game over
                return true;
            }
        }

        //Passed the checks, so row is full
        return false;
    }


    public static void updateScore(int pointsToAdd) {
        //Update the score
        score += pointsToAdd;
    }

    private void updateScoreText() {
        scoreText.text = scoreReal.ToString();
    }

    private void updateLevelText() {
        levelText.text = levelReal.ToString();
    }

    private void updateLinesText() {
        linesText.text = linesReal.ToString();
    }

    private static BackBoardPiece GetBackBoardPiece(int row, int col) {
       return GameObject.Find(("Tile_"+row+"-"+col)).GetComponent<BackBoardPiece>();
    }


    public static void SetNextPiece(String pieceName) {

    }
}

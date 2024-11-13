using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

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

    private int scoreReal = 0;


    [SerializeField]
    private TextMeshProUGUI scoreText;


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

        //Update the score text
        updateScoreText();

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



        //Check the actual board vector for line clears
        for (int i = 0; i < rows; ++i) {
            if (checkFullRow(i)) {
                //Row is full
                //print("ROW " + i + " IS FULL!!!");
            }
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


    public static void updateScore(int pointsToAdd) {
        //Update the score
        score += pointsToAdd;
    }

    private void updateScoreText() {
        scoreText.text = scoreReal.ToString();
    }
}

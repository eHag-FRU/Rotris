using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
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

    // Start is called before the first frame update
    void Start()
    {
        printBoardVectorState();    
    }

    // Update is called once per frame
    void Update()
    {
        //Grab the piece
        GameObject[] testBackBoard = GameObject.FindGameObjectsWithTag("TestPiece");

        //Loop through and find what is there
        for (int i = 0; i < testBackBoard.Count(); ++i) {
            print($"[{i}]: " + testBackBoard[i].name + $" {{Col: {BackBoardPiece.getVectorColumnIndex(testBackBoard[i].GetComponent<BackBoardPiece>())}, Row: {BackBoardPiece.getVectorRowIndex(testBackBoard[i].GetComponent<BackBoardPiece>())}}}");


            //print("Board Index: " + BackBoardPiece.getVectorIndex(testBackBoard[i]));

            board[BackBoardPiece.getVectorRowIndex(testBackBoard[i].GetComponent<BackBoardPiece>()), BackBoardPiece.getVectorColumnIndex(testBackBoard[i].GetComponent<BackBoardPiece>())] = BackBoardPiece.getPiecePresent(testBackBoard[i].GetComponent<BackBoardPiece>());
 
            printBoardVectorState();
        }

    }

    public static String printBoardVectorState() {
        StringBuilder boardString = new StringBuilder();

        print("====Tetris Board====");

        for (int i = 0; i < rows; ++i) {
            for (int j = 0; j < cols; ++j){
                boardString.Append(Board.board[i,j]);
            }
            boardString.Append("\n");
        }

        print(boardString);
        return boardString.ToString();
    }
}

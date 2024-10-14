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
    public int[,] board = {
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

    private int rows = 20;
    private int cols = 10;

    // Start is called before the first frame update
    void Start()
    {
        printBoardVectorState();    
    }

    // Update is called once per frame
    void Update()
    {
    }

    void printBoardVectorState() {
        StringBuilder boardString = new StringBuilder();

        print("====Tetris Board====");

        for (int i = 0; i < rows; ++i) {
            for (int j = 0; j < cols; ++j){
                boardString.Append(board[i,j]);
            }
            boardString.Append("\n");
        }

        print(boardString);
        return;
    }
}

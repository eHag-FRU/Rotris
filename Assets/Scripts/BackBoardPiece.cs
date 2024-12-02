using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BackBoardPiece : MonoBehaviour
{
    // Start is called before the first frame update


    [Range(0,19)]
    public int vectorRowIndex;

    [Range(0,9)]
    public int vectorColumnIndex;



    [Range(0,1)]
    public int piecePresent;

    //private int vectorIndexReal;

    //Will hold the collider for the part piece that is currently there
    [SerializeField]
    private Collider2D partPiece;

    
    void Start()
    {
        //Automatically set that there is not a piece present on top of it
        piecePresent = 0;

        //Set the part piece to null as no current part piece is there
        partPiece = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject GetPiecePartOnBackBoard() {
        return partPiece.gameObject;
    }

    public void OnTriggerEnter2D(Collider2D other) {
        //print($"ENTERING {this.name.ToUpper()}'s TRIGGER!!!!");
        //Check if the other is a piece tag
        if (other.tag == "Piece_Part") {
            //the piece is there, now signal there is a piece there
            //print($"{other.name.ToUpper()}'S TAG IS Piece");
            piecePresent = 1;
            partPiece = other;
            return;
        }
    }

    public void OnTriggerExit2D(Collider2D other) {
        //print($"LEAVING {this.name.ToUpper()}'S TRIGGER!!!!");

        //Check if the other is a piece tag
        //if (other.tag == "Piece") {
            //the piece is there, now signal there is a piece there
            piecePresent = 0;
            partPiece = null;
            return;
        //}
    }

    public static int getVectorRowIndex(BackBoardPiece piece) {

        if (piece.tag == "TestPiece" || piece.tag == "Backboard_Piece") {
            return piece.vectorRowIndex;

        }

        return -1;
    }

    public static int getVectorColumnIndex(BackBoardPiece piece) {


        if (piece.tag == "TestPiece" || piece.tag == "Backboard_Piece") {
           return piece.vectorColumnIndex;

        }

        return -1;
    }

    public static int getPiecePresent(BackBoardPiece piece) {

        return piece.piecePresent;
    }
}

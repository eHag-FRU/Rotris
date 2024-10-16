using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Piece_Backup : MonoBehaviour
{
    public GameObject prefab;


    //Timing delays for the movement downward
    //1F = 1 Second
    //0.5F = 0.5 Seconds
    public float stepDelay = 1f;
    
    public bool takeStep;

    private float stepTime;

    public UnityEngine.Vector3 position;

    public Rigidbody2D currentPiece;

    //Controls for the player
    public PlayerInput playerInput; 

    //Input actions
    private InputAction pieceMovement;
    private InputAction pieceRotation;
    private InputAction boardRotation;
    private InputAction softDrop;
    private InputAction hardDrop;
    private InputAction hold;

    // Start is called before the first frame update
    void Start()
    {
        //Grab the prefab
        //prefab = GetComponentInChildren<GameObject>();

        //Platform independent
        this.stepTime = Time.deltaTime + this.stepDelay;

        //Grab player input system
        playerInput = GetComponent<PlayerInput>();

    

        //Grab actions from the playerInput mapping
        pieceMovement = playerInput.actions["PieceMovement"];
        pieceMovement.Enable();

        //print(horizMovement.ToString());

        pieceRotation = playerInput.actions["PieceRotation"];
        pieceRotation.Enable();
        

        boardRotation = playerInput.actions["BoardRotation"];
        softDrop = playerInput.actions["SoftDrop"];
        hardDrop = playerInput.actions["HardDrop"];
        hold = playerInput.actions["Hold"];


        //Grab the current piece to control
        currentPiece = GetComponent<Rigidbody2D>();


        print(prefab.ToString());

    }
    // Update is called once per frame
    void Update(){
        



        //Update the current piece position
        //currentPiece.position = new UnityEngine.Vector2(currentPiece.position.x + movement, currentPiece.position.y);

        if (pieceMovement.triggered) {
            currentPiece.position= new UnityEngine.Vector2(currentPiece.position.x + pieceMovement.ReadValue<float>(),
                currentPiece.position.y);
        }


        //Rotation check
        if (pieceRotation.triggered) {
            //Rotate in increments of 90 deg
            currentPiece.rotation += 90;
        }

        //Soft Drop

        //If the current time is greater than or
        //equal to the stepTime (starts out as 1f [1 second])
        //Then step the piece
        if (Time.deltaTime >= this.stepTime) {
            Step();
            
        }
    }

    void Step(){
        //Push the time forward again to be 1 second into the future
        //Ensures that the steps keep going
        this.stepTime = Time.deltaTime + stepDelay;

        //Move the piece down
        Move(UnityEngine.Vector2Int.down);
    }


    //Function that will move the piece down but a set unit
    void Move(UnityEngine.Vector2 translation) {
        UnityEngine.Vector3 newPOS = this.position;
        newPOS.x += translation.x;
        newPOS.y += translation.y;

        this.position = newPOS;

    }
}
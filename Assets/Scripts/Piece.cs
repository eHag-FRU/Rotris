using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Piece_Backup : MonoBehaviour {


    //Timing delays for the movement downward
    //1F = 1 Second
    //0.5F = 0.5 Seconds
    public float stepDelay = 1f;

    private float stepTime;

    public UnityEngine.Vector3 position;

    public Rigidbody2D currentPiece;

    //Controls for the player
    public PlayerInput playerInput; 

    //Input actions
    public InputAction pieceMovement;
    private InputAction pieceRotation;
    private InputAction boardRotation;
    private InputAction softDrop;
    private InputAction hardDrop;
    private InputAction hold;


    //Used for DEBUGGING
    [SerializeField]
    private bool stepEnabled = false;

    // Start is called before the first frame update
    void Start()
    {
        //Grab the prefab
        //prefab = GetComponentInChildren<GameObject>();

        //Platform independent
        this.stepTime = Time.time + this.stepDelay;

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
        if (softDrop.triggered) {
            //Move the piece down by one
            this.transform.position = new UnityEngine.Vector3(this.transform.position.x, this.transform.position.y - 1, this.transform.position.z );
           
        }

        // //If the current time is greater than or
        // //equal to the stepTime (starts out as 1f [1 second])
        // //Then step the piece
        // if (Time.time >= this.stepTime) {
        //     print ($"Piece Move Triggered @ {Time.time - this.stepTime}");
        //     Step();
            
        // }

        if (stepEnabled) {
            Invoke("Step", 1F);
        }
        
    } 

    void Step(){
        print("Making a step");
        this.transform.position = new UnityEngine.Vector3(this.transform.position.x, this.transform.position.y - 1, this.transform.position.z );

        CancelInvoke("Step");   
    }


    
}

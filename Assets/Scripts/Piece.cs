using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;

public class Piece : MonoBehaviour {


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

    //
    //  Holds the piece prefabs
    //

    [SerializeField]
    private GameObject T;

    [SerializeField]
    private GameObject J;

    [SerializeField]
    private GameObject L;

    [SerializeField]
    private GameObject Z;

    [SerializeField]
    private GameObject I;

    [SerializeField]
    private GameObject S;

    [SerializeField]
    private GameObject O; 


    //Holds the spawn point empty object as a location holder
    [SerializeField]
    private GameObject PieceSpawnPoint;



    //REMOVE for release, used to test the piece picker functionality
    private InputAction DEBUG_PIECE_RELEASE;

    //REMOVE for release, used to test the lock timer functionality
    private InputAction DEBUG_LOCK_TIMER;

    //Smaller blocks that make up the piece locations
    //and gameobjects

    [SerializeField]
    private List<GameObject> PieceParts;


    private List<Vector2Int> PiecePartLocations;

    //Used for DEBUGGING
    [SerializeField]
    private bool stepEnabled = false;


    //Piece enum to hold the names
    enum pieceNames {
        I,
        T,
        O,
        S,
        Z,
        J,
        L
    }

    //Holds the next piece
    private pieceNames nextPiece;

     //helps ensure that a new piece is not picked multiple times
     bool piecePicked = false;



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
        DEBUG_PIECE_RELEASE = playerInput.actions["DEBUG_RELEAESE_PIECE"];
        DEBUG_LOCK_TIMER = playerInput.actions["DEBUG_LOCK_TIMER"];


        //Grab the current piece to control, the piece prefab is a child to the piece object
        //This allows for movement of each piece and release for the next piece
        currentPiece = GetComponentInChildren<Rigidbody2D>();


        //Setup the current piece part list
        //0 will ALWAYS be the current piece!!!!
        PieceParts = getChildren(this.transform.GetChild(0).gameObject);

        // List<GameObject> pieceParts = getChildren(this.transform.GetChild(0).gameObject);

        PiecePartLocations  = new List<Vector2Int>();

        //Grab the part locations 
        for (int part = 0; part < PieceParts.Count; ++part) {
            PiecePartLocations.Add(PieceParts[part].GetComponent<PiecePart_Location>().getPartLocation());
        }

        //Sorts the part locations so the largest is on the bottom and the last location checked
        //PiecePartLocations.Sort();

    }
    // Update is called once per frame
    void Update(){
        //Update the current piece position
        //currentPiece.position = new UnityEngine.Vector2(currentPiece.position.x + movement, currentPiece.position.y);

        if (pieceMovement.triggered) {
            if (!PiecePart_HorizontalMovement.validHorizontalMoveChecker(pieceMovement.ReadValue<float>(), PieceParts, PiecePartLocations)) {
                //Can not keep moving, need to stop moving and handle case
                
               
            } else {

                //Valid, so move the piece down
                currentPiece.position= new UnityEngine.Vector2(currentPiece.position.x + pieceMovement.ReadValue<float>() * 2,
                currentPiece.position.y);

                PiecePart_HorizontalMovement.piecePartHorizontalLocationUpdater(pieceMovement.ReadValue<float>(), PieceParts, PiecePartLocations);
            }
        }


        //Rotation check
        if (pieceRotation.triggered) {
            //Rotate in increments of 90 deg
            
            //Now check if the rotations would be valid
            if(PiecePart_RotationMovement.piecePartRotationValidator(currentPiece, PieceParts, PiecePartLocations)) {
                currentPiece.rotation += 90;

                //Update after the rotation
                PiecePart_RotationMovement.piecePartRotationLocationUpdater(PieceParts, PiecePartLocations);
            }
            
        }

        //Soft Drop
        if (softDrop.triggered) {
            //Move the piece down by one

            if (!PiecePart_VerticalMovement.validVerticalMoveChecker(PieceParts, PiecePartLocations)) {
                print("Hit bottom of board or piece");

                //Disable the step
                stepEnabled = false;
    
            } else {
                //True, can move down
                this.transform.position = new UnityEngine.Vector3(this.transform.position.x, this.transform.position.y - 2, this.transform.position.z );
            
                PiecePart_VerticalMovement.piecePartVetricalLocationUpdater(PieceParts, PiecePartLocations);

                //Update the score by one
                Board.updateScore(1);
            }
        }

        //DEBUG: Force release of a piece to test spawn points
        if (DEBUG_PIECE_RELEASE.triggered) {
            print("DEBUG: Piece Release triggered!!");

            //Spawn a new piece onto the board
            pickNewPiece();
        }

        //DEBUG: Force the lock timer
        if (DEBUG_LOCK_TIMER.triggered) {
            print("DEBUG: LOCK TIMER triggered!!");

            //Lock timer every 0.5 seconds
            Invoke("lockTimer", 0.5f);
        }

        if (stepEnabled) {
            Invoke("Step", 1F);
        }
    } 


    //Will hit at 0.5 seconds
    void lockTimer() {
        //Detach the piece from the player controller
        this.transform.DetachChildren();

        //Set the current piece to none to remove rotations
        this.currentPiece = null;

        //Cancel out the invoke
        CancelInvoke("locktimer");
    }

    void calculateHardDrop() {
        //Take each piece location and check the least furthest that can go down
        //The least will be what can actually go
        int shortestDropDistance = 0;

        print("calculating hard drop distance");

        //Find the lowest point
        Vector<Vector<int>> lowestPoints;

        //Cycle through them to find the lowest points


    }


    void pickNewPiece() {
        //Grab the enum of piece names
        Array values = Enum.GetValues(typeof(pieceNames));

        //Makes a random generator
        System.Random randomPiecePicker = new System.Random();

        // print((pieceNames)values.GetValue());

        //Grabs the random piece name
        //print("Next Random Value: " + randomPiecePicker.Next(values.Length));
        pieceNames randomPiece = (pieceNames)values.GetValue(randomPiecePicker.Next(values.Length));

        //DEBUG: REMOVE BEFORE RELEASE!!!!
        randomPiece = pieceNames.L;

        //print(randomPiece.DisplayName());
        //Now get the piece prefab based oon its name
        switch (randomPiece) {
            case pieceNames.T:
                this.currentPiece = Instantiate(T, PieceSpawnPoint.transform.position, UnityEngine.Quaternion.identity).GetComponent<Rigidbody2D>();
                //print("Instantiate the game object");
            
                //print("Setting the piece's parent as the piece object");
                currentPiece.transform.parent = this.gameObject.transform;
                break;
            case pieceNames.L:
                //print("In the L case");
                this.currentPiece = Instantiate(L, PieceSpawnPoint.transform.position, UnityEngine.Quaternion.identity).GetComponent<Rigidbody2D>();
                //print("Instantiate the game object");
            
                //print("Setting the piece's parent as the piece object");
                currentPiece.transform.parent = this.gameObject.transform;

                
                break;
        }

        //Signal to the board that the row check is to happen, and full rows are to be cleared
        //Board.checkBoardForLineClears();

        //Now enable the step for it to move down the board
        this.stepEnabled =  true;

        //Reset the part piece locations
        this.PiecePartLocations.Clear();

        //Now set the piece part list
        this.PieceParts = getChildren(this.transform.GetChild(0).gameObject);

        //print("Name of next piece picked: " + randomPiece.DisplayName());
    }

       

    void Step(){
        //print("Making a step");

        //Check if valid for vertical
        if (!PiecePart_VerticalMovement.validVerticalMoveChecker(PieceParts, PiecePartLocations)) {
            return;
        }


        //Move Piece
        this.transform.position = new UnityEngine.Vector3(this.transform.position.x, this.transform.position.y - 2, this.transform.position.z );


        PiecePart_VerticalMovement.piecePartVetricalLocationUpdater(PieceParts, PiecePartLocations);
        
        
        CancelInvoke("Step");   
    }


    //Will grab all the children piece parts for the positions to be grabbed of the current active piece
    List<GameObject> getChildren(GameObject parent) {
        List<GameObject> result = new List<GameObject>();

        for (int currentChild = 0; currentChild < parent.transform.childCount; ++currentChild) {
            result.Add(parent.transform.GetChild(currentChild).gameObject);
        }

        return result;
    }
}

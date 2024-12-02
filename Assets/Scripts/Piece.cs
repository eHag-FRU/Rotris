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
            if (validHorizontalMoveChecker(pieceMovement.ReadValue<float>())) {
                currentPiece.position= new UnityEngine.Vector2(currentPiece.position.x + pieceMovement.ReadValue<float>() * 2,
                currentPiece.position.y);

                piecePartHorizontalLocationUpdater(pieceMovement.ReadValue<float>());
            }
            
        }


        //Rotation check
        if (pieceRotation.triggered) {
            //Rotate in increments of 90 deg
            
            currentPiece.rotation += 90;

            //Update after the rotation
            piecePartRotationLocationUpdater();
        }

        //Soft Drop
        if (softDrop.triggered) {
            //Move the piece down by one

            if (validVerticalMoveChecker()) {
                this.transform.position = new UnityEngine.Vector3(this.transform.position.x, this.transform.position.y - 2, this.transform.position.z );
            
                piecePartVetricalLocationUpdater();

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

        //Check for step 
        
       
    


        if (stepEnabled) {
            Invoke("Step", 1F);
        }
    } 


    //Will hit at 0.5 seconds
    void lockTimer() {
        pickNewPiece();

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
        //Detach the piece from the player controller
        this.transform.DetachChildren();

        //Set the current piece to none to remove rotations
        this.currentPiece = null;

        //Grab the enum of piece names
        Array values = Enum.GetValues(typeof(pieceNames));

        //Makes a random generator
        System.Random randomPiecePicker = new System.Random();

        // print((pieceNames)values.GetValue());

        //Grabs the random piece name
        print("Next Random Value: " + randomPiecePicker.Next(values.Length));
        pieceNames randomPiece = (pieceNames)values.GetValue(randomPiecePicker.Next(values.Length));

        //DEBUG: REMOVE BEFORE RELEASE!!!!
        randomPiece = pieceNames.L;

        //print(randomPiece.DisplayName());
        //Now get the piece prefab based oon its name
        switch (randomPiece) {
            case pieceNames.T:
                this.currentPiece = Instantiate(T, PieceSpawnPoint.transform.position, UnityEngine.Quaternion.identity).GetComponent<Rigidbody2D>();
                print("Instantiate the game object");
            
                print("Setting the piece's parent as the piece object");
                currentPiece.transform.parent = this.gameObject.transform;
                break;
            case pieceNames.L:
                print("In the L case");
                this.currentPiece = Instantiate(L, PieceSpawnPoint.transform.position, UnityEngine.Quaternion.identity).GetComponent<Rigidbody2D>();
                print("Instantiate the game object");
            
                print("Setting the piece's parent as the piece object");
                currentPiece.transform.parent = this.gameObject.transform;

                
                break;
        }

        //Signal to the board that the row check is to happen, and full rows are to be cleared
        Board.checkBoardForLineClears();

        //Now enable the step for it to move down the board
        this.stepEnabled =  true;

        //Reset the part piece locations
        this.PiecePartLocations.Clear();

        //Now set the piece part list
        this.PieceParts = getChildren(this.transform.GetChild(0).gameObject);

        //print("Name of next piece picked: " + randomPiece.DisplayName());
    }

    bool validHorizontalMoveChecker(float movementDirectionModifier) {
        print("PiecePartLocations: " + PiecePartLocations.ToString());

        //Check for each piece location
        foreach (GameObject part in PieceParts) {
            print(part.name + " BEFORE: " + part.GetComponent<PiecePart_Location>().getPartLocation().y);
            
            //
            //X = Row
            //Y = Column
            //

            int currentPartRow = part.GetComponent<PiecePart_Location>().getPartLocation().x;
            int currentPartColl= part.GetComponent<PiecePart_Location>().getPartLocation().y;


            //Now we have a single piece part's row and collumn
            //Now compare against every other piece part's location
            //Based on row + 1 (x part of the vectors) => The next position after the step
            //This will tell us if the row right below (at same coll)
            //Has a piece part in it OR if another block is occupying it
            //Piece part in it => then its valid to move, as that
            //piece part is going to move down

            //NOT piece part
            //Then invalid and start the lock timer
            //This means its it something else!

            foreach(Vector2Int partletLocation in PiecePartLocations) {
                //If in the col + 1 is equal to one of the other pieceparts locations, break
                //and continue cycling through
                if (currentPartColl + movementDirectionModifier == partletLocation.y) {
                    //We already found its new position, no need to continue, break!
                    print("Already found new position that another partlet has");
                    break;
                } else if (partletLocation == PiecePartLocations[3]) {
                    //At the last location to check!!!
                    
                    print(part.name + " is at last location to check " + partletLocation + " !!!!");
                } else if (currentPartColl + movementDirectionModifier > 9 || currentPartColl + movementDirectionModifier < 0) {

                    print("At Right OR Left of board!!!");

                    return false;
                }

                //print (part.name + " still checking for the coll occupying " + currentPartRow + ", " + currentPartColl);
            }

            
            

           print("Done checking partlet location matches, NOT IT");
        }


        return true;
    }

    void piecePartRotationLocationUpdater() {
        //Clear the locations to allow for new
        PiecePartLocations.Clear();
        


        //Now update the pice part locations
        foreach (GameObject part in PieceParts) {
            //print("Horizontal Location Updater: " + movementDirectionModifier);

            //Calculate the new x and y
            //X = Row
            //Y = Column
            int x = part.GetComponent<PiecePart_Location>().getPartLocation().x;
            int y = part.GetComponent<PiecePart_Location>().getPartLocation().y;

            //Formula for an 90 deg rotation counter-clock wise
            //(x,y) =(Rotation)=> (y-1, x + 1)
            //Rotation point is from the bottom of the piece (lowest point) as center!!!

            int newX = x + 1;
            int newY = y - 1;

            //Update them
            part.GetComponent<PiecePart_Location>().updatePartLocation(newY, newX);

            //Update the location in the parts location
            PiecePartLocations.Add(part.GetComponent<PiecePart_Location>().getPartLocation());

            print(part.name + " AFTER: " + part.GetComponent<PiecePart_Location>().getPartLocation().x);
        }

    }


    void piecePartHorizontalLocationUpdater(float movementDirectionModifier) {
        //Clear the locations to allow for new
        PiecePartLocations.Clear();
        


        //Now update the pice part locations
        foreach (GameObject part in PieceParts) {
            print("Horizontal Location Updater: " + movementDirectionModifier);

            //Calculate the new x and y
            //X = Row
            //Y = Column
            int x = part.GetComponent<PiecePart_Location>().getPartLocation().x;
            int y = part.GetComponent<PiecePart_Location>().getPartLocation().y + (int)movementDirectionModifier;

            //Update them
            part.GetComponent<PiecePart_Location>().updatePartLocation(x,y);

            //Update the location in the parts location
            PiecePartLocations.Add(part.GetComponent<PiecePart_Location>().getPartLocation());

            print(part.name + " AFTER: " + part.GetComponent<PiecePart_Location>().getPartLocation().x);
        }

    }


    bool validVerticalMoveChecker() {
        print("PiecePartLocations: " + PiecePartLocations.ToString());

        //Check for each piece location
        foreach (GameObject part in PieceParts) {
            print(part.name + " BEFORE: " + part.GetComponent<PiecePart_Location>().getPartLocation().x);
            
            //
            //X = Row
            //Y = Column
            //

            int currentPartRow = part.GetComponent<PiecePart_Location>().getPartLocation().x;
            int currentPartColl= part.GetComponent<PiecePart_Location>().getPartLocation().y;


            //Now we have a single piece part's row and collumn
            //Now compare against every other piece part's location
            //Based on row + 1 (x part of the vectors) => The next position after the step
            //This will tell us if the row right below (at same coll)
            //Has a piece part in it OR if another block is occupying it
            //Piece part in it => then its valid to move, as that
            //piece part is going to move down

            //NOT piece part
            //Then invalid and start the lock timer
            //This means its it something else!

            foreach(Vector2Int partletLocation in PiecePartLocations) {
                //If in the row + 1 is equal to one of the other pieceparts locations, break
                //and continue cycling through
                if (currentPartRow + 1 == partletLocation.x) {
                    //We already found its new position, no need to continue, break!
                    print("Already found new position that another partlet has");
                    break;
                } else if (partletLocation == PiecePartLocations[3]) {
                    //At the last location to check!!!
                    print(part.name + " is at last location to check " + partletLocation + " !!!!");
                } else if (currentPartRow + 1 > 19 || currentPartRow == 19) {
                    //Piece is at bottom
                    stepEnabled = false;

                    print("At bottom of board!!!");

                    //Drop piece
                    part.transform.DetachChildren();

                    return false;
                }

                //print (part.name + " still checking for the coll occupying " + currentPartRow + ", " + currentPartColl);
            }

            
            

           print("Done checking partlet location matches, NOT IT");
        }


        return true;
    }

    void piecePartVetricalLocationUpdater() {
        //Clear the locations to allow for new
        PiecePartLocations.Clear();
        


        //Now update the pice part locations
        foreach (GameObject part in PieceParts) {
            
            //Calculate the new x and y
            //X = Row
            //Y = Column
            int x = part.GetComponent<PiecePart_Location>().getPartLocation().x + 1;
            int y = part.GetComponent<PiecePart_Location>().getPartLocation().y;

            //Update them
            part.GetComponent<PiecePart_Location>().updatePartLocation(x,y);

            //Update the location in the parts location
            PiecePartLocations.Add(part.GetComponent<PiecePart_Location>().getPartLocation());

            //print(part.name + " AFTER: " + part.GetComponent<PiecePart_Location>().getPartLocation().x);
        }

    }

    void Step(){
        print("Making a step");

        //Check if valid for vertical
        if (!validVerticalMoveChecker()) {
            return;
        }


        //Move Piece
        this.transform.position = new UnityEngine.Vector3(this.transform.position.x, this.transform.position.y - 2, this.transform.position.z );


        piecePartVetricalLocationUpdater();
        
        
        CancelInvoke("Step");   
    }

    // void OnCollisionEnter2D(Collider2D other) {
    //     print("ENTERED A 2D Collider!!!");
    // }

    //Will grab all the children piece parts for the positions to be grabbed of the current active piece
    List<GameObject> getChildren(GameObject parent) {
        List<GameObject> result = new List<GameObject>();

        for (int currentChild = 0; currentChild < parent.transform.childCount; ++currentChild) {
            result.Add(parent.transform.GetChild(currentChild).gameObject);
        }

        return result;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEditor.Animations;
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

    //Smaller blocks that make up the piece locations
    //and gameobjects

    [SerializeField]
    private List<GameObject> PieceParts;


    private List<Vector2Int> PiecePartLocations;

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
        PiecePartLocations.Sort();

    }
    // Update is called once per frame
    void Update(){
        



        //Update the current piece position
        //currentPiece.position = new UnityEngine.Vector2(currentPiece.position.x + movement, currentPiece.position.y);

        if (pieceMovement.triggered) {
            currentPiece.position= new UnityEngine.Vector2(currentPiece.position.x + pieceMovement.ReadValue<float>() * 2,
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
            this.transform.position = new UnityEngine.Vector3(this.transform.position.x, this.transform.position.y - 2, this.transform.position.z );
           
        }

        //Check for step 
        
       
    


        if (stepEnabled) {
            Invoke("Step", 1F);
        }
        


       

    } 

    void Step(){
        print("Making a step");

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
                }

                //print (part.name + " still checking for the coll occupying " + currentPartRow + ", " + currentPartColl);
            }

            
            

           print("Done checking partlet location matches, NOT IT");
        }


        //Move Piece
        this.transform.position = new UnityEngine.Vector3(this.transform.position.x, this.transform.position.y - 2, this.transform.position.z );


        
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

            print(part.name + " AFTER: " + part.GetComponent<PiecePart_Location>().getPartLocation().x);
        }

        
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

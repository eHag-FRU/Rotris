using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecePart_VerticalMovement : MonoBehaviour
{

    public static bool validVerticalMoveChecker(List<GameObject> PieceParts, List<Vector2Int> PiecePartLocations) {
        //print("PiecePartLocations: " + PiecePartLocations.ToString());

       

        //Check for each piece location
        foreach (GameObject part in PieceParts) {
            //print(part.name + " BEFORE: " + part.GetComponent<PiecePart_Location>().getPartLocation().x);
            
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
                    //print("Already found new position that another partlet has");
                    break;
                } else if (partletLocation == PiecePartLocations[3]) {
                    // print(part.name + " location: " + "(" + currentPartRow + "," + currentPartColl + ")");
                    // print(part.name + " one below location: " + "(" + (currentPartRow + 1) + "," + currentPartColl + ")");
                    

                    //At the last location to check!!!
                    //print(part.name + " is at last location to check " + partletLocation + " !!!!");

                    //Now check 1 part below
                    int pieceBelow = Board.getPiecePresent((currentPartRow + 1), currentPartColl);

                    // print("Another piece below at " + "(" + (currentPartRow + 1) + "," + currentPartColl + ")" + ": " + pieceBelow);

                    if (pieceBelow == 1) {
                        // print("Hit another piece!");
                        //Need to start lock timer and pick new piece!!
                        return false;
                    }
                    
                    // print("Not hitting another piece, keep moving!");

                } else if (currentPartRow + 1 > 19 || currentPartRow == 19) {
                    // //Piece is at bottom
                    // stepEnabled = false;

                    // print("At bottom of board!!!");

                    // //lock piece in
                    // Invoke("lockTimer", 0.5f);

                    return false;
                }

                //print (part.name + " still checking for the coll occupying " + currentPartRow + ", " + currentPartColl);
            }

            
            

           //print("Done checking partlet location matches, NOT IT");
        }


        return true;
    }



    public static void piecePartVetricalLocationUpdater(List<GameObject> PieceParts, List<Vector2Int> PiecePartLocations) {
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




}
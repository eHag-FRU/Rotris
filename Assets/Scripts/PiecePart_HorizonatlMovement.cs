using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecePart_HorizontalMovement : MonoBehaviour
{

    public static bool validHorizontalMoveChecker(float movementDirectionModifier, List<GameObject> PieceParts, List<Vector2Int> PiecePartLocations, Piece piece) {
            //print("PiecePartLocations: " + PiecePartLocations.ToString());

            //Will hold the validity

            //Ensures there is a piece there to move
            if (piece == null) {
                return false;
            }

            //Check for each piece location
            foreach (GameObject part in PieceParts) {
                //print(part.name + " BEFORE: " + part.GetComponent<PiecePart_Location>().getPartLocation().y);
                
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
                        //print("Already found new position that another partlet has");
                        break;
                    } else if (partletLocation == PiecePartLocations[(PiecePartLocations.Count - 1)]) {
                        int newCollPosition = currentPartColl + (int)movementDirectionModifier;

                        //At the last location to check!!!
                        // print(part.name + " location: " + "(" + currentPartRow + "," + currentPartColl + ")");
                        // print(part.name + " one left/right location: " + "(" + currentPartRow + "," + newCollPosition + ")");
                        // print(part.name + " direction modifier: " + (int)movementDirectionModifier);

                        

                        //At the last location to check!!!
                        //print(part.name + " is at last location to check " + partletLocation + " !!!!");

                        //Now check 1 part to the left or to the right
                        int pieceToLeftOrRight = Board.getPiecePresent(currentPartRow, newCollPosition);

                        //print("Another piece to the left or right at " + "(" + currentPartRow + "," + newCollPosition + ")" + ": " + pieceToLeftOrRight);

                        //-1 means there is not even a piece to the left or right, not even a background tile!!!
                        if (pieceToLeftOrRight == 1 || pieceToLeftOrRight == -1) {
                            //print("STOP! ANOTHER PIECE IS PRESENT!!!!!");

                            return false;
                        }
                        
                            
                        //print(part.name + " is at last location to check " + partletLocation + " !!!!");
                    } else if (currentPartColl + movementDirectionModifier > 9 || currentPartColl + movementDirectionModifier < 0) {

                        //print("At Right OR Left of board!!!");

                        return false;
                    }

                    //print (part.name + " still checking for the coll occupying " + currentPartRow + ", " + currentPartColl);
                }

                
                

            print("Done checking partlet location matches, NOT IT");
            }


            return true;
    }



    public static void piecePartHorizontalLocationUpdater(float movementDirectionModifier, List<GameObject> PieceParts, List<Vector2Int> PiecePartLocations) {
        //Clear the locations to allow for new
        PiecePartLocations.Clear();
        


        //Now update the pice part locations
        foreach (GameObject part in PieceParts) {
            //print("Horizontal Location Updater: " + movementDirectionModifier);

            //Calculate the new x and y
            //X = Row
            //Y = Column
            int x = part.GetComponent<PiecePart_Location>().getPartLocation().x;
            int y = part.GetComponent<PiecePart_Location>().getPartLocation().y + (int)movementDirectionModifier;

            //Update them
            part.GetComponent<PiecePart_Location>().updatePartLocation(x,y);

            //Update the location in the parts location
            PiecePartLocations.Add(part.GetComponent<PiecePart_Location>().getPartLocation());

            //print(part.name + " AFTER: " + part.GetComponent<PiecePart_Location>().getPartLocation().x);
        }

    }



}
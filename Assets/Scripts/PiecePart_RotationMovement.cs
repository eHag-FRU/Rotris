using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PiecePart_RotationMovement : MonoBehaviour
{

    public static void piecePartRotationLocationUpdater(List<GameObject> PieceParts, List<Vector2Int> PiecePartLocations) {
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

            //print(part.name + ", x: " + x + ", y: " + y);
            //Formula for an 90 deg rotation counter-clock wise
            //(x,y) =(Rotation)=> (y-1, x + 1)
            //Rotation point is from the bottom of the piece (lowest point) as center!!!

           

            //print(part.name + " AFTER: " + part.GetComponent<PiecePart_Location>().getPartLocation().x);
        }

    }


    public static bool piecePartRotationValidator(Rigidbody2D piece, List<GameObject> PieceParts, List<Vector2Int> PiecePartLocations) {
        //1.) Grab the angle the piece is currently at on the Z-Axis for rotation
        float currentRotationAngle = piece.rotation;

        print(piece.name + " current Z-AxisRotation: " + currentRotationAngle);

        switch ((currentRotationAngle % 360) + 90) {
            case 90:
                print("Rotating to 90");
                //We are going to swap (x) and (y), EXCEPT ON BOTTOM
                if(!PiecePart_RotationMovement.rotation90(piece, PieceParts, PiecePartLocations)) {
                    //Rotation is not possible!
                    return false;
                }


                break;
            case 180:
                print("Rotating to 180");
                break;
            case 270:
                print("Rotating to 270");
                break;
            default:
                //This is 360 deg rotation
                print("Rotating to 360/0");
                break;
        }

        //The rotation is valid!
        return true;
    }

    public static bool rotation90(Rigidbody2D piece, List<GameObject> PieceParts, List<Vector2Int> PiecePartLocations) {
        //grab the bottom part
        if(bottom)

        //Will always be 3 parts to rotate (4 parts - 1 bottom = 3 remaining parts)
        int numOfPartsToRotate = 3;

        //1.) Iterate through each of the piece parts
        foreach (GameObject part in PieceParts) {
            //Bottom is the rotation center, do no operate on
            //Pass it if possible
            if (part.name == "Bottom") {
                //DO NOT ROTATE VALIDATE

                //Grab the bottom piece col
                bottomCol = part.GetComponent<PiecePart_Location>().getPartLocation().y;


                continue;
            } else {
                //Only have top, middle top, middle bottom
                
                //grab the current x and y, swap them for the NEW cordinates
                int newX = part.GetComponent<PiecePart_Location>().getPartLocation().y;
                int newY = part.GetComponent<PiecePart_Location>().getPartLocation().x;

                print("oldX: " + part.GetComponent<PiecePart_Location>().getPartLocation().x + ", NEWX: " + newX);
                print("oldY: " + part.GetComponent<PiecePart_Location>().getPartLocation().y + ", NEWY: " + newY);
                print("");

                //Now check if either would be out of bounds
                //Formula to check if going to hit wall on 0 end
                // Bottom X - number to rotate < 0    =====> CAN NOT ROTATE, DONT HAVE ROOM
                if((bottomCol - numOfPartsToRotate) < 0) {
                    //Not valid, outside of the bounds for the cols
                   print("INVALID ROTATE, WILL GO OUTSIDE INDEX 0 COL!!!");
                    return false;
                } 
                
            }
        }

        //Rotation valid
        return true;
    }

}
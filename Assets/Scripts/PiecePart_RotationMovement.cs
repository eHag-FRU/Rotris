using System.Collections;
using System.Collections.Generic;
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

            print(part.name + ", x: " + x + ", y: " + y);
            //Formula for an 90 deg rotation counter-clock wise
            //(x,y) =(Rotation)=> (y-1, x + 1)
            //Rotation point is from the bottom of the piece (lowest point) as center!!!

           

            //print(part.name + " AFTER: " + part.GetComponent<PiecePart_Location>().getPartLocation().x);
        }

    }


    public static bool piecePartRotationValidator(List<GameObject> PieceParts, List<Vector2Int> PiecePartLocations) {


        //The rotation is valid!
        return true;
    }



}
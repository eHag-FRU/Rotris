using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Piece : MonoBehaviour
{

    //Timing delays for the movement downward
    //1F = 1 Second
    //0.5F = 0.5 Seconds
    public float stepDelay = 1f;
    
    public bool takeStep;

    private float stepTime;

    public Vector3 position;

    public InputAction input;

    // Start is called before the first frame update
    void Start()
    {
        //Sets the lock times up to sync with the systems time
        //Platform independent
        // this.stepTime = Time.time + this.stepDelay;

        input.Enable();
        print(input.ToString());
    }
    // Update is called once per frame
    void Update(){
        

        //Check for input
        //if (input.)


        //If the current time is greater than or
        //equal to the stepTime (starts out as 1f [1 second])
        //Then step the piece
        // if (Time.time >= this.stepTime && takeStep) {
        //     Step();
            
        // }
    }

    // void Step(){
    //     //Push the time forward again to be 1 second into the future
    //     //Ensures that the steps keep going
    //     this.stepTime = Time.time + stepDelay;

    //     //Move the piece down
    //     Move(new Vector2(0,-1));
    // }


    //Function that will move the piece down but a set unit
    // void Move(Vector2 translation) {
    //     Vector3 newPOS = this.position;
    //     newPOS.x += translation.x;
    //     newPOS.y += translation.y * 1/20;

    //     this.position = newPOS;

    //     this.transform.position = this.position;

    // }
}

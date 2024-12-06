using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceControl : MonoBehaviour
{
    // Continuous Movement
    private float previousTime;
    public float fallTime = 0.8f;
    public static int height = 20;
    public static int width = 10;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftArrow)) //&& inPlay())
        {
            transform.position += new Vector3(-1, 0, 0);
            if(!legal())
                transform.position -= new Vector3(-1,0,0);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)) //&& inPlay())
        {
            transform.position += new Vector3(1, 0, 0);
            if(!legal())
                transform.position -= new Vector3(1,0,0);
        }

        if(Time.time - previousTime > fallTime) //&& inPlay())
        {
            transform.position += new Vector3(0, -1, 0);
            previousTime = Time.time;
            if(!legal())
                transform.position -= new Vector3(0, -1, 0);
        }


        // Hard Drop
        if (Time.time - previousTime > (Input.GetKey(KeyCode.DownArrow) ? fallTime/10 : fallTime))
        {
            transform.position += new Vector3(0, -1, 0);
            previousTime = Time.time;
        }
    }

    bool legal()
    {
        foreach (Transform children in transform) // Browse all children 
        {
            int roundedX = Mathf.RoundToInt(children.transform.position.x); // Round X and Y
            int roundedY = Mathf.RoundToInt(children.transform.position.y);

            if(roundedX < 0 || roundedX >= width || roundedY < 0 || roundedY >= height) // If coord is outside grid size retunr false
            {
                return false;
            }
        }
        return true;
    }
}

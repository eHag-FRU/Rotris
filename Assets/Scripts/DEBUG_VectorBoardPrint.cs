using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class DEBUG_VectorBoardPrint : MonoBehaviour
{

    public TextMeshProUGUI boardTextHolder;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        //Will update the boards text output every 1f (second, when the piece moves down the board)
        Invoke("updateBoardText", 0.5f);
    }

    void updateBoardText() {

        print("UPDATING BOARD TEXT!!!");

        //Now set the text
        boardTextHolder.text = Board.printBoardVectorState();

        CancelInvoke("updateBoardText");
    }
}

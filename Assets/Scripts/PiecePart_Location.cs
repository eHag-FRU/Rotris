using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecePart_Location : MonoBehaviour
{

    [SerializeField]
    private UnityEngine.Vector2Int startingLocationReal;

    // Start is called before the first frame update
    // void Start()
    // {
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }


    public UnityEngine.Vector2Int getPartLocation() {
        return startingLocationReal;
    }

    public void updatePartLocation(int row, int column) {
        startingLocationReal = new Vector2Int(row, column);
    }


   



     

}

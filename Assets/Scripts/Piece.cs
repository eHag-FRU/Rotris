using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Piece", menuName = "Piece")]
public class Piece : ScriptableObject
{
    [SerializeField]
    private GameObject piecePrefab;
    
    [SerializeField]
    private SpriteRenderer renderer;


    void Start() {
        piecePrefab.GetComponent<SpriteRenderer>();
    }
}

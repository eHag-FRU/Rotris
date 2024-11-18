using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardRotation : MonoBehaviour
{
    [SerializeField]

    // Board transform
    private Transform boardTransform; 

    [SerializeField]
    private float rotationSpeed = 90f; 

    [SerializeField]
    // Sound played during rotation
    private AudioClip rotationSound; 

    private AudioSource audioSource;

    private bool isRotating = false;
    private Quaternion targetRotation;

    // Track rotation: (0, 90, 180, 270)
    private int currentRotationState = 0; 

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = rotationSound;

        // Set initial rotation
        targetRotation = boardTransform.rotation;
    }
        void Update()
    {
        // Check for rotation input
        if (!isRotating)
        {
             // Rotate -90
            if (Input.GetKeyDown(KeyCode.Q))
            {
                RotateBoard(-90);
            }
            // Rotate 90
            else if (Input.GetKeyDown(KeyCode.E)) 
            {
                RotateBoard(90);
            }
        }

        // Smooth rotation
        if (isRotating)
        {
            boardTransform.rotation = Quaternion.RotateTowards(boardTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            if (Quaternion.Angle(boardTransform.rotation, targetRotation) < 0.1f)
            {
                // Snap
                boardTransform.rotation = targetRotation;
                isRotating = false;

                // Adjust logic 
                //AdjustBoard();
            }
        }
    }

    private void RotateBoard(int angle)
    {
        // Update rotation state and calculate new target
        currentRotationState = (currentRotationState + angle + 360) % 360;
        targetRotation = boardTransform.rotation * Quaternion.Euler(0, 0, angle);

        // Play sound
        audioSource.Play();

        // Start rotating
        isRotating = true;
    }

    private void AdjustBoard()
    {
        // Adjust gravity or piece behavior based on currentRotationState
        switch (currentRotationState)
        {
            case 0:
                // Default gravity
                break;
            case 90:
                // Left gravity
                break;
            case 180:
                // Flipped gravity
                break;
            case 270:
                // Right gravity
                break;
        }
    }
}
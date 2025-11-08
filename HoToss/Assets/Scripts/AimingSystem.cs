using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AimingSystem : MonoBehaviour
{
    [Header("Aiming Settings")]
    public HotossGame hotossGame;   // set by GameController or via Inspector
    public Camera mainCamera;
    public Transform aimIndicator; // The visual indicator for aiming
    public float maxAimingDistance = 10f;
    public LayerMask aimLayerMask; // Mask to determine which layers can be aimed at
    public LineRenderer trajectoryLine; // Line renderer to show trajectory

    private Vector3 targetPosition;
    private bool isAiming = true;

    void Start()
    {
        // Ensure the line renderer is disabled at start
        if (trajectoryLine != null)
        {
            trajectoryLine.enabled = false;
        }
    }

    void Update()
    {
        if (isAiming)
        {
            HandleAimingInput();
        }
    }

    void HandleAimingInput()
    {
        // Handle player input for aiming (e.g., using the mouse or controller)
        if (Input.GetMouseButton(1)) // Right mouse button to aim
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxAimingDistance, aimLayerMask))
            {
                targetPosition = hit.point;
                UpdateAimIndicator(targetPosition);
                DrawTrajectoryLine(targetPosition);
            }
        }

        // Confirm the aimed position with left mouse button click
        if (Input.GetMouseButtonDown(0)) // Left mouse button to confirm
        {
            isAiming = false;
            DisableTrajectoryLine();
            // Here you can notify another script to start the power slider mechanic
        }
    }

    void UpdateAimIndicator(Vector3 position)
    {
        // Update the position of the aim indicator to show where the player is aiming
        if (aimIndicator != null)
        {
            aimIndicator.position = position;
        }
    }

    void DrawTrajectoryLine(Vector3 targetPosition)
    {
        if (trajectoryLine != null && !trajectoryLine.enabled)
            trajectoryLine.enabled = true;

        if (trajectoryLine != null)
        {
            Vector3 start = (hotossGame != null && hotossGame.throwPosition != null)
                            ? hotossGame.throwPosition.position
                            : transform.position;  // fallback if not wired

            Vector3[] positions = new Vector3[2];
            positions[0] = start;
            positions[1] = targetPosition;
            trajectoryLine.SetPositions(positions);
        }
    }


    void DisableTrajectoryLine()
    {
        if (trajectoryLine != null)
        {
            trajectoryLine.enabled = false;
        }
    }

    public Vector3 GetTargetPosition()
    {
        return targetPosition;
    }
}
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AimingSystem : MonoBehaviour
{
    [Header("Aiming Settings")]
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
        // Enable the line renderer if not already enabled
        if (trajectoryLine != null && !trajectoryLine.enabled)
        {
            trajectoryLine.enabled = true;
        }

        // Set the trajectory line points from the throw position to the target position
        if (trajectoryLine != null)
        {
            Vector3[] positions = new Vector3[2];
            positions[0] = transform.position; // Start of the line (where the player is aiming from)
            positions[1] = targetPosition; // End of the line (aimed point)
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

/*
Explanation:
1. **Aiming System**:
   - The `AimingSystem` script allows the player to aim before the throw.
   - The right mouse button (`Input.GetMouseButton(1)`) is used to control the aiming process.
   - A ray is cast from the camera to the mouse position to determine the aimed point on a valid layer (`aimLayerMask`).

2. **Trajectory Line**:
   - A `LineRenderer` component (`trajectoryLine`) is used to visually show the dotted trajectory from the throw position to the aimed target.
   - The `DrawTrajectoryLine()` method sets up the line to move with the mouse until the player confirms the aiming position.
   - The trajectory line is disabled once the player clicks the left mouse button (`Input.GetMouseButtonDown(0)`).

3. **Aim Indicator**:
   - The `aimIndicator` represents the target visually and moves to the aiming point.

4. **Confirmation of Aim**:
   - Once the left mouse button is clicked, the player confirms the aimed target, and the script transitions to the next phase, which could be the power slider mechanic.
*/

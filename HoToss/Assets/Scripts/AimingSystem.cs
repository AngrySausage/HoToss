using UnityEngine;

public class AimingSystem : MonoBehaviour
{
    [Header("Aiming Settings")]
    public Camera mainCamera;
    public Transform aimIndicator; // The visual indicator for aiming
    public float maxAimingDistance = 10f;
    public LayerMask aimLayerMask; // Mask to determine which layers can be aimed at
    public LineRenderer trajectoryLine; // Line renderer to show trajectory
    public HotossGame hotossGame; // Reference to the HotossGame script

    private Vector3 targetPosition;
    private bool isAiming = true;

    void Start()
    {
        isAiming = true; // Ensure the game starts in the aiming phase
        if (trajectoryLine != null)
        {
            trajectoryLine.enabled = false; // Hide the trajectory line
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

        if (Input.GetMouseButtonDown(0)) // Left mouse button to confirm aim
        {
            isAiming = false;
            DisableTrajectoryLine();
            if (hotossGame != null)
            {
                hotossGame.SetAimingCompleted(); // Notify HotossGame that aiming is complete
            }
        }
    }

    void UpdateAimIndicator(Vector3 position)
    {
        if (aimIndicator != null)
        {
            aimIndicator.position = position;
        }
    }

    void DrawTrajectoryLine(Vector3 targetPosition)
    {
        if (trajectoryLine != null && !trajectoryLine.enabled)
        {
            trajectoryLine.enabled = true;
        }

        if (trajectoryLine != null)
        {
            Vector3[] positions = new Vector3[2];
            positions[0] = hotossGame.throwPosition.position; // Use ThrowPosition as the start
            positions[1] = targetPosition; // Aimed target
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

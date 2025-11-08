using UnityEngine;

public class AimingSystem : MonoBehaviour
{
    public Camera mainCamera;
    public Transform throwStart;           // assign ThrowPosition
    public Transform aimIndicator;         // small sphere
    public LineRenderer trajectoryLine;    // 2-point line
    public LayerMask aimMask = ~0;         // hit any collider by default
    public float maxDistance = 200f;

    public Vector3 CurrentTarget { get; private set; }

    private readonly Vector3[] _line = new Vector3[2];

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, maxDistance, aimMask))
            {
                CurrentTarget = hit.point;

                if (aimIndicator != null) aimIndicator.position = CurrentTarget;

                if (trajectoryLine != null && throwStart != null)
                {
                    _line[0] = throwStart.position;
                    _line[1] = CurrentTarget;
                    trajectoryLine.SetPositions(_line);
                    if (!trajectoryLine.enabled) trajectoryLine.enabled = true;
                }
            }
        }
        else
        {
            if (trajectoryLine != null && trajectoryLine.enabled) trajectoryLine.enabled = false;
        }
    }

    public void ClearVisuals()
    {
        if (trajectoryLine != null) trajectoryLine.enabled = false;
    }
}

using UnityEngine;
using UnityEngine.UI;

public class ThrowController : MonoBehaviour
{
    [Header("Throw Settings")]
    public GameObject ballPrefab;      // The ball prefab to instantiate
    public float maxThrowForce = 20f;  // The maximum force (velocity magnitude) of the throw
    public float chargeDuration = 2f;  // Time (seconds) for a full charge cycle of the power bar

    [Header("Camera Settings")]
    public float baseFOV = 60f;   // Default camera Field of View
    public float maxFOV = 75f;    // FOV when fully charged (for a slight zoom-out effect)
    private Camera cam;           // Reference to the Camera component

    [Header("Charge UI")]
    public Slider chargeSlider;   // UI Slider for the charge bar
    public Image fillImage;       // Image component of the slider's fill area (to change color)
    public Color lowChargeColor = Color.green;
    public Color highChargeColor = Color.red;

    // Internal state
    private bool isCharging = false;
    private float chargeTime = 0f;  // Accumulated time for current charge cycle

    void Start()
    {
        // Cache the camera component and initial FOV
        cam = Camera.main;
        cam.fieldOfView = baseFOV;
        // Ensure the charge slider is initially hidden
        if (chargeSlider != null)
        {
            chargeSlider.gameObject.SetActive(false);
            chargeSlider.value = 0f;
        }
    }

    void Update()
    {
        // When left mouse button is pressed down, begin charging
        if (Input.GetMouseButtonDown(0))
        {
            isCharging = true;
            chargeTime = 0f;  // reset charge timer
            // Show the charge bar UI
            if (chargeSlider != null)
            {
                chargeSlider.gameObject.SetActive(true);
            }
        }

        // While holding the button, increase chargeTime and update slider & camera FOV
        if (isCharging && Input.GetMouseButton(0))
        {
            // Increase charge time
            chargeTime += Time.deltaTime;
            // Calculate charge fraction (0 to 1), looping back to 0 after full duration
            float chargeFraction = Mathf.Repeat(chargeTime, chargeDuration) / chargeDuration;
            // Update slider UI to reflect charge level
            if (chargeSlider != null)
            {
                chargeSlider.value = chargeFraction;
                // Lerp the slider fill color from green (low) to red (high) based on charge
                if (fillImage != null)
                {
                    fillImage.color = Color.Lerp(lowChargeColor, highChargeColor, chargeFraction);
                }
            }
            // Slightly increase the camera FOV based on charge (for a dynamic effect)
            if (cam != null)
            {
                cam.fieldOfView = Mathf.Lerp(baseFOV, maxFOV, chargeFraction);
            }
        }

        // On releasing the left mouse button, launch the ball
        if (isCharging && Input.GetMouseButtonUp(0))
        {
            isCharging = false;
            // Hide the charge bar UI
            if (chargeSlider != null)
            {
                chargeSlider.gameObject.SetActive(false);
            }
            // Reset camera FOV to normal
            if (cam != null)
            {
                cam.fieldOfView = baseFOV;
            }
            // Calculate final charge fraction one more time (how far along the bar when released)
            float finalChargeFraction = Mathf.Repeat(chargeTime, chargeDuration) / chargeDuration;
            // Spawn and throw the ball with velocity based on charge
            ThrowBall(finalChargeFraction);
        }
    }

    // This method instantiates the ball prefab and applies velocity to throw it
    private void ThrowBall(float chargeFraction)
    {
        if (ballPrefab == null || cam == null) return;  // safety check

        // Determine spawn position: slightly in front of the camera to avoid immediate collisions
        Vector3 spawnPos = cam.transform.position + cam.transform.forward * 0.5f;
        // Instantiate the ball prefab at the spawn position with no rotation
        GameObject ball = Instantiate(ballPrefab, spawnPos, Quaternion.identity);

        // Aim direction: a ray from the camera through the mouse position into the world
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Vector3 aimDirection;
        // If the ray hits something (e.g., a can or the table), aim towards that point; otherwise aim far
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f))
        {
            aimDirection = (hit.point - spawnPos).normalized;
        }
        else
        {
            // If nothing is hit, use the ray's direction (towards where the cursor points in space)
            aimDirection = ray.direction.normalized;
        }

        // Calculate throw force based on charge fraction
        float throwForce = chargeFraction * maxThrowForce;
        // Apply the velocity to the ball's Rigidbody to launch it
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = aimDirection * throwForce;
        }
    }
}

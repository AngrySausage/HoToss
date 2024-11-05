// Assuming you're using Unity as your development environment and C# as your scripting language, below is an implementation for the power slider and the ball throwing mechanic.

using UnityEngine;
using UnityEngine.UI;  // Assuming you will use a UI slider to represent the power level
using System.Collections; // Needed for IEnumerator

public class HoTossGame : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider powerSlider; // Slider to indicate charge power
    public Image sliderFillImage; // The fill area of the slider to change color dynamically

    [Header("Ball Settings")]
    public GameObject ballPrefab;
    public Transform throwPosition;
    public float maxThrowForce = 20f;
    public float minChargeTime = 0.25f;
    public float maxChargeTime = 1f;

    [Header("Camera Settings")]
    public Camera mainCamera; // Main camera to adjust FOV
    public float minFOV = 90f;
    public float maxFOV = 60f;

    private float currentChargeTime = 0f;
    private float randomMaxChargeTime;
    private bool isCharging = false;
    private bool isIncreasing = true;

    void Update()
    {
        HandlePowerSlider();
    }

    void HandlePowerSlider()
    {
        // Start charging when left mouse button is pressed
        if (Input.GetMouseButtonDown(0))
        {
            isCharging = true;
            currentChargeTime = 0f;
            randomMaxChargeTime = Mathf.Pow(Random.value, 1.5f) * (maxChargeTime - minChargeTime) + minChargeTime; // Randomize max charge time each throw
            powerSlider.value = 0f;
            isIncreasing = true;
        }

        // Continue charging while left mouse button is held
        if (isCharging && Input.GetMouseButton(0))
        {
            if (isIncreasing)
            {
                currentChargeTime += Time.deltaTime;
                if (currentChargeTime >= randomMaxChargeTime)
                {
                    currentChargeTime = randomMaxChargeTime;
                    isIncreasing = false;
                }
            }
            else
            {
                currentChargeTime -= Time.deltaTime;
                if (currentChargeTime <= 0f)
                {
                    currentChargeTime = 0f;
                    isIncreasing = true;
                }
            }

            float chargeRatio = Mathf.Clamp01(currentChargeTime / randomMaxChargeTime);
            powerSlider.value = chargeRatio;
            UpdateSliderColor(chargeRatio);
            UpdateCameraFOV(chargeRatio);
        }

        // Throw the ball when the left mouse button is released
        if (isCharging && Input.GetMouseButtonUp(0))
        {
            isCharging = false;
            float finalPower = Mathf.Clamp01(currentChargeTime / randomMaxChargeTime);
            StartCoroutine(ThrowBallWithDelay(finalPower));
            // StartCoroutine(ResetCameraFOV());
        }
    }

    void ThrowBall(float chargeRatio)
    {
        GameObject newBall = Instantiate(ballPrefab, throwPosition.position, throwPosition.rotation);
        Rigidbody ballRb = newBall.GetComponent<Rigidbody>();
        if (ballRb != null)
        {
            float throwForce = chargeRatio * maxThrowForce;
            Vector3 throwDirection = CalculateThrowDirection();
            ballRb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
            powerSlider.value = 0;
        StartCoroutine(ResetCameraFOV());
        }
    }

    IEnumerator ThrowBallWithDelay(float chargeRatio)
    {
        // Wait for a short delay before throwing the ball
        yield return new WaitForSeconds(0.2f); // Adjust the delay duration as needed
        ThrowBall(chargeRatio);
    }

    Vector3 CalculateThrowDirection()
    {
        // Basic direction towards where the player is aiming
        Vector3 baseDirection = throwPosition.forward;
        
        // Random skewing depending on the power level
        float skewFactor = Random.Range(-0.1f, 0.1f); // Adjust this range for randomness
        Vector3 skewDirection = new Vector3(skewFactor, 0, 1).normalized;
        
        return (baseDirection + skewDirection).normalized;
    }

    void UpdateSliderColor(float chargeRatio)
    {
        // Change slider color from green (low charge) to yellow (mid charge) to red (high charge)
        Color lowColor = Color.green;
        Color midColor = Color.yellow;
        Color highColor = Color.red;

        if (chargeRatio < 0.5f)
        {
            // Interpolate between green and yellow
            sliderFillImage.color = Color.Lerp(lowColor, midColor, chargeRatio * 2f);
        }
        else
        {
            // Interpolate between yellow and red
            sliderFillImage.color = Color.Lerp(midColor, highColor, (chargeRatio - 0.5f) * 2f);
        }
    }

    void UpdateCameraFOV(float chargeRatio)
    {
        // Adjust the camera's field of view (FOV) based on the charge level
        if (mainCamera != null)
        {
            mainCamera.fieldOfView = Mathf.Lerp(minFOV, maxFOV, chargeRatio);
        }
    }

    IEnumerator ResetCameraFOV()
    {
        // Gradually reset the camera's FOV to the minimum value
        if (mainCamera != null)
        {
            float currentFOV = mainCamera.fieldOfView;
            float duration = 0.1f; // Time over which FOV resets
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                mainCamera.fieldOfView = Mathf.Lerp(currentFOV, minFOV, elapsedTime / duration);
                yield return null;
            }
            mainCamera.fieldOfView = minFOV;
        }
    }
} 

/*
Explanation:
1. **Power Slider**:
   - A UI slider (`powerSlider`) is used to visually represent the charging progress. 
   - `isCharging` tracks whether the player is currently charging the throw. The left mouse button (M1) controls this.
   - The `currentChargeTime` increments over time, giving the slider its value based on how long M1 is held.
   - `randomMaxChargeTime` is randomized every time the player starts charging, providing a different maximum charge time for each throw.
   - The slider value oscillates between 0 and `randomMaxChargeTime` to challenge the player to time their shot.
   - On releasing M1, the power is calculated as a ratio of the `randomMaxChargeTime` to determine throw power.

2. **Ball Throw Mechanic**:
   - When the power is finalized, a new ball object (`ballPrefab`) is instantiated at the player's position.
   - The ball is given a force proportional to the charging level (`chargeRatio * maxThrowForce`).
   - A random skew is applied in `CalculateThrowDirection` using `Random.Range()`.
   - The skew factor gives the ball a randomized slight curve to the left or right, as per your requested feature.

3. **Slider Color Change**:
   - The color of the `sliderFillImage` changes dynamically based on the current charge level (`chargeRatio`).
   - It starts from green at low charge, transitions to yellow at mid charge, and finally to red at high charge to indicate increasing power.

4. **Camera FOV Adjustment**:
   - The main camera's field of view (`FOV`) is adjusted based on the current charge level (`chargeRatio`).
   - The FOV starts at a minimum value (`minFOV`) when the charge is low and increases to a maximum value (`maxFOV`) as the charge increases, creating a zoom effect.
   - After throwing the ball, the FOV is reset to the minimum value (`minFOV`) gradually to maintain a smooth transition.
*/

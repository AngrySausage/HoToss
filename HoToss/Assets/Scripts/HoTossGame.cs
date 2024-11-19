using UnityEngine;
using UnityEngine.UI; 
using System.Collections; // Needed for IEnumerator

public class HotossGame : MonoBehaviour
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
    private bool isAimingCompleted = false; // Keeps track of whether aiming is finished
    
    void Start()
    {
        isAimingCompleted = false; // Ensure aiming must finish before power slider activates
        powerSlider.gameObject.SetActive(false); // Hide power slider initially
    }


    public void SetAimingCompleted()
    {
        Debug.Log("Aiming Completed - Power Slider Activated");
        isAimingCompleted = true;
        powerSlider.gameObject.SetActive(true); // Show the power slider
    }

    void Update()
    {
        if (isAimingCompleted)
        {
            HandlePowerSlider();
        }
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
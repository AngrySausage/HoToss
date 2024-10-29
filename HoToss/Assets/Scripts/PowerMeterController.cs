using UnityEngine;
using UnityEngine.UI;

public class PowerMeterController : MonoBehaviour
{
    public Slider powerSlider;        // Reference to the slider UI element
    public float chargeSpeed = 1f;    // How quickly the meter charges up
    private bool isChargingUp = true; // Whether the bar is filling up or down
    private bool isStopped = true;    // Whether the player has stopped the meter

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isStopped = false;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isStopped = true;
        }

        if (isStopped) return; // Stop the meter if the player releases the mouse button

        // Increase or decrease the slider value based on direction
        if (isChargingUp)
        {
            powerSlider.value += chargeSpeed * Time.deltaTime;
            if (powerSlider.value >= powerSlider.maxValue)
                isChargingUp = false; // Change direction when max is reached
        }
        else
        {
            powerSlider.value -= chargeSpeed * Time.deltaTime;
            if (powerSlider.value <= powerSlider.minValue)
                isChargingUp = true; // Change direction when min is reached
        }
    }

    public void RestartMeter()
    {
        isStopped = true;
        powerSlider.value = powerSlider.minValue;
        isChargingUp = true;
    }
}

using UnityEngine;
using UnityEngine.UI;

public class HotossGame : MonoBehaviour
{
    [Header("Scene")]
    public Transform throwPosition;      // ThrowPosition object
    public GameObject ballPrefab;        // must have Rigidbody
    public AimingSystem aiming;          // reference the AimingSystem

    [Header("UI")]
    public Slider powerSlider;           // set Min 0, Max 1
    public Image sliderFillImage;        // optional, can be null

    [Header("Tuning")]
    public float chargeSpeed = 1.5f;     // how fast the slider ping-pongs
    public float maxThrowForce = 25f;    // impulse at slider value 1
    public float minThrowForce = 5f;     // impulse at slider value 0

    private bool _charging;
    private float _t;                    // time driver for PingPong
    private float _charge;               // 0..1

    private void Start()
    {
        if (powerSlider != null)
        {
            powerSlider.minValue = 0f;
            powerSlider.maxValue = 1f;
            powerSlider.value = 0f;
            powerSlider.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // start charging
        if (Input.GetMouseButtonDown(0))
        {
            _charging = true;
            _t = 0f;
            if (powerSlider != null) powerSlider.gameObject.SetActive(true);
        }

        // update charge and slider while held
        if (_charging && Input.GetMouseButton(0))
        {
            _t += Time.deltaTime * chargeSpeed;
            _charge = Mathf.PingPong(_t, 1f);          // cycles up and down
            if (powerSlider != null) powerSlider.value = _charge;

            // simple optional colour cue
            if (sliderFillImage != null)
            {
                // green to red through yellow
                sliderFillImage.color = Color.Lerp(Color.green, Color.red, _charge);
            }
        }

        // release to throw
        if (_charging && Input.GetMouseButtonUp(0))
        {
            _charging = false;
            if (powerSlider != null) powerSlider.gameObject.SetActive(false);
            ThrowBall();
            if (aiming != null) aiming.ClearVisuals();
        }
    }

    private void ThrowBall()
    {
        if (ballPrefab == null || throwPosition == null) return;

        Vector3 target = (aiming != null) ? aiming.CurrentTarget : throwPosition.position + transform.forward;
        Vector3 dir = target - throwPosition.position;
        if (dir.sqrMagnitude < 1e-6f) dir = transform.forward;

        float force = Mathf.Lerp(minThrowForce, maxThrowForce, _charge);

        GameObject ball = Instantiate(ballPrefab, throwPosition.position, Quaternion.identity);
        if (ball.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.AddForce(dir.normalized * force, ForceMode.Impulse);
        }
    }
}

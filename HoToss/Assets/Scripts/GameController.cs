using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    // Drag these in the Inspector
    public AimingSystem aimingSystem;
    public HotossGame hotossGame;

    public Transform throwPosition;
    public LineRenderer trajectoryLine;
    public Transform aimIndicator;
    public Camera mainCamera;

    public Slider powerSlider;
    public Image powerFillImage;

    private void Awake()
    {
        // Wire HotossGame
        if (hotossGame != null)
        {
            if (hotossGame.throwPosition == null)   hotossGame.throwPosition = throwPosition;
            if (hotossGame.powerSlider == null)     hotossGame.powerSlider = powerSlider;
            if (hotossGame.sliderFillImage == null) hotossGame.sliderFillImage = powerFillImage;
        }

        // Wire AimingSystem
        if (aimingSystem != null)
        {
            if (aimingSystem.mainCamera == null)      aimingSystem.mainCamera = mainCamera;
            if (aimingSystem.throwStart == null)      aimingSystem.throwStart = throwPosition;
            if (aimingSystem.aimIndicator == null)    aimingSystem.aimIndicator = aimIndicator;
            if (aimingSystem.trajectoryLine == null)  aimingSystem.trajectoryLine = trajectoryLine;
        }
    }
}

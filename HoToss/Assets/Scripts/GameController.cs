using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    // Public on purpose so you can wire in the Inspector without attributes.
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
        // Wire HotossGame references if they are not already set in the Inspector
        if (hotossGame != null)
        {
            if (hotossGame.throwPosition == null)   hotossGame.throwPosition = throwPosition;
            if (hotossGame.mainCamera == null)      hotossGame.mainCamera = mainCamera;
            if (hotossGame.powerSlider == null)     hotossGame.powerSlider = powerSlider;
            if (hotossGame.sliderFillImage == null) hotossGame.sliderFillImage = powerFillImage;
        }

        // Wire AimingSystem references if they are not already set
        if (aimingSystem != null)
        {
            if (aimingSystem.mainCamera == null)      aimingSystem.mainCamera = mainCamera;
            if (aimingSystem.aimIndicator == null)     aimingSystem.aimIndicator = aimIndicator;
            if (aimingSystem.trajectoryLine == null)   aimingSystem.trajectoryLine = trajectoryLine;
            if (aimingSystem.hotossGame == null)       aimingSystem.hotossGame = hotossGame;
        }
    }
}

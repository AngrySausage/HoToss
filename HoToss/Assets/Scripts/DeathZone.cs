using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DeathZone : MonoBehaviour
{
    [SerializeField] private string canTag = "Can";
    private GameManager gm;

    private void Awake()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true; // enforce trigger at runtime
        gm = FindObjectOfType<GameManager>();
        if (gm == null)
        {
            Debug.LogError("GameManager not found in scene. Please add one.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other || !other.gameObject) return;

        if (other.CompareTag(canTag))
        {
            if (gm != null)
            {
                gm.OnCanEnteredDeathZone(other.gameObject);
            }
        }
    }
}

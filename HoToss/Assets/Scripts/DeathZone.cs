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
        CacheGameManager();
        if (gm == null)
        {
            Debug.LogError("GameManager not found in scene. Please add one.", this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;

        GameObject canObject = other.attachedRigidbody != null
            ? other.attachedRigidbody.gameObject
            : other.gameObject;

        if (canObject != null && canObject.CompareTag(canTag))
        {
            if (gm == null)
            {
                CacheGameManager();
                if (gm == null) return;
            }

            gm.OnCanEnteredDeathZone(canObject);
        }
    }

    private void CacheGameManager()
    {
        if (gm == null)
        {
            gm = FindObjectOfType<GameManager>();
        }
    }
}

using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // This method is called whenever a collider enters the trigger zone
        // We only care if the object entering is a can
        if (other.CompareTag("Can"))
        {
            // A can has fallen into the zone – increment the knockdown counter
            GameManager gm = FindObjectOfType<GameManager>();
            if (gm != null)
            {
                gm.OnCanKnockedDown();  // notify GameManager that a can was knocked down
            }
            // Destroy the can so it doesn’t trigger multiple times (optional)
            Destroy(other.gameObject);
        }
    }
}

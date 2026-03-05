using UnityEngine;

public class HatCollectible : MonoBehaviour
{
    [Header("Settings")]
    public string hatID;

    public ParticleSystem pickupParticles;
    public AudioClip pickupSound;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHatManager hatManager = other.GetComponent<PlayerHatManager>();

            if (hatManager != null)
            {
                hatManager.EquipHat(hatID);

                if (pickupParticles != null)
                {
                    Instantiate(pickupParticles, transform.position, Quaternion.identity);
                }

                if (pickupSound != null)
                {
                    AudioSource.PlayClipAtPoint(pickupSound, transform.position);
                }

                Destroy(gameObject);
            }
        }
    }
}
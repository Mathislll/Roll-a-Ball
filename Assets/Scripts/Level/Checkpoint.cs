using UnityEngine;
using MoreMountains.Feedbacks;


public class Checkpoint : MonoBehaviour
{
    private bool isActivated = false;
    public AudioSource audioSource;
    public AudioClip activationSound;
    public ParticleSystem activationParticles;
    public MMF_Player triggerFeedback;
    public Transform respawnPoint;

    void OnTriggerEnter(Collider other)
    {
        if (isActivated) return;

        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                player.SetCheckpoint(respawnPoint.transform.position);
                isActivated = true;

                if (audioSource != null && activationSound != null)
                {
                    audioSource.PlayOneShot(activationSound);
                }

                if (activationParticles != null)
                {

                    activationParticles.Play();
                }

                if (triggerFeedback != null)
                {
                    triggerFeedback.PlayFeedbacks();
                }
            }
        }
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEndTrigger : MonoBehaviour
{
    public string levelToLoad;

    [Header("Animation Settings")]
    public float totalAnimationDuration = 2f;
    public float elevationAmount = 1.5f;
    public float rotationSpeed = 720f;
    public float maxScaleMultiplier = 1.3f;

    [Header("Post Animation Settings")]
    public float levelLoadDelay = 1f;
    public AudioSource audioSource;
    public AudioClip winSound;
    public AudioClip disappearSound;
    public ParticleSystem confettiParticles;

    private bool isTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;

        if (other.CompareTag("Player"))
        {
            isTriggered = true;
            StartCoroutine(TransitionRoutine(other.gameObject));
        }
    }

    private IEnumerator TransitionRoutine(GameObject player)
    {
        if (audioSource != null && winSound != null)
        {
            audioSource.PlayOneShot(winSound);
        }

        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            playerRb.isKinematic = true;
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
        }

        Vector3 startPosition = player.transform.position;
        Vector3 targetPosition = startPosition + Vector3.up * elevationAmount;
        Vector3 startScale = player.transform.localScale;
        Vector3 maxScale = startScale * maxScaleMultiplier;

        float halfDuration = totalAnimationDuration / 2f;
        float timeElapsed = 0f;

        while (timeElapsed < halfDuration)
        {
            player.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
            player.transform.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed / halfDuration);
            player.transform.localScale = Vector3.Lerp(startScale, maxScale, timeElapsed / halfDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        timeElapsed = 0f;

        while (timeElapsed < halfDuration)
        {
            player.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
            player.transform.localScale = Vector3.Lerp(maxScale, Vector3.zero, timeElapsed / halfDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        player.transform.localScale = Vector3.zero;

        if (confettiParticles != null)
        {
            confettiParticles.Play();
        }

        if (audioSource != null && disappearSound != null)
        {
            audioSource.PlayOneShot(disappearSound);
        }

        yield return new WaitForSeconds(levelLoadDelay);

        SceneManager.LoadScene(levelToLoad);
    }
}
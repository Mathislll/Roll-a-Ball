using UnityEngine;
using System.Collections;

public class BouncyObstacle : MonoBehaviour
{
    [Header("Reglages du Rebond")]
    public float knockbackForce = 20f;

    [Range(0f, 90f)]
    public float elevationAngle = 30f;

    [Header("Animation Impact")]
    public float animationDuration = 0.1f;
    public float scaleMultiplier = 1.3f;

    private Rigidbody rb;
    private Vector3 originalScale;
    private bool isAnimating;

    void Start()
    {
        originalScale = transform.localScale;
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.isKinematic = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();

            if (player != null)
            {
                Vector3 directionToPlayer = collision.transform.position - transform.position;
                Vector3 flatDirection = new Vector3(directionToPlayer.x, 0f, directionToPlayer.z).normalized;

                float angleInRadians = elevationAngle * Mathf.Deg2Rad;
                Vector3 finalDirection = flatDirection * Mathf.Cos(angleInRadians) + Vector3.up * Mathf.Sin(angleInRadians);

                player.ApplyKnockback(finalDirection.normalized, knockbackForce);

                if (!isAnimating)
                {
                    StartCoroutine(AnimateImpact());
                }
            }
        }
    }

    IEnumerator AnimateImpact()
    {
        isAnimating = true;

        Vector3 targetScale = originalScale * scaleMultiplier;
        float halfDuration = animationDuration / 2f;
        float time = 0f;

        while (time < halfDuration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, time / halfDuration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
        time = 0f;

        while (time < halfDuration)
        {
            transform.localScale = Vector3.Lerp(targetScale, originalScale, time / halfDuration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;
        isAnimating = false;
    }
}
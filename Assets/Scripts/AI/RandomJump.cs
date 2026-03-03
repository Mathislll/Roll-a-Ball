using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class RandomJump : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent parentAgent;

    [Header("Reglages du Saut")]
    public bool jumpWhileMoving = false;
    public float minJumpInterval = 0.5f;
    public float maxJumpInterval = 2.0f;
    public float jumpHeight = 1.5f;
    public float jumpDuration = 0.4f;

    private float jumpTimer;
    private bool isJumping;
    private Vector3 startLocalPos;

    void Start()
    {
        startLocalPos = transform.localPosition;
        ResetTimer();
    }

    void Update()
    {
        if (isJumping) return;

        bool isStandingStill = parentAgent.velocity.magnitude < 0.1f;

        if (jumpWhileMoving || isStandingStill)
        {
            jumpTimer = jumpTimer - Time.deltaTime;

            if (jumpTimer <= 0)
            {
                StartCoroutine(JumpRoutine());
            }
        }
        else
        {
            ResetTimer();
        }
    }

    IEnumerator JumpRoutine()
    {
        isJumping = true;
        float timeElapsed = 0f;

        while (timeElapsed < jumpDuration)
        {
            float normalizedTime = timeElapsed / jumpDuration;
            float currentHeight = Mathf.Sin(normalizedTime * Mathf.PI) * jumpHeight;

            transform.localPosition = new Vector3(startLocalPos.x, startLocalPos.y + currentHeight, startLocalPos.z);

            timeElapsed = timeElapsed + Time.deltaTime;
            yield return null;
        }

        transform.localPosition = startLocalPos;
        ResetTimer();
        isJumping = false;
    }

    void ResetTimer()
    {
        jumpTimer = Random.Range(minJumpInterval, maxJumpInterval);
    }
}
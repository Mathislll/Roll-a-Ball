using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState { Idle, Patrol, Chase, Attack, Dead }
    public EnemyState currentState;

    [Header("Combat Settings")]
    public int damageAmount = 1;
    public float attackPauseDuration = 1.5f;

    [Header("Damage Knockback Settings")]
    public float damageKnockbackForce = 15f;
    [Range(0f, 90f)]
    public float damageKnockbackAngle = 30f;

    [Header("Stomp Knockback Settings")]
    public float stompKnockbackForce = 25f;
    [Range(0f, 90f)]
    public float stompKnockbackAngle = 80f;
    public AudioClip stompSound;

    [Header("Launch Settings")]
    public float launchForce = 15f;
    public AudioClip launchSound;
    private Rigidbody rb;
    private bool isLaunched;

    [Header("Health Settings")]
    public Transform enemyModel;
    public ParticleSystem deathParticles;
    public AudioSource audioSource;
    public AudioClip deathSound;
    public float destroyDelay = 1f;

    [Header("Juicy Squash Animation")]
    public float squashDuration = 0.25f;
    public AnimationCurve squashCurveY = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(0.7f, 0.05f), new Keyframe(1f, 0.1f));
    public AnimationCurve squashCurveXZ = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(0.7f, 2f), new Keyframe(1f, 1.5f));

    [Header("Navigation")]
    public GameObject patrolZone;
    public float patrolSpeed = 3.5f;
    public float chaseSpeed = 6f;
    public float patrolRandomRadius = 5f;

    [Header("Detection")]
    public Transform player;
    public float detectionRange = 10f;
    public float chaseExitDelay = 3f;

    [Header("Debug UI")]
    public TextMeshPro stateText;

    private NavMeshAgent agent;
    private Animator anim;
    private List<Transform> patrolPoints = new List<Transform>();
    private float chaseTimer;
    private bool isAttacking;
    private bool isWaiting;
    private Camera mainCamera;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        mainCamera = Camera.main;

        if (patrolZone != null)
        {
            foreach (Transform child in patrolZone.transform)
            {
                patrolPoints.Add(child);
            }
        }

        currentState = EnemyState.Patrol;
        SetNextPatrolPoint();
    }

    void Update()
    {
        if (stateText != null)
        {
            stateText.text = currentState.ToString();
        }

        if (currentState == EnemyState.Dead || isLaunched) return;

        if (player == null || isAttacking)
        {
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool playerIsAlive = player.gameObject.activeInHierarchy;

        if (playerIsAlive && distanceToPlayer <= detectionRange)
        {
            if (currentState != EnemyState.Chase)
            {
                currentState = EnemyState.Chase;
                agent.isStopped = false;
                isWaiting = false;
            }
            chaseTimer = 0f;
        }
        else if (currentState == EnemyState.Chase)
        {
            chaseTimer += Time.deltaTime;
            if (chaseTimer >= chaseExitDelay || !playerIsAlive)
            {
                currentState = EnemyState.Patrol;
                SetNextPatrolPoint();
            }
        }

        LogicStateMachine();
        UpdateAnimator();
    }

    void LateUpdate()
    {
        if (stateText != null && mainCamera != null)
        {
            stateText.transform.rotation = Quaternion.LookRotation(stateText.transform.position - mainCamera.transform.position);
        }
    }

    void LogicStateMachine()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                if (!agent.pathPending && agent.remainingDistance < 0.5f && !isWaiting)
                {
                    StartCoroutine(IdleRoutine());
                }
                break;

            case EnemyState.Chase:
                agent.speed = chaseSpeed;
                agent.SetDestination(player.position);
                break;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (currentState == EnemyState.Dead) return;

        if (isLaunched)
        {
            if (!collision.gameObject.CompareTag("Player"))
            {
                Die();
            }
            return;
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerScript = collision.gameObject.GetComponent<PlayerController>();
            if (playerScript == null) return;

            if (playerScript.isHatInvincible)
            {
                LaunchEnemy();
                return;
            }

            bool isHeadStomp = false;
            foreach (ContactPoint contact in collision.contacts)
            {
                if (contact.normal.y < -0.5f)
                {
                    isHeadStomp = true;
                    break;
                }
            }

            Vector3 directionToPlayer = collision.transform.position - transform.position;
            Vector3 flatDirection = new Vector3(directionToPlayer.x, 0f, directionToPlayer.z).normalized;

            if (isHeadStomp)
            {
                float angleInRadians = stompKnockbackAngle * Mathf.Deg2Rad;
                Vector3 finalDirection = flatDirection * Mathf.Cos(angleInRadians) + Vector3.up * Mathf.Sin(angleInRadians);
                playerScript.ApplyKnockback(finalDirection.normalized, stompKnockbackForce);

                if (audioSource != null && stompSound != null)
                {
                    audioSource.PlayOneShot(stompSound);
                }
                Die();
            }
            else if (!isAttacking)
            {
                float angleInRadians = damageKnockbackAngle * Mathf.Deg2Rad;
                Vector3 finalDirection = flatDirection * Mathf.Cos(angleInRadians) + Vector3.up * Mathf.Sin(angleInRadians);

                playerScript.TakeDamage(damageAmount);
                playerScript.ApplyKnockback(finalDirection.normalized, damageKnockbackForce);
                StartCoroutine(AttackRoutine());
            }
        }
    }

    void LaunchEnemy()
    {
        isLaunched = true;

        if (agent != null)
        {
            agent.enabled = false;
        }

        if (rb != null)
        {
            rb.isKinematic = false;

            if (audioSource != null && launchSound != null)
            {
                audioSource.PlayOneShot(launchSound);
            }

            Vector3 pushDirection = transform.position - player.position;
            pushDirection.y = 0f;
            pushDirection = pushDirection.normalized;

            pushDirection.y = Random.Range(0.8f, 1.2f);
            pushDirection.x += Random.Range(-0.4f, 0.4f);
            pushDirection.z += Random.Range(-0.4f, 0.4f);

            Vector3 finalDirection = pushDirection.normalized;

            rb.AddForce(finalDirection * launchForce, ForceMode.Impulse);
            rb.AddTorque(new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f)), ForceMode.Impulse);
        }
    }

    public void Die()
    {
        if (currentState == EnemyState.Dead) return;

        currentState = EnemyState.Dead;

        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }

        if (agent != null)
        {
            agent.enabled = false;
        }

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        if (deathParticles != null) deathParticles.Play();

        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        StartCoroutine(SquashRoutine());
    }

    IEnumerator SquashRoutine()
    {
        if (enemyModel != null)
        {
            Vector3 originalScale = enemyModel.localScale;
            float time = 0f;

            while (time < squashDuration)
            {
                float normalizedTime = time / squashDuration;
                float scaleY = squashCurveY.Evaluate(normalizedTime);
                float scaleXZ = squashCurveXZ.Evaluate(normalizedTime);
                enemyModel.localScale = new Vector3(originalScale.x * scaleXZ, originalScale.y * scaleY, originalScale.z * scaleXZ);
                time += Time.deltaTime;
                yield return null;
            }

            enemyModel.localScale = new Vector3(originalScale.x * squashCurveXZ.Evaluate(1f), originalScale.y * squashCurveY.Evaluate(1f), originalScale.z * squashCurveXZ.Evaluate(1f));
            enemyModel.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }

    void SetNextPatrolPoint()
    {
        if (patrolPoints.Count == 0 || isAttacking || currentState == EnemyState.Dead || isLaunched) return;

        agent.speed = patrolSpeed;
        int randomIndex = Random.Range(0, patrolPoints.Count);
        Vector3 centerPoint = patrolPoints[randomIndex].position;
        Vector3 randomDirection = Random.insideUnitSphere * patrolRandomRadius;
        randomDirection += centerPoint;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRandomRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            agent.SetDestination(centerPoint);
        }
    }

    IEnumerator IdleRoutine()
    {
        isWaiting = true;
        currentState = EnemyState.Idle;
        if (agent.enabled && agent.isOnNavMesh) agent.isStopped = true;

        yield return new WaitForSeconds(Random.Range(1f, 3f));

        if (currentState != EnemyState.Chase && currentState != EnemyState.Dead && !isLaunched)
        {
            currentState = EnemyState.Patrol;
            if (agent.enabled && agent.isOnNavMesh) agent.isStopped = false;
            SetNextPatrolPoint();
        }
        isWaiting = false;
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        currentState = EnemyState.Attack;
        if (agent.enabled && agent.isOnNavMesh) agent.isStopped = true;

        if (anim != null)
        {
            anim.Play("Attack", 0, 0f);
        }

        yield return new WaitForSeconds(attackPauseDuration);

        if (currentState != EnemyState.Dead && !isLaunched)
        {
            if (agent.enabled && agent.isOnNavMesh) agent.isStopped = false;
            isAttacking = false;
            currentState = EnemyState.Chase;
        }
    }

    void UpdateAnimator()
    {
        if (anim == null || currentState == EnemyState.Dead) return;

        if (isAttacking)
        {
            anim.SetFloat("Speed", 0f);
            anim.SetBool("IsChasing", false);
            anim.SetBool("IsIdle", false);
            return;
        }

        float speed = agent.velocity.magnitude;
        anim.SetFloat("Speed", speed);
        anim.SetBool("IsChasing", currentState == EnemyState.Chase);
        anim.SetBool("IsIdle", currentState == EnemyState.Idle);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
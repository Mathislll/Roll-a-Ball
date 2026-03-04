using MoreMountains.Feedbacks;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private Animator anim;
    private float movementX;
    private float movementY;

    [Header("Visuals")]
    public GameObject playerModel;

    [Header("Ground control")]
    public float groundAcceleration = 60f;
    public float maxGroundSpeed = 10f;
    public float groundFriction = 10f;

    [Header("Air control")]
    public float airAcceleration = 30f;
    public float maxAirSpeed = 10f;
    public float airFriction = 1f;

    [Header("Custom gravity")]
    public float gravityMultiplier = 5f;

    [Header("Jump")]
    public float jumpForce = 15f;
    public float jumpDelay = 0.15f;

    [Header("Ground detection")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private bool isGrounded;

    [Header("Health System")]
    public int maxLives = 3;
    private int currentLives;
    public float respawnDelay = 1f;
    public float invincibilityDuration = 1.5f;
    private bool isDead;
    private bool isInvincible;
    private Vector3 currentCheckpointPosition;

    [Header("Audio System")]
    public AudioSource audioSource;
    public AudioClip[] loseLifeSounds;
    public AudioClip deathSound;
    public AudioClip respawnSound;

    private PlayerScore playerScore;
    private bool isKnockedBack;
    public MMF_Player damageFeedback;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        rb.linearDamping = 0f;
        playerScore = GetComponent<PlayerScore>();

        currentLives = maxLives;
        currentCheckpointPosition = transform.position;

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void OnMove(InputValue movementValue)
    {
        if (isDead) return;

        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    void OnJump(InputValue value)
    {
        if (isDead) return;

        if (isGrounded)
        {
            if (anim != null) anim.SetTrigger("Jump");
            StartCoroutine(JumpRoutine());
        }
    }

    private IEnumerator JumpRoutine()
    {
        yield return new WaitForSeconds(jumpDelay);
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (anim != null)
        {
            anim.SetBool("IsGrounded", isGrounded);
            anim.SetFloat("VerticalVelocity", rb.linearVelocity.y);
        }
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        if (isKnockedBack)
        {
            if (!isGrounded)
            {
                rb.linearVelocity += Vector3.up * Physics.gravity.y * (gravityMultiplier - 1f) * Time.fixedDeltaTime;
            }

            return;
        }

        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        float currentAccel = isGrounded ? groundAcceleration : airAcceleration;
        float currentMaxSpeed = isGrounded ? maxGroundSpeed : maxAirSpeed;

        rb.AddForce(movement * currentAccel);

        if (movement.magnitude == 0)
        {
            float friction = isGrounded ? groundFriction : airFriction;
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, new Vector3(0f, rb.linearVelocity.y, 0f), friction * Time.fixedDeltaTime);
        }

        Vector3 horizontalVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (horizontalVel.magnitude > currentMaxSpeed)
        {
            Vector3 limitedVel = horizontalVel.normalized * currentMaxSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }

        if (!isGrounded)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (gravityMultiplier - 1f) * Time.fixedDeltaTime;
        }
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        if (isDead) return;

        isKnockedBack = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(direction * force, ForceMode.Impulse);
        StartCoroutine(KnockbackRecovery());
    }

    private IEnumerator KnockbackRecovery()
    {
        yield return new WaitForSeconds(0.1f);

        while (rb.linearVelocity.magnitude > maxGroundSpeed)
        {
            yield return new WaitForFixedUpdate();
        }

        isKnockedBack = false;
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead || isInvincible) return;

        currentLives = currentLives - damageAmount;

        if (currentLives > 0)
        {
            if (loseLifeSounds.Length > 0)
            {
                int randomIndex = Random.Range(0, loseLifeSounds.Length);
                PlaySound(loseLifeSounds[randomIndex]);
                damageFeedback?.PlayFeedbacks();
            }
            StartCoroutine(InvincibilityRoutine());
        }
        else
        {
            Die();
        }
    }

    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    public void Die()
    {
        isDead = true;
        isKnockedBack = false;
        movementX = 0f;
        movementY = 0f;

        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (playerModel != null)
        {
            playerModel.SetActive(false);
        }

        PlaySound(deathSound);
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);

        transform.position = currentCheckpointPosition;

        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (playerModel != null)
        {
            playerModel.SetActive(true);
        }

        currentLives = maxLives;

        PlaySound(respawnSound);

        isDead = false;
        StartCoroutine(InvincibilityRoutine());
    }

    public void SetCheckpoint(Vector3 newPosition)
    {
        currentCheckpointPosition = newPosition;
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            if (playerScore != null)
            {
                playerScore.count++;
                playerScore.SetCountText();
            }
        }
    }
}
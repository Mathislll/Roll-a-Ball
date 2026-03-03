using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private Animator anim;
    private float movementX;
    private float movementY;

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

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        rb.linearDamping = 0f;
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    void OnJump(InputValue value)
    {
        if (isGrounded)
        {
            anim.SetTrigger("Jump");
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
        anim.SetBool("IsGrounded", isGrounded);
        anim.SetFloat("VerticalVelocity", rb.linearVelocity.y);
    }

    private void FixedUpdate()
    {
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
}
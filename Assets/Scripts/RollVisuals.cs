using UnityEngine;

public class RollVisuals : MonoBehaviour
{
    public Transform rootTransform;
    public float radius = 0.5f;
    public float airRotationSpeed = 20f;
    private Vector3 lastPosition;
    private Animator anim;

    void Start()
    {
        lastPosition = rootTransform.position;
        anim = rootTransform.GetComponent<Animator>();
    }

    void Update()
    {
        Vector3 currentPos = rootTransform.position;
        Vector3 movement = new Vector3(currentPos.x - lastPosition.x, 0f, currentPos.z - lastPosition.z);
        bool isGrounded = anim.GetBool("IsGrounded");

        if (isGrounded)
        {
            if (movement != Vector3.zero)
            {
                float distance = movement.magnitude;
                float angle = (distance / (2f * Mathf.PI * radius)) * 360f;
                Vector3 rotationAxis = Vector3.Cross(Vector3.up, movement.normalized);
                transform.Rotate(rotationAxis, angle, Space.World);
            }
        }
        else
        {
            if (movement != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(movement.normalized, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * airRotationSpeed);
            }
            else
            {
                Vector3 currentForward = transform.forward;
                currentForward.y = 0;
                if (currentForward != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(currentForward.normalized, Vector3.up);
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * airRotationSpeed);
                }
            }
        }

        lastPosition = rootTransform.position;
    }
}
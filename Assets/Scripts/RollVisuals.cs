using UnityEngine;

public class RollVisuals : MonoBehaviour
{
    public Transform parentTransform;
    public float radius = 0.5f;
    private Vector3 lastPosition;

    void Start()
    {
        if (parentTransform == null)
        {
            parentTransform = transform.parent;
        }
        lastPosition = parentTransform.position;
    }

    void Update()
    {
        Vector3 movement = parentTransform.position - lastPosition;

        if (movement != Vector3.zero)
        {
            float distance = movement.magnitude;
            float angle = (distance / (2f * Mathf.PI * radius)) * 360f;
            Vector3 rotationAxis = Vector3.Cross(Vector3.up, movement.normalized);
            transform.Rotate(rotationAxis, angle, Space.World);
            lastPosition = parentTransform.position;
        }
    }
}
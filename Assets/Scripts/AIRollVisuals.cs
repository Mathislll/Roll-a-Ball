using UnityEngine;
using UnityEngine.AI;

public class AIRollVisuals : MonoBehaviour
{
    public NavMeshAgent agent; 
    public float radius = 0.2f; 

    void Update()
    {
        Vector3 movement = agent.velocity * Time.deltaTime;

        // On ignore l axe Y
        movement.y = 0;

        if (movement.magnitude > 0.001f)
        {
            float distance = movement.magnitude;
            float angle = (distance / (2f * Mathf.PI * radius)) * 360f;

            Vector3 rotationAxis = Vector3.Cross(Vector3.up, movement.normalized);
            transform.Rotate(rotationAxis, angle, Space.World);
        }
    }
}
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float rotationSpeed = 100f; 
    public float xRotation = 15f; 
    public float yRotation = 30f; 
    public float zRotation = 45f; 

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(xRotation, yRotation, zRotation) * (Time.deltaTime* rotationSpeed));
    }

}
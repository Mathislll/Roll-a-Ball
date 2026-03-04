using UnityEngine;

namespace ithappy
{
    public class RotationScript : MonoBehaviour
    {
        public enum RotationAxis
        {
            X,
            Y,
            Z
        }

        public RotationAxis rotationAxis = RotationAxis.Y;
        public float rotationSpeed = 50.0f;

        private Rigidbody rb;

        void Start()
        {
            rb = GetComponent<Rigidbody>();

            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }

            rb.isKinematic = true;
        }

        void FixedUpdate()
        {
            float rotationValue = rotationSpeed * Time.fixedDeltaTime;

            Vector3 axis = Vector3.zero;
            switch (rotationAxis)
            {
                case RotationAxis.X:
                    axis = Vector3.right;
                    break;
                case RotationAxis.Y:
                    axis = Vector3.up;
                    break;
                case RotationAxis.Z:
                    axis = Vector3.forward;
                    break;
            }

            Quaternion deltaRotation = Quaternion.AngleAxis(rotationValue, axis);
            rb.MoveRotation(rb.rotation * deltaRotation);
        }
    }
}
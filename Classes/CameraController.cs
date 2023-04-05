using UnityEngine;

namespace Assets.Classes
{
    class CameraController : MonoBehaviour
    {
        public Transform anchorPoint;

        public float movementSpeed = 1.0f;
        public bool speedBoost = false;
        public bool speedSink = false;

        public float rotationSpeed = 10.0f;
        public float maxYAngle = 90.0f;
        public float minYAngle = -90.0f;

        private Vector2 currentRotation;
        private bool isRotating = false;

        void LateUpdate()
        {
            // Camera Rotation
            if (Input.GetMouseButtonDown(1))
            {
                isRotating = true;
            }
            else if (Input.GetMouseButtonUp(1))
            {
                isRotating = false;
            }

            if (isRotating)
            {
                float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
                float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

                currentRotation.x += mouseX;
                currentRotation.y -= mouseY;
                currentRotation.y = Mathf.Clamp(currentRotation.y, minYAngle, maxYAngle);

                transform.rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0.0f);
            }

            // Camera Movement
            float yAxis = 0.0f;
            Vector3 movement = Vector3.zero;

            if (Input.GetKey(KeyCode.A))
            {
                movement -= transform.right;
            }
            if (Input.GetKey(KeyCode.D))
            {
                movement += transform.right;
            }
            if (Input.GetKey(KeyCode.W))
            {
                movement += transform.forward;
            }
            if (Input.GetKey(KeyCode.S))
            {
                movement -= transform.forward;
            }
            if (Input.GetKey(KeyCode.Space))
            {
                yAxis += 1.0f;
            }
            if (Input.GetKey(KeyCode.C))
            {
                yAxis -= 1.0f;
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                speedBoost = true;
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift)) {
                speedBoost = false;
            }

            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                speedSink = true;
            }
            else if (Input.GetKeyUp(KeyCode.LeftAlt))
            {
                speedSink = false;
            }

            if (speedBoost)
            {
                movementSpeed = 1000.0f;
            }
            else if (speedSink)
            {
                movementSpeed = 10.0f;
            }
            else
            {
                movementSpeed = 100.0f;
            }

            movement.Normalize();
            transform.position += movementSpeed * Time.deltaTime * movement;
            transform.position += movementSpeed * Time.deltaTime * yAxis * Vector3.up;

        }
    }
}

using UnityEngine;

namespace Assets.Classes
{
    class CameraController : MonoBehaviour
    {
        private float movementSpeed = 1.0f;
        private bool OmegaBoost = false;
        private bool speedBoost = false;
        private bool speedSink = false;

        private float rotationSpeed = 10.0f;
        private float maxYAngle = 90.0f;
        private float minYAngle = -90.0f;

        private Vector2 currentRotation;
        private float currentRotationZ;
        private bool isRotatingNormal = false;
        private bool isRotatingScene = false;

        public ObjectSelectionManager selectionManager;

        void LateUpdate()
        {
            // Camera Rotation
            if (Input.GetMouseButtonDown(1))
            {
                isRotatingNormal = true;
            }
            else if (Input.GetMouseButtonUp(1))
            {
                isRotatingNormal = false;
            }
            // X/Y axis rotation
            if (isRotatingNormal)
            {
                float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
                float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

                currentRotation.x += mouseX;
                currentRotation.y -= mouseY;
                currentRotation.y = Mathf.Clamp(currentRotation.y, minYAngle, maxYAngle);

                transform.rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, currentRotationZ);
            }

            // Z axis rotation
            if (Input.GetMouseButtonDown(2))
            {
                isRotatingScene = true;
            }
            if (Input.GetMouseButtonUp(2))
            {
                isRotatingScene = false;
            }
            if (isRotatingScene)
            {
                float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;

                currentRotationZ += mouseX;

                transform.rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, currentRotationZ);
            }
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                currentRotationZ = 0.0f;
                currentRotation.x = 0.0f;
                currentRotation.y = 0.0f;
                transform.rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, currentRotationZ);
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


            // SPEEDOBURSTO
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                speedBoost = true;
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
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

            if (Input.GetKeyDown(KeyCode.E))
            {
                OmegaBoost = true;
            }
            else if (Input.GetKeyUp(KeyCode.E))
            {
                OmegaBoost = false;
            }
            if (speedBoost)
            {
                if (OmegaBoost)
                {
                    movementSpeed = 10000.0f;
                }
                else
                {
                    movementSpeed = 1000.0f;
                }
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

            //Selected Node/Edge telepotration
            if (Input.GetKeyDown(KeyCode.R))
            {
                Vector3 meanPosition = Vector3.zero;
                int NodeEdgeCount = 0;
                foreach (GameObject node in selectionManager.getNodes())
                {
                    meanPosition += node.transform.position;
                    NodeEdgeCount++;
                }
                foreach (GameObject edge in selectionManager.getEdges())
                {
                    meanPosition += edge.transform.position;
                    NodeEdgeCount++;
                }
                if (NodeEdgeCount > 0)
                {
                    meanPosition /= NodeEdgeCount;
                }
                transform.position = meanPosition;

            }
        }
    }
}

using Assets.Scripts;
using System.Linq;
using UnityEngine;
using static UnityEngine.Mathf;

public class ObjectIDDisplay : MonoBehaviour
{
    private float hoverTime = 1f; // The amount of time the mouse needs to hover over an object
    private float hoverTimer = 0f; // The amount of time the mouse has hovered over an object

    private bool isDisplaying = false; // Whether the GUI window is currently being displayed
    private string objectID = ""; // The ID of the object being displayed
    private string content = "";
    RaycastHit lastHit;

    void Update()
    {
        // Get the position of the mouse cursor
        Vector3 mousePosition = Input.mousePosition;

        // Create a ray from the camera through the mouse cursor position
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        // Detect the object that the ray intersects with
        if (Physics.Raycast(ray, out lastHit))
        {
            // Check if the object has an ID
            ObjectID objectIDComponent = lastHit.collider.GetComponent<ObjectID>();
            if (objectIDComponent != null)
            {
                // Start or continue the hover timer
                hoverTimer += Time.deltaTime;

                // If the mouse has hovered over the object for the required time, display the ID
                if (hoverTimer >= hoverTime)
                {
                    objectID = objectIDComponent._id;
                    content = "ID: " + objectID + "\n" +
                "x: " + lastHit.transform.position.x.ToString("F0") + "\n" +
                "y: " + lastHit.transform.position.y.ToString("F0") + "\n" +
                "z: " + lastHit.transform.position.z.ToString("F0");
                    isDisplaying = true;
                }
            }
            else
            {
                hoverTimer = 0f;
                isDisplaying = false;
            }
        }
        else
        {
            hoverTimer = 0f;
            isDisplaying = false;
        }
    }
    void OnGUI()
    {
        if (isDisplaying)
        {
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontSize = 15;
            GUIContent labelContent = new GUIContent(content);
            Vector2 labelSize = labelStyle.CalcSize(labelContent);
            Rect windowRect = new Rect(
                Input.mousePosition.x + 15f,
                Screen.height - Input.mousePosition.y,
                labelSize.x + 20f,
                labelSize.y + 20f);
            windowRect = GUI.Window(0, windowRect, DrawWindow, labelContent, labelStyle);
        }
    }

    void DrawWindow(int windowID)
    {;
        GUI.DragWindow();
    }

}
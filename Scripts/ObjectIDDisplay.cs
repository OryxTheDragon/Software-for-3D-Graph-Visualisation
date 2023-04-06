using Assets.Scripts;
using UnityEngine;

public class ObjectIDDisplay : MonoBehaviour
{
    private float hoverTime = 1f; // The amount of time the mouse needs to hover over an object
    private float hoverTimer = 0f; // The amount of time the mouse has hovered over an object

    private bool isDisplaying = false; // Whether the GUI window is currently being displayed
    private string objectID = ""; // The ID of the object being displayed

    void Update()
    {
        // Get the position of the mouse cursor
        Vector3 mousePosition = Input.mousePosition;

        // Create a ray from the camera through the mouse cursor position
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        // Detect the object that the ray intersects with
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            // Check if the object has an ID

            ObjectID objectIDComponent = hit.collider.GetComponent<ObjectID>();
            if (objectIDComponent != null)
            {
                // Start or continue the hover timer
                hoverTimer += Time.deltaTime;

                // If the mouse has hovered over the object for the required time, display the ID
                if (hoverTimer >= hoverTime)
                {
                    objectID = objectIDComponent._id;
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
        // Define the size and position of the window
        Rect windowRect = new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, 100, 50);
        
        // Call the method to draw the window
        windowRect = GUI.Window(0, windowRect, DrawWindow, "");
    }
}

void DrawWindow(int windowID)
{
    // Set the label style to red text
    GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
    labelStyle.normal.textColor = Color.red;
    Debug.Log("Label color: " + labelStyle.normal.textColor); // Check the color value

    // Set the label properties
    labelStyle.fontSize = 20;
    labelStyle.alignment = TextAnchor.MiddleCenter;

    // Draw the label with the object ID
    GUI.Label(new Rect(0, 0, 100, 50), objectID, labelStyle);

    // Make the window draggable
    GUI.DragWindow();
}

}
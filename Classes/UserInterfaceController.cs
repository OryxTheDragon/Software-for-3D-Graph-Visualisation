using UnityEngine;
using UnityEngine.UI;

public class UserInterfaceController : MonoBehaviour
{
    public Canvas canvas;

    private bool canvasEnabled;

    public ScrollRect nodeScrollView;
    public ScrollRect edgeScrollView;

    private bool isNodeScrollViewActive = true;

    private void Start()
    {
        canvasEnabled = true;
        canvas.enabled = canvasEnabled;

        // Enable NodeScrollView and disable EdgeScrollView on startup
        nodeScrollView.gameObject.SetActive(true);
        edgeScrollView.gameObject.SetActive(false);
    }

    private void Update()
    {
        // Toggle between NodeScrollView and EdgeScrollView when TAB is pressed
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            isNodeScrollViewActive = !isNodeScrollViewActive;
            nodeScrollView.gameObject.SetActive(isNodeScrollViewActive);
            edgeScrollView.gameObject.SetActive(!isNodeScrollViewActive);
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            canvasEnabled = !canvasEnabled;
            canvas.enabled = canvasEnabled;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleCanvas : MonoBehaviour
{
    public Canvas canvas;
    public ScrollRect scrollNodes;
    public ScrollRect scrollEdges;

    private bool canvasEnabled;
    private bool NodesEnabled;

    private void Start()
    {
        canvasEnabled = true;
        NodesEnabled = true;
        scrollNodes.enabled = true;
        scrollEdges.enabled = false;
        canvas.enabled = canvasEnabled;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            canvasEnabled = !canvasEnabled;
            canvas.enabled = canvasEnabled;
        }
        if (Input.GetKeyDown(KeyCode.LeftAlt) && canvasEnabled)
        {
            NodesEnabled = !NodesEnabled;
            scrollNodes.enabled = NodesEnabled;
            scrollEdges.enabled = !NodesEnabled;
        }
    }
}

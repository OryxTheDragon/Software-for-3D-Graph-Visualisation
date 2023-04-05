using System.Collections.Generic;
using UnityEngine;

namespace Assets.Classes
{
    public class ObjectSelectionManager
    {
        private List<GameObject> nodes = new List<GameObject>();
        private List<GameObject> edges = new List<GameObject>();
        private float intensity = 2.0f; // Increase for extra shiny selected nodes

        public ObjectSelectionManager(int field_width)
        {
        }

        public void SelectObject(GameObject selectedObject)
        {
            if (selectedObject.CompareTag("Vertex"))
            {
                if (!nodes.Contains(selectedObject))
                {
                    nodes.Add(selectedObject);
                    HighlightObject(selectedObject);
                }
                else
                {
                    nodes.Remove(selectedObject);
                    ResetObject(selectedObject);
                }
            }
            else if (selectedObject.CompareTag("Connector"))
            {
                if (!edges.Contains(selectedObject))
                {
                    edges.Add(selectedObject);
                    HighlightObject(selectedObject);
                }
                else
                {
                    edges.Remove(selectedObject);
                    ResetObject(selectedObject);
                }
            }
        }

        public void DeselectAll()
        {
            foreach (GameObject node in nodes)
            {
                ResetObject(node);
            }
            nodes.Clear();

            foreach (GameObject edge in edges)
            {
                ResetObject(edge);
            }
            edges.Clear();
        }

        private void HighlightObject(GameObject obj)
        {
            if (obj.CompareTag("Vertex"))
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                renderer.material.EnableKeyword("_EMISSION");
                renderer.material.SetColor("_EmissionColor", intensity * renderer.material.color);
            }
            else if (obj.CompareTag("Connector"))
            {
                for (int i = 0; i < obj.transform.childCount; i++)
                {
                    Renderer connectorRenderer = obj.transform.GetChild(i).GetComponent<Renderer>();
                    connectorRenderer.material.EnableKeyword("_EMISSION");
                    connectorRenderer.material.SetColor("_EmissionColor", intensity * connectorRenderer.material.color);
                }
            }
        }

        private void ResetObject(GameObject obj)
        {
            if (obj.CompareTag("Vertex"))
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                renderer.material.DisableKeyword("_EMISSION");
            }
            else if (obj.CompareTag("Connector"))
            {
                for (int i = 0; i < obj.transform.childCount; i++)
                {
                    Renderer connectorRenderer = obj.transform.GetChild(i).GetComponent<Renderer>();
                    connectorRenderer.material.DisableKeyword("_EMISSION");
                }
            }
        }
    }
}

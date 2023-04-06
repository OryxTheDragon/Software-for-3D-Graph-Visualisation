using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Classes
{
    public class ObjectSelectionManager
    {
        private readonly List<GameObject> nodes;
        private readonly List<GameObject> edges;
        private readonly float intensity = 4.0f;

        public ObjectSelectionManager()
        {
            nodes = new List<GameObject>();
            edges = new List<GameObject>();
        }

        public void selectObject(GameObject selectedObject, GameObject ListCell)
        {
            if (selectedObject.CompareTag("Vertex"))
            {
                if (!nodes.Contains(selectedObject))
                {
                    nodes.Add(selectedObject);
                    highlightObject(selectedObject);
                    ListCell.GetComponent<Button>().GetComponent<Image>().color = new Color(0.3f, 0.8f, 0.4f, 1f);
                }
                else
                {
                    nodes.Remove(selectedObject);
                    resetObject(selectedObject);
                    ListCell.GetComponent<Button>().GetComponent<Image>().color = Color.white;
                }
            }
            else if (selectedObject.CompareTag("Connector"))
            {
                if (!edges.Contains(selectedObject))
                {
                    edges.Add(selectedObject);
                    highlightObject(selectedObject);
                    ListCell.GetComponent<Button>().GetComponent<Image>().color = new Color(0.3f, 0.8f, 0.4f, 1f);
                }
                else
                {
                    edges.Remove(selectedObject);
                    resetObject(selectedObject);
                    ListCell.GetComponent<Button>().GetComponent<Image>().color = Color.white;
                }
            }
        }

        public void deselectAll()
        {
            foreach (GameObject node in nodes)
            {
                resetObject(node);
            }
            nodes.Clear();

            foreach (GameObject edge in edges)
            {
                resetObject(edge);
            }
            edges.Clear();
        }

        private void highlightObject(GameObject obj)
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

        private void resetObject(GameObject obj)
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

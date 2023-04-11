using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Classes
{
    public class ObjectSelectionManager
    {
        private readonly List<GameObject> selectedNodes;
        private readonly List<GameObject> selectedEdges;
        private readonly float intensity = 100.0f;

        public ObjectSelectionManager()
        {
            selectedNodes = new List<GameObject>();
            selectedEdges = new List<GameObject>();
        }

        public List<GameObject> getNodes() { return selectedNodes; }
        public List<GameObject> getEdges() { return selectedEdges; }

        public void selectObject(GameObject selectedObject, GameObject ListCell)
        {
            if (selectedObject.CompareTag("Vertex"))
            {
                if (!selectedNodes.Contains(selectedObject))
                {
                    selectedNodes.Add(selectedObject);
                    highlightObject(selectedObject);
                    ListCell.GetComponent<Button>().GetComponent<Image>().color = new Color(0.3f, 0.8f, 0.4f, 1f);
                }
                else
                {
                    selectedNodes.Remove(selectedObject);
                    resetObject(selectedObject);
                    ListCell.GetComponent<Button>().GetComponent<Image>().color = Color.white;
                }
            }
            else if (selectedObject.CompareTag("Connector"))
            {
                if (!selectedEdges.Contains(selectedObject))
                {
                    selectedEdges.Add(selectedObject);
                    highlightObject(selectedObject);
                    ListCell.GetComponent<Button>().GetComponent<Image>().color = new Color(0.3f, 0.8f, 0.4f, 1f);
                }
                else
                {
                    selectedEdges.Remove(selectedObject);
                    resetObject(selectedObject);
                    ListCell.GetComponent<Button>().GetComponent<Image>().color = Color.white;
                }
            }
        }

        public void deselectAll()
        {
            foreach (GameObject node in selectedNodes)
            {
                resetObject(node);
            }
            selectedNodes.Clear();

            foreach (GameObject edge in selectedEdges)
            {
                resetObject(edge);
            }
            selectedEdges.Clear();
        }

        private void highlightObject(GameObject obj)
        {
            if (obj.CompareTag("Vertex"))
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                Material material = renderer.material;
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", material.color * intensity);

            }
            else if (obj.CompareTag("Connector"))
            {
                for (int i = 0; i < obj.transform.childCount; i++)
                {
                    Renderer connectorRenderer = obj.transform.GetChild(i).GetComponent<Renderer>();
                    connectorRenderer.material.EnableKeyword("_EMISSION");
                    connectorRenderer.material.SetColor("_EmissionColor", connectorRenderer.material.color * intensity);
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

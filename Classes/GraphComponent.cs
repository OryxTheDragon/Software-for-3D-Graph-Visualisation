using Assets.Classes.DataObjects;
using Assets.Classes.DataStructures;
using UnityEngine;
using static UnityEngine.Mathf;

[ExecuteInEditMode]
public class GraphComponent : MonoBehaviour
{
    public GameObject graphParent;
    public GameObject vertexModel;
    public GameObject edgeModel;

    private Graph graph;

    void Start()
    {
        clearGraph();

        graph = new Graph();
        graph.loadGraphFromCSV("GeneratedDataBig.csv", false);
        drawGraph();
    }


    void drawGraph()
    {
        if (graph == null)
        {
            Start();
        }
        int nodeCounter = 0;
        int edgeCounter = 0;

        foreach (Node node in graph.getNodes())
        {
            
            GameObject newVertex = Instantiate(vertexModel, node.getPosition(), Quaternion.identity,graphParent.transform);
            newVertex.GetComponent<Renderer>().sharedMaterial.color = node.getNodeColor();
            newVertex.gameObject.name = "Node " + nodeCounter++;
        }
        foreach (Edge edge in graph.getEdges())
        {
            float x = (edge.getEndNode().getPosition().x - edge.getStartNode().getPosition().x);
            float y = (edge.getEndNode().getPosition().y - edge.getStartNode().getPosition().y);
            float z = (edge.getEndNode().getPosition().z - edge.getStartNode().getPosition().z);
            float length = Sqrt((x * x) + (y * y) + (z * z)) / 2;
            Vector3 scale = new Vector3(edgeModel.transform.localScale.x, length, edgeModel.transform.localScale.z);
            Vector3 position = ((edge.getStartNode().getPosition() + edge.getEndNode().getPosition()) / 2);
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, edge.getEndNode().getPosition() - edge.getStartNode().getPosition());
            GameObject connector = Instantiate(edgeModel, position, rotation, graphParent.transform);
            connector.transform.localScale = scale;
            connector.GetComponent<Renderer>().sharedMaterial.color = edge.getEdgeColor();
            connector.gameObject.name = "Edge " + edgeCounter++;
        }
    }

    void clearGraph() {
        foreach (Transform obj in graphParent.transform)
        {
            if (obj.gameObject.tag == "Vertex" || obj.gameObject.tag == "Connector")
            {
                DestroyImmediate(obj.gameObject);
                DestroyImmediate(obj);

            }
        }
    }
}

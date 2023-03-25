using Assets.Classes.DataObjects;
using Assets.Classes.DataStructures;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.Mathf;

[ExecuteInEditMode]
public class GraphComponent : MonoBehaviour
{
    public GameObject graphParent;
    public GameObject baseVertexModel;
    public GameObject baseEdgeModel;

    public GameObject omniDirectionalConnector;
    public GameObject startToEndConnector;
    public GameObject endToStartConnector;

    public Canvas canvas;

    private Graph graph;

    public TextMeshProUGUI fileName;
    public TextMeshProUGUI TextItem;

    public ScrollRect NodeViewPort;
    public ScrollRect EdgeViewPort;



    void Start()
    {
        if (Application.isPlaying)
        {
            graph = new Graph();
        }
    }


    public void instantiateGraph()
    {
        foreach (Node node in graph.getNodes())
        {

            GameObject newVertex = Instantiate(baseVertexModel, node.getPosition(), Quaternion.identity,graphParent.transform);

            TextMeshProUGUI nodeTextItem = Instantiate(TextItem, NodeViewPort.transform);
            nodeTextItem.name = "Node: " + node.getNodeId();
        }
        foreach (Edge edge in graph.getEdges())
        {
            float x = (edge.getEndNode().getPosition().x - edge.getStartNode().getPosition().x);
            float y = (edge.getEndNode().getPosition().y - edge.getStartNode().getPosition().y);
            float z = (edge.getEndNode().getPosition().z - edge.getStartNode().getPosition().z);
            float length = Sqrt((x * x) + (y * y) + (z * z)) / 4;
            Vector3 scale = new Vector3(baseEdgeModel.transform.localScale.x, length, baseEdgeModel.transform.localScale.z);
            Vector3 position = ((edge.getStartNode().getPosition() + edge.getEndNode().getPosition()) / 2);
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, edge.getEndNode().getPosition() - edge.getStartNode().getPosition());
            GameObject connector;

            if (edge.getDirection() == Direction.Omni_Directional)
            {
                connector = GameObject.Instantiate(omniDirectionalConnector, position, rotation, graphParent.transform);
            }
            else if (edge.getDirection() == Direction.Start_To_End)
            {
                connector = GameObject.Instantiate(startToEndConnector, position, rotation, graphParent.transform);
            }
            else if (edge.getDirection() == Direction.End_To_Start)
            {
                connector = GameObject.Instantiate(endToStartConnector, position, rotation, graphParent.transform);
            }
            else
            {
                connector = GameObject.Instantiate(baseEdgeModel, position, rotation, graphParent.transform);
            }
            connector.transform.localScale = scale;
            TextMeshProUGUI edgeTextItem = Instantiate(TextItem, EdgeViewPort.transform);
            edgeTextItem.text = edge.getEdgeId();
            edgeTextItem.name = "Edge: " + edge.getEdgeId();
        }
    }

    public void selectFile()
    {
        string[] fileExtensions = { "csv" };
        string path = EditorUtility.OpenFilePanel("Select a File", "", string.Join(",", fileExtensions));
        if (File.Exists(path))
        {
            graph.loadGraphFromCSV(path, false);
            fileName.text = Path.GetFileName(path);
        }
    }

}

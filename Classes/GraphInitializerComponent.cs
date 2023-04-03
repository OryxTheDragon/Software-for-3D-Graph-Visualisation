using Assets.Classes.DataObjects;
using Assets.Classes.DataStructures;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

using static GraphInitializerComponent;
using static UnityEngine.Mathf;

[ExecuteInEditMode]
public class GraphInitializerComponent : MonoBehaviour
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

    public GameObject[] tabPages;
    public Button[] tabButtons;

    public GameObject listCellPrefab;
    public GameObject NodeViewPort;
    public GameObject EdgeViewPort;

    void Start()
    {
        if (Application.isPlaying)
        {
            graph = new Graph();
        }
    }

    public void instantiateGraph()
    {
        int counter = 1;
        foreach (Node node in graph.getNodes())
        {

            GameObject newVertex = Instantiate(baseVertexModel, node.getPosition(), Quaternion.identity,graphParent.transform);
            GameObject listCell = Instantiate(listCellPrefab, NodeViewPort.transform);
            TextMeshProUGUI[] textComponents = listCell.GetComponentsInChildren<TextMeshProUGUI>();
            textComponents[0].text = "" + counter++;
            textComponents[1].text = node.getNodeId();
        }
        counter = 1;
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
                connector = Instantiate(omniDirectionalConnector, position, rotation, graphParent.transform);
            }
            else if (edge.getDirection() == Direction.Start_To_End)
            {
                connector = Instantiate(startToEndConnector, position, rotation, graphParent.transform);
            }
            else if (edge.getDirection() == Direction.End_To_Start)
            {
                connector = Instantiate(endToStartConnector, position, rotation, graphParent.transform);
            }
            else
            {
                connector = Instantiate(baseEdgeModel, position, rotation, graphParent.transform);
            }
            connector.transform.localScale = scale;
            GameObject listCell = Instantiate(listCellPrefab, EdgeViewPort.transform);
            TextMeshProUGUI[] textComponents = listCell.GetComponentsInChildren<TextMeshProUGUI>();
            textComponents[0].text = "" + counter++;
            textComponents[1].text = edge.getEdgeId();
        }
    }

    public void selectFileForImport()
    {
        string[] fileExtensions = { "csv" };
        string path = EditorUtility.OpenFilePanel("Select a File", "", string.Join(",", fileExtensions));
        if (File.Exists(path))
        {
            graph.loadGraphFromCSV(path, false);
            fileName.text = Path.GetFileName(path);
            fileName.color = new Color(0,0.5f,0,1f);
        }
    }

    public void ExportToCSV()
    {
        string[] fileExtensions = { "csv" };
        string delimiter = ";";

        string defaultPath = Application.dataPath;
        string defaultFileName = "Exported Data";
        string filePath = EditorUtility.SaveFilePanel("Save File", defaultPath, defaultFileName, fileExtensions[0]);

        StreamWriter exportedFile = new StreamWriter(filePath);

        foreach (Node node in graph.getNodes())
        {
            exportedFile.WriteLine("N" + delimiter + node.getNodeId() + delimiter + node.getPosition().x + delimiter + node.getPosition().y + delimiter + node.getPosition().z);
        }
        foreach (Edge edge in graph.getEdges())
        {
            exportedFile.WriteLine("E" + delimiter + edge.getEdgeId() + delimiter + edge.getStartNode().getNodeId() + delimiter + edge.getEndNode().getNodeId() + delimiter + (int) edge.getDirection());
        }
        exportedFile.Close();
    }
}

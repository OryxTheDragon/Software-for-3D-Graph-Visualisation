using Assets.Classes;
using Assets.Classes.DataObjects;
using Assets.Classes.DataStructures;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Mathf;

[ExecuteInEditMode]
public class GraphManager : MonoBehaviour
{
    public GameObject graphParent;
    public GameObject baseVertexModel;
    public GameObject baseEdgeModel;

    private int fieldSize = 500;


    public GameObject DirectedConnector;
    public GameObject UndirectedConnector;

    public Material highlightedMaterial;

    public Canvas canvas;

    private Graph graph;
    private Treap<string, GameObject> NodeGameObjects;
    private Treap<string, GameObject> EdgeGameObjects;

    public TextMeshProUGUI fileName;
    public TextMeshProUGUI TextItem;

    public GameObject[] tabPages;
    public Button[] tabButtons;

    public GameObject listCellPrefab;
    public GameObject NodeViewPort;
    public GameObject EdgeViewPort;

    public List<GameObject> selectedNodes;
    public List<GameObject> selectedEdges;

    private ObjectSelectionManager selectionManager;

    void Start()
    {
        if (Application.isPlaying)
        {
            graph = new Graph();
            selectedNodes = new List<GameObject>();
            selectedEdges = new List<GameObject>();
            selectionManager = new ObjectSelectionManager(fieldSize);
        }
    }

    public void instantiateGraph()
    {
        NodeGameObjects = new Treap<string, GameObject>();
        EdgeGameObjects = new Treap<string, GameObject>();

        int counter = 1;
        foreach (Node node in graph.getNodes())
        {
            GameObject newVertex = Instantiate(baseVertexModel, node.getPosition(), Quaternion.identity, graphParent.transform);
            GameObject listCell = Instantiate(listCellPrefab, NodeViewPort.transform);
            
            NodeGameObjects.Insert(node.getNodeId(), newVertex);

            TextMeshProUGUI[] textComponents = listCell.GetComponentsInChildren<TextMeshProUGUI>();

            listCell.GetComponent<Button>().onClick.AddListener(() => HighlightNodeFromUICell(node.getNodeId()));
            textComponents[0].text = "" + counter++;
            textComponents[1].text = node.getNodeId();
            Renderer renderer = newVertex.GetComponent<Renderer>();
            Vector3 pos = transform.position;
            renderer.material.color =
                new Color(
                    Abs(pos.x) / fieldSize,
                    Abs(pos.y) / fieldSize,
                    Abs(pos.z) / fieldSize
                );
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
            Quaternion rotation = Quaternion.FromToRotation(
                Vector3.up,
                edge.getEndNode().getPosition() - edge.getStartNode().getPosition());

            GameObject connector;
            if (edge.getDirection() == Direction.Undirected)
            {
                connector = Instantiate(UndirectedConnector, position, rotation, graphParent.transform);
            }
            else if (edge.getDirection() == Direction.Directed)
            {
                connector = Instantiate(DirectedConnector, position, rotation, graphParent.transform);
            }
            else
            {
                connector = Instantiate(baseEdgeModel, position, rotation, graphParent.transform);
            }
            connector.transform.localScale = scale;

            EdgeGameObjects.Insert(edge.getEdgeId(), connector);
            GameObject listCell = Instantiate(listCellPrefab, EdgeViewPort.transform);
            TextMeshProUGUI[] textComponents = listCell.GetComponentsInChildren<TextMeshProUGUI>();
            listCell.GetComponent<Button>().onClick.AddListener(() => HighlightEdgeFromUICell(edge.getEdgeId()));
            textComponents[0].text = "" + counter++;
            textComponents[1].text = edge.getEdgeId();
        }
    }

    private void HighlightEdgeFromUICell(string edge_id)
    {
        GameObject selectedEdge = EdgeGameObjects.getTNode(edge_id);
        selectionManager.SelectObject(selectedEdge);

    }

    private void HighlightNodeFromUICell(string node_id)
    {
        GameObject selectedNode = NodeGameObjects.getTNode(node_id);
        selectionManager.SelectObject(selectedNode);
    }

    public void selectFileForImport()
    {
        string[] fileExtensions = { "csv" };
        string path = EditorUtility.OpenFilePanel("Select a File", "", string.Join(",", fileExtensions));
        if (File.Exists(path))
        {
            graph.loadGraphFromCSV(path, false);
            fileName.text = Path.GetFileName(path);
            fileName.color = new Color(0, 0.5f, 0, 1f);
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
            exportedFile.WriteLine("E" + delimiter + edge.getEdgeId() + delimiter + edge.getStartNode().getNodeId() + delimiter + edge.getEndNode().getNodeId() + delimiter + (int)edge.getDirection());
        }
        exportedFile.Close();
    }
}

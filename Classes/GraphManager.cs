using Assets.Classes;
using Assets.Classes.DataObjects;
using Assets.Classes.DataStructures;
using Assets.Scripts;
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
    public GameObject quadPrefab;

    public float fieldSize = 500.0f;

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
            selectionManager = new ObjectSelectionManager();
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
            ObjectID newNodeId = newVertex.GetComponent<ObjectID>();
            newNodeId._id = node.getNodeId();
            GameObject listCell = Instantiate(listCellPrefab, NodeViewPort.transform);

            NodeGameObjects.Insert(node.getNodeId(), newVertex);

            TextMeshProUGUI[] textComponents = listCell.GetComponentsInChildren<TextMeshProUGUI>();

            listCell.GetComponent<Button>().onClick.AddListener(() => highlightNodeFromUICell(node.getNodeId()));
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
            Vector3 scale = new(baseEdgeModel.transform.localScale.x, length, baseEdgeModel.transform.localScale.z);
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
            ObjectID newEdgeId = connector.GetComponent<ObjectID>();
            newEdgeId._id = edge.getEdgeId();

            GameObject listCell = Instantiate(listCellPrefab, EdgeViewPort.transform);
            TextMeshProUGUI[] textComponents = listCell.GetComponentsInChildren<TextMeshProUGUI>();
            listCell.GetComponent<Button>().onClick.AddListener(() => highlightEdgeFromUICell(edge.getEdgeId()));
            textComponents[0].text = "" + counter++;
            textComponents[1].text = edge.getEdgeId();
        }
    }

    private void highlightEdgeFromUICell(string edge_id)
    {
        GameObject selectedEdge = EdgeGameObjects.getTNode(edge_id);
        selectionManager.selectObject(selectedEdge);
    }

    private void highlightNodeFromUICell(string node_id)
    {
        GameObject selectedNode = NodeGameObjects.getTNode(node_id);
        selectionManager.selectObject(selectedNode);
    }

    public void selectFileForImport()
    {
        string[] fileExtensions = { "csv" };
        string path = EditorUtility.OpenFilePanel("Select a File", "", string.Join(",", fileExtensions));
        if (File.Exists(path))
        {
            graph.loadGraphFromCSV(path, false);
            fieldSize = graph.getScale();
            fileName.text = Path.GetFileName(path);
            fileName.color = new Color(0, 0.5f, 0, 1f);
        }
    }

    public void exportToCSV()
    {
        string[] fileExtensions = { "csv" };
        string delimiter = ";";

        string defaultPath = Application.dataPath;
        string defaultFileName = "Exported Data";
        string filePath = EditorUtility.SaveFilePanel("Save File", defaultPath, defaultFileName, fileExtensions[0]);

        StreamWriter exportedFile = new(filePath);

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

    public void generateCube()
    {
        List<GameObject> quadList = new List<GameObject>();
        GameObject North = Instantiate(quadPrefab, new Vector3(1f * fieldSize, 0, 0), Quaternion.Euler(0, 90.0f, 0), transform.GetChild(1));
        GameObject West = Instantiate(quadPrefab, new Vector3(0, 0, 1f * fieldSize), Quaternion.Euler(0, 0, 0), transform.GetChild(1));
        GameObject East = Instantiate(quadPrefab, new Vector3(0, 0, -1f * fieldSize), Quaternion.Euler(0, 180.0f, 0), transform.GetChild(1));
        GameObject South = Instantiate(quadPrefab, new Vector3(-1f * fieldSize, 0, 0), Quaternion.Euler(0, -90.0f, 0), transform.GetChild(1));
        GameObject Top = Instantiate(quadPrefab, new Vector3(0, 1f * fieldSize, 0), Quaternion.Euler(-90.0f, 0, 0), transform.GetChild(1));
        GameObject Bottom = Instantiate(quadPrefab, new Vector3(0, -1f * fieldSize, 0), Quaternion.Euler(90.0f, 0, 0), transform.GetChild(1));

        North.name = "NorthQuadWall";
        West.name = "WestQuadWall";
        East.name = "EastQuadWall";
        South.name = "SouthQuadWall";
        Top.name = "TopQuadWall";
        Bottom.name = "BottomQuadWall";

        quadList.Add(North);
        quadList.Add(West);
        quadList.Add(East);
        quadList.Add(South);
        quadList.Add(Top);
        quadList.Add(Bottom);

        foreach (GameObject quad in quadList)
        {
            quad.transform.parent = transform;
            quad.transform.localScale = new Vector3(2 * fieldSize, 2 * fieldSize, 1);
        }
    }
}

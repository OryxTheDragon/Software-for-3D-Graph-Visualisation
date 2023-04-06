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
    //Parent:
    public GameObject graphParent;

    //Prefabs:
    public GameObject baseVertexModel;
    public GameObject baseEdgeModel;
    public GameObject quadPrefab;
    public GameObject DirectedConnector;
    public GameObject UndirectedConnector;
    public TextMeshProUGUI TextItem;

    //Data Structures:
    private Graph graph;
    private Treap<string, GameObject> NodeGameObjects;
    private Treap<string, GameObject> EdgeGameObjects;

    //Necessary Data:
    public float fieldSize = 500.0f;
    public int OptimizationLimit = 10000;
    private readonly bool overrideLoadedData = true;
    List<GameObject> quadList = new();
    private int numOfNodes;
    private int numOfEdges;


    //UI References:
    // Data Management Resources:
    public GameObject[] tabPages;
    public Button[] tabButtons;
    public GameObject DataManagementPanel;

    // Data Lab Resources:
    public GameObject listCellPrefab;
    public GameObject NodeViewPort;
    public GameObject EdgeViewPort;

    private ObjectSelectionManager selectionManager;

    void Start()
    {
        if (Application.isPlaying)
        {
            graph = new Graph();
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
            createNodeGameObject(node, counter);
            counter++;
        }
        counter = 1;
        foreach (Edge edge in graph.getEdges())
        {
            createEdgeGameObject(edge, counter);
            counter++;
        }

    }

    private void createNodeGameObject(Node node, int counter)
    {
        GameObject newVertex = Instantiate(baseVertexModel, node.getPosition(), Quaternion.identity, graphParent.transform);
        ObjectID newNodeId = newVertex.GetComponent<ObjectID>();
        newNodeId._id = node.getNodeId();

        NodeGameObjects.insert(node.getNodeId(), newVertex);
        if (numOfNodes < OptimizationLimit)
        {
            CreateNodeListEntry(node.getNodeId(), counter);
        }
    }

    private void createEdgeGameObject(Edge edge, int counter)
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

        EdgeGameObjects.insert(edge.getEdgeId(), connector);
        ObjectID newEdgeId = connector.GetComponent<ObjectID>();
        newEdgeId._id = edge.getEdgeId();
        if (numOfEdges <= OptimizationLimit)
        {
            CreateEdgeListEntry(edge.getEdgeId(), counter);
        }
    }

    private void CreateNodeListEntry(string node_id, int counter)
    {
        GameObject listCell = Instantiate(listCellPrefab, NodeViewPort.transform);
        TextMeshProUGUI[] textComponents = listCell.GetComponentsInChildren<TextMeshProUGUI>();
        listCell.GetComponent<Button>().onClick.AddListener(() => highlightNodeFromUICell(node_id, listCell));
        textComponents[0].text = "" + counter;
        textComponents[1].text = node_id;
    }

    private void CreateEdgeListEntry(string edge_id, int counter)
    {
        GameObject listCell = Instantiate(listCellPrefab, EdgeViewPort.transform);
        TextMeshProUGUI[] textComponents = listCell.GetComponentsInChildren<TextMeshProUGUI>();
        listCell.GetComponent<Button>().onClick.AddListener(() =>
        {
            highlightEdgeFromUICell(edge_id, listCell);
        });
        textComponents[0].text = "" + counter++;
        textComponents[1].text = edge_id;
    }

    private void highlightEdgeFromUICell(string edge_id, GameObject cell)
    {
        GameObject selectedEdge = EdgeGameObjects.getTNode(edge_id);
        selectionManager.selectObject(selectedEdge, cell);
    }

    private void highlightNodeFromUICell(string node_id, GameObject cell)
    {
        GameObject selectedNode = NodeGameObjects.getTNode(node_id);
        selectionManager.selectObject(selectedNode, cell);
    }

    public void selectFileForImport()
    {
        if (overrideLoadedData)
        {
            if (NodeGameObjects != null || EdgeGameObjects != null)
            {
                foreach (GameObject quad in quadList)
                {
                    quad.SetActive(false);
                }
                clearTheField();
                NodeGameObjects = new Treap<string, GameObject>();
                EdgeGameObjects = new Treap<string, GameObject>();
                graph = new Graph();
                selectionManager = new ObjectSelectionManager();
            }
        }
        string[] fileExtensions = { "csv" };
        string path = EditorUtility.OpenFilePanel("Select a File", "", string.Join(",", fileExtensions));
        if (File.Exists(path))
        {
            graph.loadGraphFromCSV(path, false);
            fieldSize = graph.getScale();
            numOfNodes = graph.getNumOfNodes();
            numOfEdges = graph.getNumOfEdges();
            DataManagementPanel.transform.GetChild(0).transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = Path.GetFileName(path);
            DataManagementPanel.transform.GetChild(0).transform.GetChild(4).GetComponent<TextMeshProUGUI>().color = new Color(0, 0.5f, 0, 1f);
            DataManagementPanel.transform.GetChild(0).transform.GetChild(7).GetComponent<TextMeshProUGUI>().text = "" + graph.getNumOfNodes();
            DataManagementPanel.transform.GetChild(0).transform.GetChild(8).GetComponent<TextMeshProUGUI>().text = "" + graph.getNumOfEdges();
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
        if (quadList.Count == 0)
        {
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
        else
        {
            quadList[0].transform.position = new Vector3(1f * fieldSize, 0, 0);
            quadList[1].transform.position = new Vector3(0, 0, 1f * fieldSize);
            quadList[2].transform.position = new Vector3(0, 0, -1f * fieldSize);
            quadList[3].transform.position = new Vector3(-1f * fieldSize, 0, 0);
            quadList[4].transform.position = new Vector3(0, 1f * fieldSize, 0);
            quadList[5].transform.position = new Vector3(0, -1f * fieldSize, 0);
            foreach (GameObject quad in quadList)
            {
                quad.SetActive(true);
                quad.transform.localScale = new Vector3(2 * fieldSize, 2 * fieldSize, 1);
            }
        }
    }

    public void clearTheField() {
        foreach (GameObject edge in EdgeGameObjects)
        {
            Destroy(edge);
        }
        foreach (GameObject node in NodeGameObjects)
        {
            Destroy(node);
        }
    }
}

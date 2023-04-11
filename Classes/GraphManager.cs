using Assets.Classes;
using Assets.Classes.DataObjects;
using Assets.Classes.DataStructures;
using Assets.Scripts;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using static UnityEngine.Mathf;
using System.Windows.Forms;
using Application = UnityEngine.Application;
using UnityEditor;

public class GraphManager : MonoBehaviour
{
    //Parent:
    public GameObject graphParent;

    //Prefabs:
    public GameObject baseVertexModel;
    public GameObject baseEdgeModel;
    public GameObject quadPrefab;
    public GameObject DirectedConnectorPrefab;
    public GameObject UndirectedConnectorPrefab;
    public TextMeshProUGUI TextItem;

    //Data Structures:
    private Graph graph;
    private Treap<string, GameObject> NodeGameObjects;
    private Treap<string, GameObject> EdgeGameObjects;

    //Necessary Data:
    private float fieldSize = 500.0f;
    private int OptimizationLimit = 10000;
    private readonly bool overrideLoadedData = true;
    private List<GameObject> quadList = new();
    private int numOfNodes;
    private int numOfEdges;

    //Data Manager:
    private ObjectSelectionManager selectionManager;

    //UI References:
    public GameObject canvas;
    // Data Management Resources:
    public GameObject[] tabPages;
    public Button[] tabButtons;

    // Data Lab Resources:
    public GameObject listCellPrefab;
    public GameObject NodeViewList;
    public GameObject EdgeViewList;

    // Physics and Statistics Resources:
    private SpringJoint[] SpringJointArray;
    public Slider[] sliders;

    void Start()
    {
        if (Application.isPlaying)
        {
            Physics.autoSimulation = false;
            graph = new Graph();
            selectionManager = new ObjectSelectionManager();
            Camera.main.GetComponent<CameraController>().selectionManager = selectionManager;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            setQuitPanelActive(true);
        }
    }

    public void setQuitPanelActive(bool on)
    {
        canvas.transform.GetChild(5).gameObject.SetActive(on);
    }

    public void quitApplication()
    {
    #if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
    #endif
        Application.Quit();
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
        newNodeId.scale = fieldSize;
        newVertex.transform.localScale = newVertex.transform.localScale * Min(15, fieldSize, numOfNodes);
        NodeGameObjects.insert(node.getNodeId(), newVertex);
        if (numOfNodes < OptimizationLimit)
        {
            CreateNodeListEntry(node.getNodeId(), counter);
        }
    }

    private void createEdgeGameObject(Edge edge, int counter)
    {
        GameObject connector;
        if (edge.getDirection() == Direction.Undirected)
        {
            connector = Instantiate(UndirectedConnectorPrefab, graphParent.transform);
        }
        else if (edge.getDirection() == Direction.Directed)
        {
            connector = Instantiate(DirectedConnectorPrefab, graphParent.transform);
        }
        else
        {
            connector = Instantiate(baseEdgeModel, graphParent.transform);
        }

        connector.GetComponent<DynamicScale>().startNode = NodeGameObjects.getTNodeValue(edge.getStartNode().getNodeId()).transform;
        connector.GetComponent<DynamicScale>().endNode = NodeGameObjects.getTNodeValue(edge.getEndNode().getNodeId()).transform;
        SpringJoint springJoint = connector.GetComponent<DynamicScale>().startNode.gameObject.AddComponent<SpringJoint>();
        springJoint.connectedBody = connector.GetComponent<DynamicScale>().endNode.GetComponent<Rigidbody>();
        springJoint.enableCollision = true;
        SpringJointArray[counter - 1] = springJoint;
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
        GameObject listCell = Instantiate(listCellPrefab, NodeViewList.transform);
        TextMeshProUGUI[] textComponents = listCell.GetComponentsInChildren<TextMeshProUGUI>();
        listCell.GetComponent<Button>().onClick.AddListener(() => highlightNodeFromUICell(node_id, listCell));
        textComponents[0].text = "" + counter;
        textComponents[1].text = node_id;
    }

    private void CreateEdgeListEntry(string edge_id, int counter)
    {
        GameObject listCell = Instantiate(listCellPrefab, EdgeViewList.transform);
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
        GameObject selectedEdge = EdgeGameObjects.getTNodeValue(edge_id);
        selectionManager.selectObject(selectedEdge, cell);
    }

    private void highlightNodeFromUICell(string node_id, GameObject cell)
    {
        GameObject selectedNode = NodeGameObjects.getTNodeValue(node_id);
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
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "CSV files (*.csv)|*.csv";
        openFileDialog.FilterIndex = 0;
        openFileDialog.RestoreDirectory = true;
        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            string path = openFileDialog.FileName;
            loadGraphFromCSV(path, false);
            fieldSize = graph.getScale();
            numOfNodes = graph.getNumOfNodes();
            numOfEdges = graph.getNumOfEdges();
            if (numOfNodes >= OptimizationLimit)
            {
                NodeViewList.transform.parent.GetChild(1).gameObject.SetActive(true);
            }
            else { NodeViewList.transform.parent.GetChild(1).gameObject.SetActive(false); }
            if (numOfEdges >= OptimizationLimit)
            {
                EdgeViewList.transform.parent.GetChild(1).gameObject.SetActive(true);
            }
            else { NodeViewList.transform.parent.GetChild(1).gameObject.SetActive(false); }
            Transform DataManagementRect = canvas.transform.GetChild(1).GetChild(0).transform;
            DataManagementRect.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = Path.GetFileName(path);
            DataManagementRect.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(0, 0.5f, 0, 1f);
            DataManagementRect.GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + graph.getNumOfNodes();
            DataManagementRect.GetChild(5).GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + graph.getNumOfEdges();
            SpringJointArray = new SpringJoint[numOfEdges];
            generateCube();
            InitializeSliderMaxValues();
        }
    }


    private void InitializeSliderMaxValues()
    {
        sliders[3].maxValue = fieldSize;
        sliders[2].maxValue = fieldSize;
        sliders[0].maxValue = 1000;
        sliders[1].maxValue = 1000;
        sliders[4].maxValue = 10;
        sliders[3].transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = "" + sliders[3].maxValue;
        sliders[2].transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = "" + sliders[2].maxValue;
        sliders[0].transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = "" + sliders[0].maxValue;
        sliders[1].transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = "" + sliders[1].maxValue;
        sliders[4].transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = "" + sliders[4].maxValue;
    }

    public void exportToCSV()
    {
        string[] fileExtensions = { "csv" };
        string delimiter = ";";

        string defaultFileName = "Exported Data";
        SaveFileDialog saveFileDialog = new SaveFileDialog();
        saveFileDialog.Filter = string.Format("{0} files (*.{1})|*.{1}", "CSV", fileExtensions[0]);
        saveFileDialog.FileName = defaultFileName;

        if (saveFileDialog.ShowDialog() == DialogResult.OK)
        {
            string filePath = saveFileDialog.FileName;
            using StreamWriter exportedFile = new StreamWriter(filePath);
            foreach (Node node in graph.getNodes())
            {
                exportedFile.WriteLine("N" + delimiter + node.getNodeId() + delimiter + node.getPosition().x + delimiter + node.getPosition().y + delimiter + node.getPosition().z);
            }
            foreach (Edge edge in graph.getEdges())
            {
                exportedFile.WriteLine("E" + delimiter + edge.getEdgeId() + delimiter + edge.getStartNode().getNodeId() + delimiter + edge.getEndNode().getNodeId() + delimiter + (int)edge.getDirection());
            }
        }
    }

    public void loadGraphFromCSV(string filepath, bool overwrite)
    {
        using (StreamReader reader = new(filepath))
        {
            if (overwrite)
            {
                graph = new Graph();
            }
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] values = line.Split(';');
                if (values.Length < 2)
                {
                    Debug.LogErrorFormat("Invalid data format in line: {0}", line);
                    continue;
                }
                if (values[0].Equals("N"))
                {
                    if (values.Length != 5)
                    {
                        Debug.LogErrorFormat("Invalid data format in line: {0}", line);
                        continue;
                    }
                    string nodeId = values[1];
                    float posX, posY, posZ;
                    if (!float.TryParse(values[2], out posX) || !float.TryParse(values[3], out posY) || !float.TryParse(values[4], out posZ))
                    {
                        Debug.LogErrorFormat("Invalid data format in line: {0}", line);
                        continue;
                    }
                    graph.setScale(Max(posX, posY, posZ, graph.getScale()));
                    graph.insertNode(graph.createNode(nodeId, new Vector3(posX, posY, posZ)));
                }
                else if (values[0].Equals("E"))
                {
                    if (values.Length > 5 || values.Length < 3)
                    {
                        Debug.LogErrorFormat("Invalid data format in line: {0}", line);
                        continue;
                    }
                    else if (values.Length == 5)
                    {
                        string edgeId = values[1];
                        Node startNode = graph.getNodes().getTNodeValue(values[2]);
                        Node endNode = graph.getNodes().getTNodeValue(values[3]);
                        if (startNode == null || endNode == null)
                        {
                            Debug.LogErrorFormat("Invalid data format in line: {0}", line);
                            continue;
                        }
                        Direction direction = Direction.Undirected;
                        if (values.Length == 5 && values[4].Equals("0"))
                        {
                            direction = Direction.Undirected;
                        }
                        if (values.Length == 5 && values[4].Equals("1"))
                        {
                            direction = Direction.Directed;
                        }
                        graph.insertEdge(graph.createEdge(edgeId, startNode, endNode, direction));
                    }
                }
                else
                {
                    Debug.LogErrorFormat("Invalid data format in line: {0}", line);
                    continue;
                }
            }
        }
    }
    public void clearTheField()
    {
        selectionManager.deselectAll();
        foreach (GameObject edge in EdgeGameObjects)
        {
            Destroy(edge);
        }
        foreach (GameObject node in NodeGameObjects)
        {
            Destroy(node);
        }
        for (int i = NodeViewList.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(NodeViewList.transform.GetChild(i).gameObject);
        }
        for (int i = EdgeViewList.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(EdgeViewList.transform.GetChild(i).gameObject);
        }
    }



    public void physicsJointSpringChange()
    {
        foreach (SpringJoint joint in SpringJointArray)
        {
            joint.spring = sliders[0].value;
        }

    }
    public void physicsJointDamperChange()
    {
        foreach (SpringJoint joint in SpringJointArray)
        {
            joint.damper = sliders[1].value;
        }
    }
    public void physicsJointMinDistanceChange()
    {
        foreach (SpringJoint joint in SpringJointArray)
        {
            joint.maxDistance = sliders[2].value;
        }
    }
    public void physicsJointMaxDistanceChange()
    {
        foreach (SpringJoint joint in SpringJointArray)
        {
            joint.minDistance = sliders[3].value;
        }
    }
    public void physicsNodeCollisionSphereSize()
    {
        foreach (GameObject node in NodeGameObjects)
        {
            node.GetComponent<SphereCollider>().radius = sliders[4].value;
        }
    }



    public void generateCube()
    {
        // The purpose of this functionality was the creation of a measuring device, so that the node and edge positions could be measured in real time.
        // But sadly there isn't enough time to flesh it out. For now this method just creates a cube around the graph, which if nothing helps with the graph staying in one place.
        if (quadList.Count == 0)
        {
            GameObject North = Instantiate(quadPrefab, new Vector3(10f * fieldSize, 0, 0), Quaternion.Euler(0, 90.0f, 0), transform.GetChild(1));
            GameObject West = Instantiate(quadPrefab, new Vector3(0, 0, 10f * fieldSize), Quaternion.Euler(0, 0, 0), transform.GetChild(1));
            GameObject East = Instantiate(quadPrefab, new Vector3(0, 0, -10f * fieldSize), Quaternion.Euler(0, 180.0f, 0), transform.GetChild(1));
            GameObject South = Instantiate(quadPrefab, new Vector3(-10f * fieldSize, 0, 0), Quaternion.Euler(0, -90.0f, 0), transform.GetChild(1));
            GameObject Top = Instantiate(quadPrefab, new Vector3(0, 10f * fieldSize, 0), Quaternion.Euler(-90.0f, 0, 0), transform.GetChild(1));
            GameObject Bottom = Instantiate(quadPrefab, new Vector3(0, -10f * fieldSize, 0), Quaternion.Euler(90.0f, 0, 0), transform.GetChild(1));

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
                quad.transform.localScale = new Vector3(20 * fieldSize, 20 * fieldSize, 1);
            }
        }
        else
        {
            quadList[0].transform.position = new Vector3(10f * fieldSize, 0, 0);
            quadList[1].transform.position = new Vector3(0, 0, 10f * fieldSize);
            quadList[2].transform.position = new Vector3(0, 0, -10f * fieldSize);
            quadList[3].transform.position = new Vector3(-10f * fieldSize, 0, 0);
            quadList[4].transform.position = new Vector3(0, 10f * fieldSize, 0);
            quadList[5].transform.position = new Vector3(0, -10f * fieldSize, 0);
            foreach (GameObject quad in quadList)
            {
                quad.SetActive(true);
                quad.transform.localScale = new Vector3(20 * fieldSize, 20 * fieldSize, 1);
            }
        }
    }
}

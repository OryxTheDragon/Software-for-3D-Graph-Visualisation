using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.Classes.DataObjects;
using System.IO;

namespace Assets.Classes.DataStructures
{
    public class Graph
    {
        private Treap<string,Node> nodes;
        private Treap<string,Edge> edges;

        public Graph()
        {
            nodes = new Treap<string,Node>();
            edges = new Treap<string,Edge>();
        }

        public void createNode(string node_id)
        {
            Node temp = new Node(node_id);
            nodes.Insert(temp.getNodeId(),temp);
        }

        public void insertNode(Node node)
        {
            nodes.Insert(node.getNodeId(),node);
        }

        public void insertEdge(Edge edge)
        {
            edges.Insert(edge.getEdgeId(),edge);
        }

        public void removeNode(Node node)
        {
            foreach (Edge edge in edges)
            {
                if (edge.getEndNode() == node || edge.getStartNode() == node)
                {
                    removeEdge(edge);
                }
            }
            nodes.Remove(node.getNodeId());
        }

        public Node searchNodes(string node_id)
        {
            return nodes.getTNode(node_id);
        }

        public void removeEdge(Edge edge)
        {
            edges.Remove(edge.getEdgeId());
        }
        public Treap<string, Edge> getEdges() { return edges; }
        public Treap<string, Node> getNodes() { return nodes; }

        public void createEdge(string edge_id, Node node1, Node node2, Direction direction)
        {
            foreach (Edge edge in edges)
            {
                if (edge.getStartNode() == node1 && edge.getEndNode() == node2)
                {
                    return;
                }
            }
            edges.Insert(edge_id, new Edge(edge_id, node1, node2, direction));
        }

        public void loadGraphFromCSV(string filepath, bool overwrite)
        {
            using (StreamReader reader = new StreamReader(filepath))
            {
                if (overwrite) {
                    this.nodes = new Treap<string, Node>();
                    this.edges = new Treap<string, Edge>();
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
                        Node node = new Node(nodeId);
                        node.setPosition(posX, posY, posZ);
                        insertNode(node);
                    }
                    else if (values[0].Equals("E"))
                    {
                        if (values.Length > 5 || values.Length < 3)
                        {
                            Debug.LogErrorFormat("Invalid data format in line: {0}", line);
                            continue;
                        }
                        else if (values.Length == 5) {
                            string edgeId = values[1];
                            Node startNode = searchNodes(values[2]);
                            Node endNode = searchNodes(values[3]);
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
                            createEdge(edgeId, startNode, endNode, direction);
                        }
                        // TODO 3 value edges
                    }
                    else
                    {
                        Debug.LogErrorFormat("Invalid data format in line: {0}", line);
                        continue;
                    }
                }
            }
        }
        public void readNodesFromCSV(string filePath, bool overwrite)
        {
            if (overwrite)
            {
                this.nodes = new Treap<string,Node>();
            }
            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    // Create node from CSV data
                    if (values.Length >= 1)
                    {
                        string nodeId = values[0];
                        Node node = new Node(nodeId);
                        insertNode(node);
                    }
                }
            }
        }
        public void readEdgesFromCSV(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    // Create edge from CSV data
                    if (values.Length >= 3)
                    {
                        string edgeId = values[0];
                        string startNodeId = values[1];
                        string endNodeId = values[2];
                        Direction direction = Direction.Undirected; // Default to undirected

                        // Parse direction if provided
                        if (values.Length >= 4 && Direction.TryParse(values[3], true, out Direction parsedDirection))
                        {
                            direction = parsedDirection;
                        }

                        Node startNode = searchNodes(startNodeId);
                        Node endNode = searchNodes(endNodeId);

                        // Create edge only if its nodes exist
                        if (startNode != null && endNode != null)
                        {
                            Edge edge = new Edge(edgeId, startNode, endNode, direction);
                            insertEdge(edge);
                        }
                    }
                }
            }
        }

    }
}
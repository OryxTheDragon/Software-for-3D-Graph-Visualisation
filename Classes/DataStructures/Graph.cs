using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.Classes.DataObjects;
using System.IO;
using UnityEngine.UIElements;
using System.Linq;
using UnityEngine.Networking.Types;
using System.Collections;

namespace Assets.Classes.DataStructures
{
    public class Graph
    {
        private Treap<string, Node> nodes;
        private Treap<string, Edge> edges;
        private float scale = 500f;
        private int numOfEdges;
        private int numOfNodes;

        public Graph()
        {
            nodes = new Treap<string, Node>();
            edges = new Treap<string, Edge>();
            numOfEdges = 0;
            numOfNodes = 0;
        }
        public float getScale() { return scale; }
        public void setScale(float scale) { this.scale = scale; }
        public int getNumOfNodes() { return numOfNodes; }
        public int getNumOfEdges() { return numOfEdges; }

        public Node createNode(string node_id, Vector3? position)
        {
            if (node_id != null)
            {
                if (position == null)
                {
                    return new Node(node_id);
                }
                else
                {
                    return new Node(node_id, position ?? Vector3.zero);
                }
            }
            else
            {
                Console.Error.WriteLine("Attempted to create node failed because its ID did not exist. ");
                return null;
            }
        }
        public Edge createEdge(string edge_id, Node node1, Node node2, Direction direction)
        {
            if (edge_id != null && node1 != null && node2 != null)
            {
                return new Edge(edge_id, node1, node2, direction);
            }
            else
            {
                Console.Error.WriteLine("Attempted to create edge failed because its ID or its nodes did not exist. ");
                return null;
            }
        }

        public void insertNode(Node node)
        {
            if (!nodes.contains(node.getNodeId()))
            {
                nodes.insert(node.getNodeId(), node);
                numOfNodes++;
            }
            else
            {
                Console.Error.WriteLine("Node " + node.getNodeId() + " was attempted to be inserted into the structee already exists in the structure and therefore will not be inserted. ");
            }
        }

        public void insertEdge(Edge edge)
        {
            if (!edges.Contains(edge))
            {
                edges.insert(edge.getEdgeId(), edge);
                numOfEdges++;
            }
            else
            {
                Console.Error.WriteLine("Edge " + edge.getEdgeId() + " from node " + edge.getStartNode() + " to node " + edge.getEndNode()
                    + " was attempted to be inserted into the structee already exists in the structure and therefore will not be inserted. ");
            }
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
            nodes.remove(node.getNodeId());
        }

        public Node searchNodes(string node_id)
        {
            return nodes.getTNode(node_id);
        }

        public void removeEdge(Edge edge)
        {
            edges.remove(edge.getEdgeId());
        }
        public Treap<string, Edge> getEdges() { return edges; }
        public Treap<string, Node> getNodes() { return nodes; }

        public void readNodesFromCSV(string filePath, bool overwrite)
        {
            if (overwrite)
            {
                this.nodes = new Treap<string, Node>();
            }
            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    if (values.Length >= 1)
                    {
                        string nodeId = values[0];
                        insertNode(createNode(nodeId, null));
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

                    if (values.Length >= 3)
                    {
                        string edgeId = values[0];
                        string startNodeId = values[1];
                        string endNodeId = values[2];
                        Direction direction = Direction.Undirected;

                        if (values.Length >= 4 && Direction.TryParse(values[3], true, out Direction parsedDirection))
                        {
                            direction = parsedDirection;
                        }

                        Node startNode = searchNodes(startNodeId);
                        Node endNode = searchNodes(endNodeId);

                        if (startNode != null && endNode != null)
                        {
                            insertEdge(createEdge(edgeId, startNode, endNode, direction));
                        }
                    }
                }
            }
        }
    }
}
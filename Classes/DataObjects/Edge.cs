using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Classes.DataObjects
{
    public class Edge : IComparable<Edge> , IEnumerable<Edge>
    {
        private Color edgeColor;
        private string edge_id;
        private Node start;
        private Node end;
        private Direction direction;

        public Edge(string edge_id, Node start, Node end, Direction direction)
        {
            this.edgeColor = Color.white;
            this.start = start;
            this.end = end;
            this.direction = direction;
            this.edge_id = edge_id;
        }

        public Color getEdgeColor() { return edgeColor; }
        public Node getStartNode() { return start; }
        public Node getEndNode() { return end; }
        public Direction getDirection() { return direction; }
        public string getEdgeId() { return edge_id; }

        public void setEdgeColor(Color color)
        {
            this.edgeColor = color;
        }
        public void setStartNode(Node newStartNode)
        {
            this.start = newStartNode;
        }
        public void setEndNode(Node newEndNode)
        {
            this.end = newEndNode;
        }
        public void setDirection(Direction newDirection)
        {
            this.direction = newDirection;
        }

        public int CompareTo(Edge other)
        {
            return edge_id.CompareTo(other.getEdgeId());
        }
        public IEnumerator<Edge> GetEnumerator()
        {
            yield return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
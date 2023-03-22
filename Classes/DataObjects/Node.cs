using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Classes.DataObjects
{    public class Node : IComparable<Node>, IEnumerable<Node>
    {
        private string node_id;
        private Color NodeColor;
        private Vector3 position;

        public Node(string node_id)
        {
            this.position = new Vector3();
            this.NodeColor = Color.white;
            this.node_id = node_id; 
        }
        public string getNodeId() { return node_id; }
        
        public Color getNodeColor() { return NodeColor; }

        public Vector3 getPosition() { return position; }

        public void setNodeColor(Color color)
        {
            this.NodeColor = color;
        }
        public void setPosition(Vector3 position)
        {
            this.position = position;
        }
        public void setPosition(float x, float y, float z)
        {
            this.position = new Vector3(x, y, z);
        }

        public int CompareTo(Node other)
        {
            return node_id.CompareTo(other.getNodeId());
        }
        public IEnumerator<Node> GetEnumerator()
        {
            yield return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

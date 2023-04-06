﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Classes.DataStructures
{
    using System;
    using System.Collections;
    using UnityEngine.Networking.Types;

    public class Treap<K, T> : IEnumerable<T> where K : IComparable<K>
    {
        private class TreapNode
        {
            public T Value { get; set; }
            public K Key { get; set; }
            public int Priority { get; set; }
            public TreapNode Left { get; set; }
            public TreapNode Right { get; set; }
            public int compareTo(TreapNode other)
            {
                return Key.CompareTo(other.Key);
            }

            public TreapNode(K key, T value, int priority)
            {
                Value = value;
                Key = key;
                Priority = priority;
            }
        }

        private readonly Random _random = new();
        private TreapNode _root;

        public void insert(K key, T value)
        {
            _root = Insert(_root, value, key, _random.Next());
        }

        private TreapNode Insert(TreapNode tNode, T value, K key, int priority)
        {
            if (tNode == null)
            {
                return new TreapNode(key, value, priority);
            }

            if (key.CompareTo(tNode.Key) < 0)
            {
                tNode.Left = Insert(tNode.Left, value, key, priority);
                if (tNode.Left.Priority > tNode.Priority)
                {
                    tNode = RotateRight(tNode);
                }
            }
            else
            {
                tNode.Right = Insert(tNode.Right, value, key, priority);
                if (tNode.Right.Priority > tNode.Priority)
                {
                    tNode = RotateLeft(tNode);


                }
            }

            return tNode;
        }

        public bool remove(K key)
        {
            if (Find(_root, key) == null)
            {
                return false;
            }

            _root = Remove(_root, key);
            return true;
        }

        public bool isEmpty() {
            return _root == null;
        }

        private TreapNode Remove(TreapNode tNode, K key)
        {
            if (key.CompareTo(tNode.Key) < 0)
            {
                tNode.Left = Remove(tNode.Left, key);
            }
            else if (key.CompareTo(tNode.Key) > 0)
            {
                tNode.Right = Remove(tNode.Right, key);
            }
            else if (tNode.Left == null)
            {
                tNode = tNode.Right;
            }
            else if (tNode.Right == null)
            {
                tNode = tNode.Left;
            }
            else if (tNode.Left.Priority > tNode.Right.Priority)
            {
                tNode = RotateRight(tNode);
                tNode.Right = Remove(tNode.Right, key);
            }
            else
            {
                tNode = RotateLeft(tNode);
                tNode.Left = Remove(tNode.Left, key);
            }

            return tNode;
        }

        public bool contains(K key)
        {
            return Find(_root, key) != null;
        }
        private TreapNode Find(TreapNode tNode, K key)
        {
            while (tNode != null)
            {
                if (key.CompareTo(tNode.Key) < 0)
                {
                    tNode = tNode.Left;
                }
                else if (key.CompareTo(tNode.Key) > 0)
                {
                    tNode = tNode.Right;
                }
                else
                {
                    return tNode;
                }
            }

            return null;
        }
        public T getTNode(K key)
        {
            TreapNode node = _root;
            TreapNode returnedNode = node;

            while (node != null && node.Key.CompareTo(key) != 0) {
                node = key.CompareTo(node.Key) < 0 ? node.Left : node.Right;
                if (node != null)
                {
                    returnedNode = node;
                }
            }
            return returnedNode.Value;

        }

        private TreapNode RotateLeft(TreapNode tNode)
        {
            TreapNode temp = tNode.Right;
            tNode.Right = temp.Left;
            temp.Left = tNode;
            return temp;
        }

        private TreapNode RotateRight(TreapNode tNode)
        {
            TreapNode temp = tNode.Left;
            tNode.Left = temp.Right;
            temp.Right = tNode;
            return temp;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            // I decided to implement the in-order traversal method
            if (_root == null)
            {
                yield break;
            }

            var stack = new Stack<TreapNode>();
            var current = _root;

            while (current != null || stack.Count > 0)
            {
                // Traverse all the way to the leftmost tNode
                while (current != null)
                {
                    stack.Push(current);
                    current = current.Left;
                }

                // Visit the leftmost tNode and move to its right subtree
                current = stack.Pop();
                yield return current.Value;
                current = current.Right;
            }
        }
    }
}

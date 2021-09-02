using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph<Data> where Data : class
{
    private List<Node> _nodes = new List<Node>();

    public class Node
    {
        private Data _data;
        private List<Node> _incoming = new List<Node>();
        private List<Node> _outgoing = new List<Node>();
        
        internal Node(Data data)
        {
            _data = data;
        }

        public Data GetData()
        {
            return _data;
        }

        public List<Node> GetIncoming()
        {
            return _incoming;
        }

        public List<Node> GetOutgoing()
        {
            return _outgoing;
        }
    }

    public Node AddNode(Data data)
    {
        Node node = new Node(data);
        _nodes.Add(node);
        return node;
    }

    public Node FindNode(Data data)
    {
        for (int i = 0; i < _nodes.Count; i++)
        {
            if (_nodes[i].GetData() == data)
                return _nodes[i];
        }

        return null;
    }

    public void AddEdge(Node srcNode, Node dstNode)
    {
        if (srcNode == null || dstNode == null)
        { 
            Debug.Log("Could not add edge because both nodes are not in the graph");
            return;
        }

        srcNode.GetOutgoing().Add(dstNode);
        dstNode.GetIncoming().Add(srcNode);
    }

    public void AddEdge(Data src, Data dst)
    {
        AddEdge(FindNode(src), FindNode(dst));
    }
}
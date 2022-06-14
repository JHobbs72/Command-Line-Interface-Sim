using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Graph : ScriptableObject
{
    [SerializeField]
    private List<Node> nodes;
    private List<Node> Nodes
    {
        get
        {
            if (nodes == null)
            {
                nodes = new List<Node>();
            }
            return nodes;
        }
    }

    public static Graph Create(string name)
    {
        Graph graph = CreateInstance<Graph>();

        string path = string.Format("Assets/FileSystem/{0}.asset", name);
        AssetDatabase.CreateAsset(graph, path);

        return graph;
    }

    public void AddNode(Node node)
    {
        Nodes.Add(node);
        AssetDatabase.AddObjectToAsset(node, this);
        AssetDatabase.SaveAssets();
    }

    public void removeDirectoryNode(Node node)
    {
        if (node.getNeighbours().Count > 0)
        {
            Debug.Log(">> Cannot remove - has children");
        }
        else
        {
            Nodes.Remove(node);
        }
    }

    public void removeLeafNode(Node node)
    {
        Nodes.Remove(node);
    }

    public Node getRootNode()
    {
        return Nodes[0];
    }
}

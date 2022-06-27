using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Graph : ScriptableObject
{
    // Contains methods to manage the structure of the graph (file structure)

    private GraphManager _outputSource;

    public void Awake()
    {
        _outputSource = FindObjectOfType<GraphManager>();
    }

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

    // Add nodes to graph
    // Root (First node i.e. no parent)
    public void AddRoot(DirectoryNode root)
    {
        Nodes.Add(root);
        AssetDatabase.AddObjectToAsset(root, this);
        AssetDatabase.SaveAssets();
    }
    
    // All other nodes
    public void AddNode(DirectoryNode parent, Node node)
    {
        Nodes.Add(node);
        parent.Neighbours.Add(node);
        AssetDatabase.AddObjectToAsset(node, this);
        AssetDatabase.SaveAssets();
    }

    public void RemoveNode(DirectoryNode parent, Node target)
    {
        if (target.GetType() == typeof(DirectoryNode) && target.getNeighbours().Count > 0)
        {
            _outputSource.SendOutput("rmdir: " + target.name + ": Directory not empty");
            return;
        }
        
        Nodes.Remove(target);
        parent.RemoveNeighbour(target);
        AssetDatabase.DeleteAsset("Assets/FileSystem/FileSystemGraph.asset/" + target.name);
        AssetDatabase.SaveAssets();
    }

    public DirectoryNode GetRootNode()
    {
        return (DirectoryNode)Nodes[0];
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Graph : ScriptableObject
{
    // Contains methods to manage the structure of the graph (file structure)

    private GraphManager outputSource;

    public void Awake()
    {
        outputSource = FindObjectOfType<GraphManager>();
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

    public void AddNode(Node node)
    {
        Nodes.Add(node);
        AssetDatabase.AddObjectToAsset(node, this);
        AssetDatabase.SaveAssets();
    }

    // Extra step in removing a 'directory node' - must be removed from it's
    // parent's neighbours list
    public void removeDirectoryNode(DirectoryNode parent, Node node)
    {
        if (node.getNeighbours().Count > 0)
        {
            outputSource.sendOutput("Cannot remove - has children");
        }
        else
        {
            Nodes.Remove(node);
            parent.removeNeighbour(node);
        }
    }

    public void removeLeafNode(DirectoryNode parent, FileNode target)
    {
        Nodes.Remove(target);
        parent.removeNeighbour(target);
    }

    public DirectoryNode getRootNode()
    {
        return (DirectoryNode)Nodes[0];
    }
}

/*
 * Author: Jacob Hobbs - 190161842
 * Date : July 2022
 */

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Graph : ScriptableObject
{
    // Contains methods to manage the structure of the graph (file structure)

    private GraphManager _outputSource;
    private GameObject _fileSystemObj;

    // On start find and assign graph manager
    public void Awake()
    {
        _outputSource = FindObjectOfType<GraphManager>();
    }

    // Build Nodes list
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

    // Create graph structure for file system
    public static Graph Create(string name)
    {
        Graph graph = CreateInstance<Graph>();

        return graph;
    }

    // Add nodes to graph
    // Root (First node i.e. no parent)
    public void AddRoot(DirectoryNode root)
    {
        Nodes.Add(root);
    }
    
    // All other nodes
    public void AddNode(DirectoryNode parent, Node node)
    {
        Nodes.Add(node);
        parent.Neighbours.Add(node);
        node.SetParent(parent);
    }

    // Remove a node from the graph and assets 
    public void RemoveNode(DirectoryNode parent, Node target)
    {
        if (target.GetType() == typeof(DirectoryNode) && target.GetNeighbours().Count > 0)
        {
            _outputSource.SendOutput("zsh: " + target.name + ": Directory not empty");
            return;
        }
        
        Nodes.Remove(target);
        parent.RemoveNeighbour(target);
    }

    public DirectoryNode GetRootNode()
    {
        return (DirectoryNode)Nodes[0];
    }
}

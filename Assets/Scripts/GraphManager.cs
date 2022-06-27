using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    // Create and manage the graph file structure

    private Graph _graph;
    private DirectoryNode _currentNode;
    private List<Node> _currentPath;
    private outputText _outputSource;
    private string _currentCommand;
    private autoScroll scroll;

    void Start()
    {
        _graph = Graph.Create("FileSystemGraph");

        // Nodes created and added from start up
        DirectoryNode rootNode = Node.Create<DirectoryNode>("UserA");
        DirectoryNode documents = Node.Create<DirectoryNode>("Documents");
        FileNode file1 = Node.Create<FileNode>("file1.txt");
        FileNode file2 = Node.Create<FileNode>("file2.txt");
        _graph.AddRoot(rootNode);
        _graph.AddNode(rootNode, documents);
        _graph.AddNode(rootNode, file1);
        _graph.AddNode(documents, file2);

        // Helper variables initialised
        _currentNode = rootNode;
        _currentPath = new List<Node>();
        AddToCurrentPath(rootNode);

        _outputSource = FindObjectOfType<outputText>();
        _currentCommand = "";
        scroll = FindObjectOfType<autoScroll>();
    }
    
    // Remove a node
    public void RemoveNode(DirectoryNode parentNode, Node targetNode)
    {
        _graph.RemoveNode(parentNode, targetNode);
    }
    
    // Add a file node
    public void AddFileNode(DirectoryNode sourceNode, string name)
    {
        FileNode newFile = Node.Create<FileNode>(name);
        _graph.AddNode(sourceNode, newFile);
    }

    // Add a directory node
    public void AddDirectoryNode(string name)
    {
        DirectoryNode newDir = Node.Create<DirectoryNode>(name);
        _graph.AddNode(_currentNode, newDir);
    }
    
    public DirectoryNode GetRootNode()
    {
        return _graph.GetRootNode();
    }

    public DirectoryNode GetCurrentNode()
    {
        return _currentNode;
    }

    public void SetCurrentNode(DirectoryNode node)
    {
        _currentNode = node;
    }

    public List<Node> GetCurrentPath()
    {
        return _currentPath;
    }

    public void AddToCurrentPath(DirectoryNode directory)
    {
        _currentPath.Add(directory);
    }

    // One step back in current path through file system
    public DirectoryNode StepBackInPath()
    {
        if (_currentPath.Count > 1)
        {
            _currentPath.RemoveAt(_currentPath.Count - 1);
            return (DirectoryNode)_currentPath[^1];
        }
        
        SendOutput("At root");
        return null;
    }

    public void SetCurrentCommand(string command)
    {
        _currentCommand = command;
    }
    
    public void SendOutput(string content)
    {
        _outputSource.AddOutput(_currentCommand, content);
    }
}

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

        // Nodes added from start up
        DirectoryNode rootNode = DirectoryNode.Create<DirectoryNode>("UserA");
        DirectoryNode documents = DirectoryNode.Create<DirectoryNode>("Documents");
        FileNode file1 = FileNode.Create<FileNode>("file1.txt");
        FileNode file2 = FileNode.Create<FileNode>("file2.txt");

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
    
    // TODO Add checks for target being a file node - add checks in graph.cs
    // ^^ Redundant?
    public void RemoveNode(DirectoryNode parentNode, Node targetNode)
    {
        _graph.RemoveNode(parentNode, targetNode);
    }
    
    public void AddFileNode(DirectoryNode sourceNode, string name)
    {
        FileNode newFile = FileNode.Create<FileNode>(name);
        _graph.AddNode(sourceNode, newFile);
    }

    public void AddDirectoryNode(string name)
    {
        DirectoryNode newDir = DirectoryNode.Create<DirectoryNode>(name);
        _graph.AddNode(_currentNode, newDir);
    }

    // TODO check that what should be returned is expecting a directory node
    // Done?
    public DirectoryNode GetRootNode()
    {
        return _graph.GetRootNode();
    }

    // TODO check that what should be returned is expecting a directory node
    // Done?
    public DirectoryNode GetCurrentNode()
    {
        return _currentNode;
    }

    // TODO check that parameter is a directory node
    // Done?
    public void SetCurrentNode(DirectoryNode node)
    {
        _currentNode = node;
    }

    public List<Node> GetCurrentPath()
    {
        return _currentPath;
    }

    // TODO check that parameter is a directory node
    // Done?
    public void AddToCurrentPath(DirectoryNode directory)
    {
        _currentPath.Add(directory);
    }

    // TODO move checks to graph.cs
    // TODO check that return type expected is directory node
    // Done?
    public DirectoryNode StepBackInPath()
    {
        if (_currentPath.Count > 1)
        {
            _currentPath.RemoveAt(_currentPath.Count - 1);
            return (DirectoryNode)_currentPath[_currentPath.Count - 1];
        }
        else
        {
            SendOutput("At root");
            return null;
        }
    }

    public void SendOutput(string content)
    {
        _outputSource.AddOutput(_currentCommand, content);
    }

    public void SetCurrentCommand(string command)
    {
        _currentCommand = command;
    }
}

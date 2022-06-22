using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    // Create and manage the graph file structure

    private Graph graph;
    private DirectoryNode currentNode;
    private List<Node> currentPath;
    private outputText outputSource;
    private string currentCommand;
    private autoScroll scroll;

    void Start()
    {
        graph = Graph.Create("FileSystemGraph");

        // Nodes added from start up
        DirectoryNode rootNode = DirectoryNode.Create<DirectoryNode>("UserA");
        DirectoryNode Documents = DirectoryNode.Create<DirectoryNode>("Documents");
        FileNode file1 = FileNode.Create<FileNode>("file1.txt");
        FileNode file2 = FileNode.Create<FileNode>("file2.txt");
        rootNode.Neighbours.Add(Documents);
        rootNode.Neighbours.Add(file1);
        Documents.Neighbours.Add(file2);

        graph.AddNode(rootNode);
        graph.AddNode(Documents);
        graph.AddNode(file1);
        graph.AddNode(file2);

        // Helper variables initialised
        currentNode = rootNode;
        currentPath = new List<Node>();
        currentPath.Add(rootNode);

        outputSource = FindObjectOfType<outputText>();
        currentCommand = "";
        scroll = FindObjectOfType<autoScroll>();
    }

    public void addLeafNode(DirectoryNode sourceNode, string name)
    {
        FileNode newFile = FileNode.Create<FileNode>(name);
        sourceNode.Neighbours.Add(newFile);
        graph.AddNode(newFile);
    }

    // TODO Add checks for target being a file node - add checks in graph.cs
    // Done?
    public void removeLeafNode(DirectoryNode parentNode, FileNode targetNode)
    {
        graph.removeLeafNode(parentNode, targetNode);
    }

    // TODO Add checks for target being a directory node - add checks in graph.cs
    // Done?
    public void removeDirectoryNode(DirectoryNode parent, DirectoryNode node)
    {
        graph.removeDirectoryNode(parent, node);
    }

    public void addDirectoryNode(string name)
    {
        DirectoryNode newDir = DirectoryNode.Create<DirectoryNode>(name);
        currentNode.Neighbours.Add(newDir);
        graph.AddNode(newDir);
    }

    // TODO check that what should be returned is expecting a directory node
    // Done?
    public DirectoryNode getRootNode()
    {
        return graph.getRootNode();
    }

    // TODO check that what should be returned is expecting a directory node
    // Done?
    public DirectoryNode getCurrentNode()
    {
        return currentNode;
    }

    // TODO check that parameter is a directory node
    // Done?
    public void setCurrentNode(DirectoryNode node)
    {
        currentNode = node;
    }

    public List<Node> getCurrentPath()
    {
        return currentPath;
    }

    // TODO check that parameter is a directory node
    // Done?
    public void addToCurrentPath(DirectoryNode directory)
    {
        currentPath.Add(directory);
    }

    // TODO move checks to graph.cs
    // TODO check that return type expected is directory node
    // Done?
    public DirectoryNode stepBackInPath()
    {
        if (currentPath.Count > 1)
        {
            currentPath.RemoveAt(currentPath.Count - 1);
            return (DirectoryNode)currentPath[currentPath.Count - 1];
        }
        else
        {
            sendOutput("At root");
            return null;
        }
    }

    public void sendOutput(string content)
    {
        outputSource.addOutput(currentCommand, content);
    }

    public void setCurrentCommand(string command)
    {
        currentCommand = command;
    }
}

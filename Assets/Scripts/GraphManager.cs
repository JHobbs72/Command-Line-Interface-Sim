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

        // Helper variables initialsised
        currentNode = rootNode;
        currentPath = new List<Node>();
        currentPath.Add(rootNode);

        outputSource = FindObjectOfType<outputText>();
    }

    public void addLeafNode(string name)
    {
        FileNode newFile = FileNode.Create<FileNode>(name);
        currentNode.Neighbours.Add(newFile);
        graph.AddNode(newFile);
    }

    // TODO Add checks for target being a file node - add checks in graph.cs
    public void removeLeafNode(DirectoryNode parentNode, FileNode targetNode)
    {
        graph.removeLeafNode(parentNode, targetNode);
    }

    // TODO Add checks for target being a directory node - add checks in graph.cs
    public void removeDirectoryNode(DirectoryNode parent, Node node)
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
    public Node getRootNode()
    {
        return graph.getRootNode();
    }

    // TODO check that what should be returned is expecting a directory node
    public Node getCurrentNode()
    {
        return currentNode;
    }

    // TODO check that parameter is a directory node
    public void setCurrentNode(DirectoryNode node)
    {
        currentNode = node;
    }

    public List<Node> getCurrentPath()
    {
        return currentPath;
    }

    // TODO check that parameter is a directory node
    public void addToCurrentPath(DirectoryNode directory)
    {
        currentPath.Add(directory);
    }

    // TODO move checks to graph.cs
    // TODO check that return type expected is directory node
    public DirectoryNode stepBackInPath()
    {
        if (currentPath.Count > 1)
        {
            currentPath.RemoveAt(currentPath.Count - 1);
            return (DirectoryNode)currentPath[currentPath.Count - 1];
        }
        else
        {
            Debug.Log("At root");
            sendOutput("At root");
            return null;
        }
    }

    public void sendOutput(string content)
    {
        outputSource.addOutput(content);
    }
}

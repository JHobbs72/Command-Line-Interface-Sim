using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    private Graph graph;
    private DirectoryNode currentNode;
    private List<Node> currentPath;

    // Start is called before the first frame update
    void Start()
    {
        graph = Graph.Create("FileSystemGraph");

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

        currentNode = rootNode;
        currentPath = new List<Node>();
        currentPath.Add(rootNode);
    }

    public void addLeafNode(string name)
    {
        FileNode newFile = FileNode.Create<FileNode>(name);
        currentNode.Neighbours.Add(newFile);
        graph.AddNode(newFile);
    }

    public void removeLeafNode(DirectoryNode parentNode, FileNode targetNode)
    {
        graph.removeLeafNode(parentNode, targetNode);
    }

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

    public Node getRootNode()
    {
        return graph.getRootNode();
    }

    public Node getCurrentNode()
    {
        return currentNode;
    }

    public void setCurrentNode(DirectoryNode node)
    {
        currentNode = node;
    }

    public List<Node> getCurrentPath()
    {
        return currentPath;
    }

    public void addToCurrentPath(DirectoryNode directory)
    {
        currentPath.Add(directory);
    }

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
            return null;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    private Graph graph;
    private Node currentNode;

    // Start is called before the first frame update
    void Start()
    {
        graph = Graph.Create("FileSystemGraph");

        DirectoryNode rootNode = DirectoryNode.Create<DirectoryNode>("UserA");
        DirectoryNode Documents = DirectoryNode.Create<DirectoryNode>("Documents");
        FileNode file1 = FileNode.Create<FileNode>("file1.txt");
        FileNode file2 = FileNode.Create<FileNode>("file2.txt");
        rootNode.Neighbours.Add(Documents);
        Documents.Neighbours.Add(file1);
        Documents.Neighbours.Add(file2);

        graph.AddNode(rootNode);
        graph.AddNode(Documents);
        graph.AddNode(file1);
        graph.AddNode(file2);

        currentNode = rootNode;
    }

    public Node getRootNode()
    {
        return graph.getRootNode();
    }

    public Node getCurrentNode()
    {
        return currentNode;
    }

    public Node[] getNeighbours()
    {
        if (currentNode.GetType().ToString().Equals("DiretoryNode"))
        {
            Debug.Log("Directory Node");
            // May return null as using method from 'Node' instead of 'Directory Node'
            return currentNode.getNeighbours();
        }
        else
        {
            Debug.Log("Cannot get neighbours of leaf node");
            return null;
        }
    }
}

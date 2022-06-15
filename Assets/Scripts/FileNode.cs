using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileNode : Node
{
    // Inherits from generic object 'Node' - describes a leaf node of the graph
    // i.e. no children and in this context must be a file not a directory

    private GraphManager outputSource;

    public void Awake()
    {
        outputSource = FindObjectOfType<GraphManager>();
    }

    // Protects from error when trying to get children of a leaf node
    public override List<Node> getNeighbours()
    {
        outputSource.sendOutput("Cannont get children on a leaf node");
        Debug.Log("Cannont get children on a leaf node");
        return null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileNode : Node
{
    private GraphManager outputSource;

    public void Awake()
    {
        outputSource = FindObjectOfType<GraphManager>();
    }

    public override List<Node> getNeighbours()
    {
        outputSource.sendOutput("Cannont get children on a leaf node");
        Debug.Log("Cannont get children on a leaf node");
        return null;
    }
}

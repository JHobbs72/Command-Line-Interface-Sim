using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileNode : Node
{
    public override List<Node> getNeighbours()
    {
        Debug.Log("Cannont get children on a leaf node");
        return null;
    }
}

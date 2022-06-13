using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LS : MonoBehaviour
{
    public GraphManager fileSystem;

    public void ls(string options)
    {
        Debug.Log("Listing files");
        Debug.Log(options);

        Node localCurrentNode = fileSystem.getCurrentNode();
        localCurrentNode.GetType();

        //Node[] neighbours = localCurrentNode.getNeighbours();

        // Return all neighbours of current node
    }
}

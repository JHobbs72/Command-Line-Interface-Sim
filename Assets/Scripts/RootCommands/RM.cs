using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RM : MonoBehaviour
{
    public GraphManager fileSystem;

    public void rm(string option)
    {
        List<Node> neighbours = fileSystem.getCurrentNode().getNeighbours();
        bool found = false;

        foreach (Node node in neighbours)
        {
            if (node.name == option)
            {
                fileSystem.removeLeafNode(node);
                found = true;
                break;
            }
        }

        if (!found)
        {
            Debug.Log(">> No file found named " + option);
        }
    }
}

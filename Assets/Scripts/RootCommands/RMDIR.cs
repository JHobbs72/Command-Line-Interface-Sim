using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RMDIR : MonoBehaviour
{
    public GraphManager fileSystem;

    public void rmdir(string option)
    {
        List<Node> neighbours = fileSystem.getCurrentNode().getNeighbours();
        bool found = false;

        foreach (Node node in neighbours)
        {
            if (node.name == option)
            {
                fileSystem.removeDirectoryNode(node);
                found = true;
                break;
            }
        }

        if (!found)
        {
            Debug.Log(">> No directory found named " + option);
        }
    }
}

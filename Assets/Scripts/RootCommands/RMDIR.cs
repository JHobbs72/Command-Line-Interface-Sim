using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RMDIR : MonoBehaviour
{
    public GraphManager fileSystem;

    public void rmdir(string option)
    {
        DirectoryNode currentNode = (DirectoryNode)fileSystem.getCurrentNode();
        List<Node> neighbours = currentNode.getNeighbours();
        bool found = false;

        foreach (Node node in neighbours)
        {
            if (node.name == option)
            {
                fileSystem.removeDirectoryNode(currentNode, node);
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

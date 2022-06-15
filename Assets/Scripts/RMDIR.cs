using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RMDIR : MonoBehaviour
{
    // Root command for 'remove directory' - Remove empty directory

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
            fileSystem.sendOutput("No directory found named " + option);
            Debug.Log(">> No directory found named " + option);
        }
    }
}

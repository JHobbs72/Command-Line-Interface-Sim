using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RM : MonoBehaviour
{
    // Root command for 'remove' - remove a file

    public GraphManager fileSystem;

    public void rm(string option)
    {
        DirectoryNode currentNode = (DirectoryNode)fileSystem.getCurrentNode();
        List<Node> neighbours = currentNode.getNeighbours();
        bool found = false;

        // Check requested node is a child of the current node and is a file not
        // a directory
        // TODO move is a file check to graph.cs?
        foreach (Node targetNode in neighbours)
        {
            if (targetNode.GetType() != typeof(FileNode))
            {
                fileSystem.sendOutput("Cannot remove - not a file.");
                Debug.Log("Cannot remove - not a file.");
                break;
            }

            if (targetNode.name == option)
            {
                fileSystem.removeLeafNode(currentNode, (FileNode)targetNode);
                List<Node> newNeighbours = fileSystem.getCurrentNode().getNeighbours();
                found = true;
                break;
            }
        }

        if (!found)
        {
            fileSystem.sendOutput("Cannot remove - not a file.");
            Debug.Log(">> No file found named " + option);
        }
    }
}

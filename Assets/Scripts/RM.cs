using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RM : MonoBehaviour
{
    // Root command for 'remove' - remove a file

    public GraphManager fileSystem;

    public void rm(string option)
    {
        if (option == "")
        {
            fileSystem.SendOutput("usage: rm [-f | -i] [-dPRrvW] file ... \n           unlink file");
            return;
        }
        
        DirectoryNode currentNode = (DirectoryNode)fileSystem.GetCurrentNode();
        List<Node> neighbours = currentNode.GetNeighbours();
        bool found = false;
        bool isDirectory = false;

        // Check requested node is a child of the current node and is a file not
        // a directory
        // TODO move 'is a file' check to graph.cs?
        foreach (Node targetNode in neighbours)
        {
            if (targetNode.name == option && targetNode.GetType() == typeof(FileNode))
            {
                fileSystem.RemoveNode(currentNode, targetNode);
                found = true;
                fileSystem.SendOutput("");
                break;
            }

            if (targetNode.name == option && targetNode.GetType() == typeof(DirectoryNode))
            {
                isDirectory = true;
            }
        }

        if (!found && isDirectory)
        {
            fileSystem.SendOutput(option + " is a directory");
        }
        else if (!found)
        {
            fileSystem.SendOutput("No file found named " + option);
        }
    }
}

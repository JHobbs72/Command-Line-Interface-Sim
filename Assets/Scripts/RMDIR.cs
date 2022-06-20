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
        bool isFile = false;

        // TODO move 'is a directory' check to graph.cs?
        foreach (Node targetNode in neighbours)
        {
            if (targetNode.name == option && targetNode.GetType() == typeof(DirectoryNode))
            {
                fileSystem.removeDirectoryNode(currentNode, (DirectoryNode)targetNode);
                found = true;
                fileSystem.sendOutput("");
                break;
            }
            
            if (targetNode.name == option && targetNode.GetType() == typeof(FileNode))
            {
                isFile = true;
            }
        }
        
        if (!found && isFile)
        {
            fileSystem.sendOutput(option + " Is not a directory");
            Debug.Log(option + " Is not a directory");
        } else if (!found)
        {
            fileSystem.sendOutput("No directory found named " + option);
            Debug.Log(">> No directory found named " + option);
        }
    }
}

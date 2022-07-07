/*
 * Author: Jacob Hobbs - 190161842
 * Date : July 2022
 */

using System.Collections.Generic;
using UnityEngine;

public class RMDIR : MonoBehaviour
{
    // Root command for 'remove directory' - Remove empty directory

    public GraphManager fileSystem;

    public void rmdir(string option)
    {
        if (option == "")
        {
            fileSystem.SendOutput("usage: rmdir [-p] directory ...", false);
            return;
        }
        
        DirectoryNode currentNode = fileSystem.GetCurrentNode();
        List<Node> neighbours = currentNode.GetNeighbours();
        bool found = false;
        bool isFile = false;

        foreach (Node targetNode in neighbours)
        {
            if (targetNode.name == option && targetNode.GetType() == typeof(DirectoryNode))
            {
                fileSystem.RemoveNode(currentNode, targetNode);
                found = true;
                fileSystem.SendOutput("", false);
                break;
            }
            
            if (targetNode.name == option && targetNode.GetType() == typeof(FileNode))
            {
                isFile = true;
            }
        }
        
        if (!found && isFile)
        {
            fileSystem.SendOutput(option + " Is not a directory", false);
        } else if (!found)
        {
            fileSystem.SendOutput("No directory found named " + option, false);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CD : MonoBehaviour
{
    public GraphManager fileSystem;
    private bool validPath = true;
    private Node localCurrentNode;

    public void cd(string options)
    {
        if (options == "..")
        {
            DirectoryNode lastNode = fileSystem.stepBackInPath();
            if (lastNode != null)
            {
                fileSystem.setCurrentNode(lastNode);
                printPath();
            }
        }
        else
        {
            string[] path = options.Split('/');
            localCurrentNode = fileSystem.getCurrentNode();

            // For each directory in path check if it exists in context
            foreach (string dir in path)
            {
                if (validPath)
                {
                    checkNextStep(localCurrentNode, dir);
                }
                else
                {
                    break;
                }
            }

            if (validPath)
            {
                executePathChange(fileSystem.getCurrentNode(), path);
                printPath();
            }
        }
    }

    private void checkNextStep(Node checkNode, string target)
    {
        List<Node> children = checkNode.getNeighbours();

        foreach (Node child in children)
        {
            if (child.name == target && child.GetType() == typeof(DirectoryNode))
            {
                localCurrentNode = child;
                break;
            }
            else
            {
                Debug.Log("No directory named " + target);
                validPath = false;
                localCurrentNode = fileSystem.getCurrentNode();
            }
        }
    }

    private void executePathChange(Node localCurrentNode, string[] targetPath)
    {
        foreach (string nextDir in targetPath)
        {
            List<Node> neighbours = localCurrentNode.getNeighbours();
            foreach (Node node in neighbours)
            {
                if (node.name == nextDir)
                {
                    localCurrentNode = node;
                    fileSystem.setCurrentNode((DirectoryNode)node);
                    fileSystem.addToCurrentPath((DirectoryNode)node);
                    break;
                }
            }
        }
    }

    private void printPath()
    {
        List<string> pathNames = new List<string>();
        foreach (Node dir in fileSystem.getCurrentPath())
        {
            pathNames.Add(dir.name);
        }
        Debug.Log(">> " + string.Join("/", pathNames));
    }
}

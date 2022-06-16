using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CD : MonoBehaviour
{
    // Root command for 'change directory'
    // Take '..', '<path>' or '<directory>' and move to the appropriate directory

    public GraphManager fileSystem;
    private bool validPath;
    private Node localCurrentNode;
    private string invalidNode;

    public void cd(string options)
    {
        validPath = true;
        // Move 'back' a directory
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
            // Get component directories & check path validity
            string[] path = options.Split('/');
            localCurrentNode = fileSystem.getCurrentNode();

            // For each directory in path check if it exists in context
            foreach (string dir in path)
            {
                if (validPath)
                {
                    invalidNode = dir;
                    checkNextStep((DirectoryNode)localCurrentNode, dir);
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
            else
            {
                fileSystem.sendOutput("No directory named " + invalidNode);
                Debug.Log("No directory named " + invalidNode);
                localCurrentNode = fileSystem.getCurrentNode();
            }
        }
    }

    // Check the requested next directory exists
    private void checkNextStep(DirectoryNode checkNode, string target)
    {
        List<Node> children = checkNode.getNeighbours();

        foreach (Node child in children)
        {
            if (child.name == target && child.GetType() == typeof(DirectoryNode))
            {
                localCurrentNode = child;
                validPath = true;
                break;
            }
            else
            {
                validPath = false;
            }
        }
    }

    // Once valid, execute - go to valid file path
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

    // Display new path to user
    private void printPath()
    {
        List<string> pathNames = new List<string>();
        foreach (Node dir in fileSystem.getCurrentPath())
        {
            pathNames.Add(dir.name);
        }
        fileSystem.sendOutput(string.Join("/", pathNames));
        Debug.Log(">> " + string.Join("/", pathNames));
    }
}

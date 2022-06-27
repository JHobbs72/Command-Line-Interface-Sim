using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CD : MonoBehaviour
{
    // Root command for 'change directory'
    // Take '..', '<path>' or '<directory>' and move to the appropriate directory

    public GraphManager fileSystem;
    private bool _validPath;
    private Node _localCurrentNode;
    private string _invalidNode;

    public void cd(string options)
    {
        if (options == "")
        {
            // Go to root
            fileSystem.SendOutput("");
        }
        
        _validPath = true;
        // Move 'back' a directory
        if (options == "..")
        {
            DirectoryNode lastNode = fileSystem.StepBackInPath();
            if (lastNode != null)
            {
                fileSystem.SetCurrentNode(lastNode);
                PrintPath(options);
            }
        }
        else
        {
            // Get component directories & check path validity
            string[] path = options.Split('/');
            _localCurrentNode = fileSystem.GetCurrentNode();

            // For each directory in path check if it exists in context
            foreach (string dir in path)
            {
                if (_validPath)
                {
                    _invalidNode = dir;
                    CheckNextStep((DirectoryNode)_localCurrentNode, dir);
                }
                else
                {
                    break;
                }
            }

            if (_validPath)
            {
                ExecutePathChange(fileSystem.GetCurrentNode(), path);
                PrintPath(options);
            }
            else
            {
                fileSystem.SendOutput("No directory named " + _invalidNode);
                Debug.Log("No directory named " + _invalidNode);
                _localCurrentNode = fileSystem.GetCurrentNode();
            }
        }
    }

    // Check the requested next directory exists
    private void CheckNextStep(DirectoryNode checkNode, string target)
    {
        List<Node> children = checkNode.getNeighbours();

        foreach (Node child in children)
        {
            if (child.name == target && child.GetType() == typeof(DirectoryNode))
            {
                _localCurrentNode = child;
                _validPath = true;
                break;
            }
            else
            {
                _validPath = false;
            }
        }
    }

    // Once valid, execute - go to valid file path
    private void ExecutePathChange(Node localCurrentNode, string[] targetPath)
    {
        foreach (string nextDir in targetPath)
        {
            List<Node> neighbours = localCurrentNode.getNeighbours();
            foreach (Node node in neighbours)
            {
                if (node.name == nextDir)
                {
                    localCurrentNode = node;
                    fileSystem.SetCurrentNode((DirectoryNode)node);
                    fileSystem.AddToCurrentPath((DirectoryNode)node);
                    break;
                }
            }
        }
    }

    // Display new path to user
    private void PrintPath(string options)
    {
        List<string> pathNames = new List<string>();
        foreach (Node dir in fileSystem.GetCurrentPath())
        {
            pathNames.Add(dir.name);
        }
        fileSystem.SendOutput(string.Join("/", pathNames));
        Debug.Log(">> " + string.Join("/", pathNames));
    }
}

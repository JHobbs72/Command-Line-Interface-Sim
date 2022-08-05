/*
 * Author: Jacob Hobbs - 190161842
 * Date : July 2022
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RMDIR : MonoBehaviour
{
    // Root command for 'remove directory' - Remove empty directory

    public GraphManager fileSystem;
    private char[] _options;

    public void rmdir(string input)
    {
        Tuple<List<char>, List<string>, List<Tuple<string, string>>> command = fileSystem.ValidateOptions(input, new[] {'p', 'v'}, "rmdir");

        _options = command.Item1.ToArray();
        List<string> arguments = command.Item2;
        
        // If no options, _options cannot be null for checks
        _options ??= new char[] { 'x' };
        
        // TODO --> if arguments is empty i.e. only options 
        
        // Iterate through arguments, check each exists
        foreach (string arg in arguments)
        {
            string[] splitArg = arg.Split('/');
            if (splitArg.Length > 1)
            {
                Tuple<List<Node>, string> toCheck = fileSystem.CheckPath(fileSystem.GetCurrentNode(), splitArg, 0, new List<Node>());
                List<Node> path = toCheck.Item1;
                
                if (toCheck.Item2 != null)
                {
                    // Error message already printed
                    return;
                }
                
                if (path[^1].GetType() == typeof(FileNode))
                {
                    fileSystem.SendOutput("rmdir: " + path[^1] + ": Not a directory", false);
                    return;
                }

                if (_options.Contains('p'))
                {
                    RemovePath(path, 1);
                }
                else
                {
                    RemoveDir((DirectoryNode)path[^2], path[^1].name);
                }
            }

            RemoveDir(fileSystem.GetCurrentNode(), splitArg[0]);
        }
    }

    // Removes all nodes in a path (if empty) start from the least node i.e. starts at end of path and works backwards
    // TODO Test
    private void RemovePath(List<Node> path, int step)
    {
        for (int i = step; i < path.Count - 1; i++)
        {
            if (path[^i].GetNeighbours().Count > 0)
            {
                fileSystem.SendOutput("rmdir: " + path[^i] + ": Directory not empty", false);
            }
            else
            {
                fileSystem.RemoveNode((DirectoryNode)path[^(i+1)], path[^i]);
            }
        }
        
        // Last node to remove (first in path)
        if (path[0].GetNeighbours().Count > 0)
        {
            fileSystem.SendOutput("rmdir: " + path[0] + ": Directory not empty", false);
        }
        else
        {
            fileSystem.RemoveNode(fileSystem.GetCurrentNode(), path[0]);
        }
    }

    // Method to remove a single directory
    private void RemoveDir(DirectoryNode parent, string target)
    {
        // Don't need to check if target is a file - done in main method
        Node targetNode = parent.SearchChildren(target);
        if (targetNode.GetNeighbours().Count > 0)
        {
            fileSystem.SendOutput("rmdir: " + targetNode.name + ": Directory not empty", false);
            return;
        }
        
        fileSystem.RemoveNode(parent, targetNode);
        fileSystem.SendOutput("", false);

        if (_options.Contains('v'))
        {
            fileSystem.SendOutput(targetNode.name, true);
        }
    }
}

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

    public void rmdir(string option)
    {
        if (option == "")
        {
            fileSystem.SendOutput("usage: rmdir [-p] directory ...", false);
            return;
        }
        
        Tuple<char[], string[]> commands = fileSystem.SeparateOptions(option, 2);
        _options = commands.Item1;
        string[] arguments = commands.Item2;
        
        // If no options, _options cannot be null for checks
        _options ??= new char[] { 'x' };
        
        // TODO --> if arguments is empty i.e. only options 
        
        foreach (string arg in arguments)
        {
            string[] splitArg = arg.Split('/');
            if (splitArg.Length > 1)
            {
                List<Node> path = fileSystem.CheckPath(fileSystem.GetCurrentNode(), splitArg, 0, new List<Node>());
                if (path == null)
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
                    RemovePath(fileSystem.GetCurrentNode(), path, 1);
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
    private void RemovePath(DirectoryNode lcn, List<Node> path, int step)
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

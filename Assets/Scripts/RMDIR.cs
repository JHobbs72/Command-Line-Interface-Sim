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
    private bool _pOption;
    private bool _vOption;
    private string _usage = "usage: rmdir [-pv] ...";
    private List<string> _toOutput;

    public void rmdir(string input)
    {
        _pOption = false; 
        _vOption = false;
        _toOutput = new List<string>();
        
        // No input
        if (string.IsNullOrEmpty(input))
        {
            fileSystem.SendOutput(_usage, false);
            return;
        }
        
        Tuple<List<char>, List<string>, List<Tuple<string, string>>> command = fileSystem.ValidateOptions(input, new[] {'p', 'v'}, "rmdir");
        
        // Illegal option
        if (command.Item3.Count > 0)
        {
            fileSystem.SendOutput("rmdir: " + command.Item3[0].Item2, false);
            return;
        }

        // No arguments
        if (command.Item2.Count < 1)
        {
            fileSystem.SendOutput(_usage, false);
            return;
        }

        // Set option booleans
        if (command.Item1.Contains('p'))
        {
            _pOption = true;
        }
        if (command.Item1.Contains('v'))
        {
            _vOption = true;
        }
        
        // Iterate through arguments, check each exists
        foreach (string arg in command.Item2)
        {
            string[] splitPath = arg.Split('/');
            if (splitPath.Length > 1)
            {
                Tuple<List<Node>, string> path = fileSystem.CheckPath(fileSystem.GetCurrentNode(), splitPath, 0, new List<Node>());
                
                if (path.Item2 != null)
                {
                    _toOutput.Add(path.Item2);
                    
                }
                else
                {
                    if (path.Item1[^1].GetType() == typeof(FileNode))
                    {
                        fileSystem.SendOutput("rmdir: " + path.Item1[^1].name + ": Not a directory", false);
                        return;
                    }

                    if (_pOption)
                    {
                        RemovePath(path.Item1);
                    }
                    else
                    {
                        RemoveDir((DirectoryNode)path.Item1[^2], path.Item1[^1].name);
                    }
                }
            }
            else
            {
                RemoveDir(fileSystem.GetCurrentNode(), splitPath[0]);
            }
        }
        
        fileSystem.SendOutput(string.Join('\n', _toOutput), false);
    }

    // Removes all nodes in a path (if empty) start from the least node i.e. starts at end of path and works backwards
    private void RemovePath(List<Node> path)
    {
        for (int i = path.Count - 1; i >= 0; i--)
        {
            if (path[i].GetNeighbours().Count > 0)
            {
                _toOutput.Add("rmdir: " + path[i].name + ": directory not empty");
                return;
            }
            
            fileSystem.RemoveNode(path[i].GetParent(), path[i]);
        }
    }

    // Method to remove a single directory
    private void RemoveDir(DirectoryNode parent, string target)
    {
        // Don't need to check if target is a file - done in main method
        Node targetNode = parent.SearchChildren(target);

        if (targetNode == null)
        {
            _toOutput.Add("rmdir: " + target + ": no such file or directory");
            return;
        }

        if (targetNode.GetType() == typeof(FileNode))
        {
            _toOutput.Add("rmdir: " + targetNode.name + ": not a directory");
            return;
        }
        
        if (targetNode.GetNeighbours().Count > 0)
        {
            _toOutput.Add("rmdir: " + targetNode.name + ": Directory not empty");
            return;
        }
        
        fileSystem.RemoveNode(parent, targetNode);

        if (_vOption)
        {
            fileSystem.SendOutput(targetNode.name, true);
            _toOutput.Insert(0, targetNode.name);
        }
    }
}

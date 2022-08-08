/*
 * Author: Jacob Hobbs - 190161842
 * Date : July 2022
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class MKDIR : MonoBehaviour
{
    // Root command for 'make directory' 
    
    public GraphManager fileSystem;
    private bool _pOption;
    private bool _vOption;
    private List<string> _toOutput;
    private string _usage = "usage: mkdir [-pv] directory ...";
    
    public void mkdir(string input)
    {
        _pOption = false;
        _vOption = false;
        
        // If no arguments given
        if (string.IsNullOrEmpty(input))
        {
            fileSystem.SendOutput(_usage);
            return;
        }
        
        Tuple<List<char>, List<string>, List<Tuple<string, string>>> command = fileSystem.ValidateOptions(input,
            new[] { 'p', 'v' }, "mkdir");
        
        List<char> options = command.Item1;
        List<string> arguments = command.Item2;
        List<Tuple<string, string>> caughtErrors = command.Item3;

        _toOutput = new List<string>();

        if (caughtErrors != null)
        {
            if (caughtErrors.Count > 0)
            {
                fileSystem.SendOutput(caughtErrors[0].Item2 + "\n" + _usage);
                return;
            }
        }

        // Remove illegal characters -- Abstraction --> '{' '}' and ',' are allowed in certain conditions.
            // For this purpose it's valid to remove them at all times  
        for (int i = 0; i < arguments.Count; i++)
        {
            arguments[i] = Regex.Replace(arguments[i], @"['\{},'-]+", "");
        }

        // Set option flags
        if (options != null)
        {
            if (options.Contains('p')) { _pOption = true; }
            if (options.Contains('v')) { _vOption = true; }
        }
        
        // If no arguments given
        if (arguments.Count == 0)
        {
            fileSystem.SendOutput(_usage);
            return;
        }

        if (_pOption)
        {
            foreach (string arg in arguments)
            {
                AddParentDirs(fileSystem.GetCurrentNode(), arg.Split('/'), 0);
            }
        }
        else
        {
            foreach (string arg in arguments)
            {
                List<string> path = arg.Split('/').ToList();

                if (path.Count > 1)
                {
                    Tuple<List<Node>, string> validPath = fileSystem.CheckPath(fileSystem.GetCurrentNode(), path.SkipLast(1).ToArray(), 0, new List<Node>());
                    
                    if (validPath.Item2 == null)
                    {
                        if (validPath.Item1[^1].GetType() == typeof(DirectoryNode))
                        {
                            AddDir((DirectoryNode)validPath.Item1[^1], path[^1]);
                        }
                        else
                        {
                            List<string> names = new List<string>();
                            foreach (Node node in validPath.Item1)
                            {
                                names.Add(node.name);
                            }
                            _toOutput.Add("mkdir: " + string.Join('/', names) + ": Not a directory");
                        }
                    }
                    else
                    {
                        _toOutput.Add(validPath.Item2);
                    }
                }
                else
                {
                    AddDir(fileSystem.GetCurrentNode(), path[0]);
                }
                
                
                
            }
        }
        
        fileSystem.SendOutput(string.Join('\n', _toOutput));
    }

    // Method to create a new directory
        // parent = the directory that the new directory will be a child of
        // newNode = the name of the directory to be created
    private DirectoryNode AddDir(DirectoryNode parent, string newNode)
    {
        List<Node> neighbours = parent.GetNeighbours();
        foreach (Node node in neighbours)
        {
            if (node.name == newNode)
            {
                // Check if a node with that name exists already
                _toOutput.Add("mkdir: " + newNode + ": File exists");
                return null;
            }
        }

        DirectoryNode created = fileSystem.AddDirectoryNode(parent, newNode);
        
        if (_vOption)
        {
            _toOutput.Add("mkdir: created directory '" + newNode + "'");
        }

        return created;
    }
    
    // Recursive method to create a directory from a path, including the creation of any directories that don't already
    // exist in the path given
        // lcn = (Local current node) the current directory being visited
        // path = The string path
        // step = The index of the path being visited 
    private void AddParentDirs(DirectoryNode lcn, string[] path, int step)
    {
        // Return if at the end of the path
        if (step == path.Length)
        {
            return;
        }
        
        // Iterate through the local current node's neighbours
        foreach (Node node in lcn.GetNeighbours())
        {
            // If the next node exists and is a directory node
            if (node.name == path[step] && node.GetType() == typeof(DirectoryNode))
            {
                // Recursive call to continue along the path
                AddParentDirs((DirectoryNode)node, path, step + 1);
            }
            // If the next node exists but is a file - error, invalid path
            else if (node.name == path[step] && node.GetType() == typeof(FileNode))
            {
                // TODO TEST -- check path.SkipLast(path.Length - step) --> should show path up to the point of error
                _toOutput.Add("mkdir: " + string.Join('/', path.SkipLast(path.Length - step)) + ": Not a directory");
                return;
            }
            // The next node doesn't exist, could be the last node in the path or an intermediate node that need to be
                // created before continuing
            else
            {
                // Recursive call to continue along the path
                AddParentDirs(AddDir((DirectoryNode)node, path[step]), path, step + 1);
                break;
            }
        }
    }
}

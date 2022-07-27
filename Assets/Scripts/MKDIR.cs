/*
 * Author: Jacob Hobbs - 190161842
 * Date : July 2022
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;

public class MKDIR : MonoBehaviour
{
    // Root command for 'make directory' 
    
    public GraphManager fileSystem;
    private bool _pOption;
    private bool _vOption;
    
    public void mkdir(string input)
    {
        Tuple<char[], string[]> command = fileSystem.SeparateAndValidate(input, "mkdir", new[] { 'p', 'v' }, 
            "usage: mkdir [-pv] directory ...");
        
        if (command == null) { return; }
        
        char[] charOptions = command.Item1;
        string[] arguments = command.Item2;
        
        // Remove illegal characters -- Abstraction --> '{' '}' and ',' are allowed in certain conditions. For this purpose it's valid to remove them at all times  
        for (int i = 0; i < arguments.Length; i++)
        {
            arguments[i] = Regex.Replace(arguments[i], @"['\{},']+", "");
        }

        // Set option flags
        if (charOptions != null)
        {
            if (charOptions.Contains('p')) { _pOption = true; }
            if (charOptions.Contains('v')) { _vOption = true; }
        }
        
        // If no arguments given
        if (arguments.Length == 0)
        {
            fileSystem.SendOutput("usage: mkdir [-pv] directory ...", false);
            return;
        }

        fileSystem.SendOutput("", false);
        
        // Iterate through each arguments
        foreach (string arg in arguments)
        {
            string[] path = arg.Split('/');

            if (path.Length > 1)
            {
                // If argument is a path
                if (_pOption)
                {
                    // If arguments is a path and option os given requiring any intermediate, non-existing directories to be created
                    AddParentDirs(fileSystem.GetCurrentNode(), path, 0);
                }
                else
                {
                    // Check the path, if valid create the dir at the end
                    List<Node> validPath = fileSystem.CheckPath(fileSystem.GetCurrentNode(), path.SkipLast(1).ToArray(), 0, new List<Node>());
                    if (validPath[^1].GetType() == typeof(DirectoryNode))
                    {
                        AddDir((DirectoryNode)validPath[^1], path[^1]);
                    }
                }
            }
            else
            {
                // If a single argument is given (not a path), create under current node
                AddDir(fileSystem.GetCurrentNode(), path[0]);
            }
        }
    }

    // Recursive method to create a directory from a path, including the creation of any directories that don't already exist in the path given
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
                fileSystem.SendOutput("mkdir: " + string.Join('/', path.SkipLast(path.Length - step)) + ": Not a directory", false);
                return;
            }
            // The next node doesn't exist, could be the last node in the path or an intermediate node that need to be created before continuing
            else
            {
                // Recursive call to continue along the path
                AddParentDirs(AddDir((DirectoryNode)node, path[step]), path, step + 1);
                break;
            }
        }
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
                fileSystem.SendOutput("mkdir: " + newNode + ": File exists", false);
                return null;
            }
        }

        DirectoryNode created = fileSystem.AddDirectoryNode(parent, newNode);
        if (_vOption)
        {
            fileSystem.SendOutput("mkdir: created directory '" + newNode + "'", false);
        }

        return created;
    }
}

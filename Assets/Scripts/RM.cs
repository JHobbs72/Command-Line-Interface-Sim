/*
 * Author: Jacob Hobbs - 190161842
 * Date : July 2022
 */

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class RM : MonoBehaviour
{
    // Root command for 'remove' - remove a node

    public GraphManager fileSystem;
    private char[] _options;

    public void rm(string input)
    {
        Tuple<List<char>, List<string>, List<Tuple<string, string>>> command = fileSystem.ValidateOptions(input, new[] {'f', 'i', 'r', 'v'}, "rm");
        
        _options = command.Item1.ToArray();
        List<string> arguments = command.Item2;

        // If options -i and -f are both given the second is used
        if (_options != null)
        {
            if (_options.Contains('i') && _options.Contains('f')) { _options = InteractAndForce(_options); }
        }
        
        // No options
        else { _options = new char[]{'x'}; }
        
        foreach (string arg in arguments)
        {
            string[] path = arg.Split('/');

            // Argument is path
            if (path.Length > 1)
            {
                // Check all nodes in path are valid
                Tuple<List<Node>, string> toCheck = fileSystem.CheckPath(fileSystem.GetCurrentNode(), path, 0, new List<Node>());
                List<Node> validPath = toCheck.Item1; 
                
                // Invalid path 
                if (toCheck.Item2 != null) { return; }
                
                // If path is valid and -r option used --> remove all nodes in path
                if (_options.Contains('r'))
                {
                    RecursiveRemove((DirectoryNode)validPath[^2], validPath[^1]);
                }
                
                // If valid path, no -r option and final node is a FileNode --> remove last node
                else if (validPath[^1].GetType() == typeof(FileNode))
                {
                    RemoveSingle((DirectoryNode)validPath[^2], validPath[^1]);
                }
                
                // If valid path, no -r option and final node is a DirectoryNode --> error
                else if (validPath[^1].GetType() == typeof(DirectoryNode)) { fileSystem.SendOutput("rm: " + validPath[^1] + ": is a directory", false); }
                
            }
            // Argument is a single Node 
            else
            {
                Node target = fileSystem.GetCurrentNode().SearchChildren(path[0]);
                // If it's a valid FileNode --> remove
                if (target.GetType() == typeof(FileNode))
                {
                    RemoveSingle(fileSystem.GetCurrentNode(), target);
                }
                else if (target.GetType() == typeof(DirectoryNode))
                {
                    // If it's a valid DirectoryNode and -r option used --> remove
                    if (_options.Contains('r'))
                    {
                        RecursiveRemove(fileSystem.GetCurrentNode(), target);
                    }
                    else
                    {
                        // Valid DirectoryNode but no -r option --> error
                        fileSystem.SendOutput("rm: " + target.name + ": is a directory", false);
                    }
                }
                else
                {
                    // Not a valid node
                    fileSystem.SendOutput("rm: " + path[0] + ": No such file or directory", false);
                }
            }
        }
        fileSystem.SendOutput("", false);
    }

    private void RemoveSingle(DirectoryNode parent, Node target)
    {
        // TODO if -i

        if (target.GetType() == typeof(DirectoryNode))
        {
            // If is directory that has children and -r option NOT applied
            if (target.GetNeighbours().Count > 0 && !_options.Contains('r'))
            {
                fileSystem.SendOutput("rm: " + target.name + ": is a directory", false);
            }
            // If is directory that has children and -r option IS applied
            else if (target.GetNeighbours().Count > 0 && _options.Contains('r'))
            {
                RecursiveRemove(parent, target);
            }
            // If is empty directory
            else
            {
                fileSystem.RemoveNode(parent, target);
                if (_options.Contains('v'))
                {
                    fileSystem.SendOutput(target.name, true);
                }
            }
        }
        // If is FileNode
        else if (target.GetType() == typeof(FileNode))
        {
            fileSystem.RemoveNode(parent, target);
            if (_options.Contains('v'))
            {
                fileSystem.SendOutput(target.name, true);
            }
        }
    }

    private void RecursiveRemove(DirectoryNode parent, Node current)
    {
        // If the argument is a FileNode
        if (current.GetType() == typeof(FileNode))
        {
            RemoveSingle(parent, current); return;
        }
        
        // TODO if -i

        // If the argument is a DirectoryNode
        List<Node> neighbours = current.GetNeighbours();

        // If the Directory has children --> Call RecursiveRemove on each child
        if (neighbours.Count > 0)
        {
            foreach (Node node in new List<Node>(neighbours))
            {
                RecursiveRemove((DirectoryNode)current, node);
            }
        }
        
        // If the directory has no children
        fileSystem.RemoveNode(parent, current);
        if (_options.Contains('v'))
        {
            fileSystem.SendOutput(current.name, true);
        }
    }

    // The order the options -i and -f are input changes which is used 
    private char[] InteractAndForce(char[] options)
    {
        int priority = Array.IndexOf(options, 'i') - Array.IndexOf(options, 'f');
        if (priority > 0)
        {
            // -i is after -f so use -i
            List<char> charList = options.ToList();
            charList.Remove('f');
            return charList.ToArray();
        }
        else
        {
            // -f is after -i so use -f
            List<char> charList = options.ToList();
            charList.Remove('i');
            return charList.ToArray();
        }
    }
}

/*
 * Author: Jacob Hobbs - 190161842
 * Date : July 2022
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MV : MonoBehaviour
{
    // Root command for 'move' - move a file from one directory to another
        // Can move, rename a file or overwrite one file with another

    public GraphManager fileSystem;
    private List<string> _toOutput;
    private bool _vOption;
    private const string Usage = "mv: not enough arguments" + "usage: mv [-v] source target \n" + "           mv [-v] source ... directory";

    public void mv(string input)
    {
        _toOutput = new List<string>();
        _vOption = false;
        
        Tuple<List<char>, List<string>, List<Tuple<string, string>>> command = fileSystem.ValidateOptions(input, new[] {'v'}, "mv");

        // Handles error on invalid option(s)
        if (command.Item3 != null)
        {
            fileSystem.SendOutput(command.Item3[0].Item2);
            return;
        }

        if (command.Item1.Contains('v'))
        {
            _vOption = true;
        }

        // Must have a source and a destination
        if (command.Item2.Count < 2)
        {
            fileSystem.SendOutput("mv: not enough arguments \n" + Usage);
            return;
        }

        // DESTINATION
        // Gets and checks the destination - deals with path and returns a tuple with one or both items null
            // If the destination exists it's returned as item1 (List to handle paths) and item2 is null
            // If the destination doesn't exist, item1 is null or list of path up to the penultimate node, item2 is the string name
            // If the path is invalid prints error and returns null
        Tuple<List<Node>, string> dest = GetDest(command.Item2[^1]);
        
        if (dest == null) { return; }

        // SOURCES
        Tuple<List<Node>, List<Tuple<string, string>>> sources = fileSystem.ValidateArgs(command.Item2, "mv");

        if (sources.Item2 != null)
        {
            fileSystem.SendOutput(sources.Item2[0].Item2);
            return;
        }
        
        // If there's more than one source --> must be wanting to move multiple directories to another directory
        if (sources.Item1.Count > 1)
        {
            // If the node doesn't exist
            if (dest.Item1 == null)
            {
                DirectoryNode finalDest = fileSystem.AddDirectoryNode(fileSystem.GetCurrentNode(), dest.Item2);
                Move(sources.Item1, finalDest);
                
            } // Node should be a directory
            else if (dest.Item1[^1].GetType() == typeof(FileNode))
            {
                fileSystem.SendOutput("mv: " + dest.Item1[^1].name + ": is not a directory");
                return;
            }
            else
            {
                // Call to move all the valid source nodes
                Move(sources.Item1, (DirectoryNode)dest.Item1[^1]);
            }
            
            fileSystem.SendOutput(string.Join('\n', _toOutput));
            return;
        }
        
        // If dest == name --> RENAME
        if (dest.Item2 != null)
        {
            Rename(sources.Item1[0], dest.Item2);
            fileSystem.SendOutput(string.Join('\n', _toOutput));
            return;
        }
        
        if (dest.Item1 != null)
        {
            // if source = <file> and dest = <dir> --> MOVE
            if (sources.Item1[0].GetType() == typeof(FileNode) && dest.Item1.GetType() == typeof(DirectoryNode))
            {
                Move(new List<Node> { sources.Item1[0] }, (DirectoryNode)dest.Item1[^1]);
                fileSystem.SendOutput(string.Join('\n', _toOutput));
                return;
            }
            
            // if source = <file> and dest = <file> --> OVERWRITE
            if (sources.Item1[0].GetType() == typeof(FileNode) && dest.Item1.GetType() == typeof(FileNode))
            {
                Overwrite((FileNode)sources.Item1[0], (FileNode)dest.Item1[^1]);
                fileSystem.SendOutput(string.Join('\n', _toOutput));
                return;
            }
            
            // if source = <dir> and dest = <dir> --> MOVE
            if (sources.Item1[0].GetType() == typeof(DirectoryNode) && dest.Item1[^1].GetType() == typeof(DirectoryNode))
            {
                Move(new List<Node> { sources.Item1[0] }, (DirectoryNode)dest.Item1[^1]);
                fileSystem.SendOutput(string.Join('\n', _toOutput));
            }
        }
    }

    // Method to return the destination node as tuple to signify if the destination is an existing node or the name of a node to be created
        // One of {<Node>, <name>} will be null, the other will be the value --> destination can be a string name (new node) or an existing node
    private Tuple<List<Node>, string> GetDest(string destination)
    {
        string[] dest = destination.Split('/');
        
        // If destination is a path
        if (dest.Length > 1)
        {
            // Check path
            Tuple<List<Node>, string> path = fileSystem.CheckPath(fileSystem.GetCurrentNode(), dest.SkipLast(1).ToArray(), 0, new List<Node>());
            // If the path is valid
            if (path.Item2 == null)
            {
                // Check last node in path
                DirectoryNode lastValid = (DirectoryNode)path.Item1[^1];
                Node destinationNode = lastValid.SearchChildren(dest[^1]);
                
                // If it doesn't exist --> is a name
                if (destinationNode == null)
                {
                    // Return valid part of path (all but last node) and the name of the 'new' node to be placed at the end 
                    return new Tuple<List<Node>, string>(path.Item1, dest[^1]);
                }

                // Full path is valid, return full path
                List<Node> finalPath = path.Item1;
                finalPath.Add(destinationNode);
                return new Tuple<List<Node>, string>(finalPath, null);
            }
            
            // Invalid path
            fileSystem.SendOutput(path.Item2);
            return null;
        }
        
        // If destination is a single node
        
        // Go up a directory
        if (dest[0] == "..")
        {
            return new Tuple<List<Node>, string>(new List<Node>(fileSystem.GetCurrentPath().SkipLast(1)), null);
        }
    
        Node validNode = fileSystem.GetCurrentNode().SearchChildren(dest[0]);
        
        // Check if destination exists and return the relevant tuple --> defines whether a valid destination has been input or a name
        return validNode == null ? new Tuple<List<Node>, string>(null, dest[0]) : 
            new Tuple<List<Node>, string>(new List<Node> { validNode }, null);
    }

    // Method to rename a node
    private void Rename(Node src, string dest)
    {
        if (_vOption)
        {
            _toOutput.Add(src.name + " --> " + dest);
        }
        
        src.name = dest;
    }

    // Method to overwrite one file with another
    private void Overwrite(FileNode src, FileNode dest)
    {
        if (_vOption)
        {
            _toOutput.Add(src.name + " --> " + dest.name);
        }
        
        // Dest takes contents of src, src removed
        dest.SetContents(src.GetContents());
        fileSystem.RemoveNode(src.GetParent(), src);
    }

    // Method to move multiple nodes to a new directory
    private void Move(List<Node> srcList, DirectoryNode dest)
    {
        foreach (Node src in srcList)
        {
            if (_vOption)
            {
                _toOutput.Add(src.name + " --> " + dest.name);
            }
            src.GetParent().RemoveNeighbour(src);
            src.SetParent(dest);
        }
    }
}

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

    public void mv(string input)
    {

        Tuple<char[], string[]> command = fileSystem.SeparateAndValidate(input, "mv", new[] {'f', 'i', 'n', 'v'}, 
            "usage: mv [-f | -i | -n] [-v] source target \n" +
            "           mv [-f | -i | -n] [-v] source ... directory");
        
        if (command == null) { return; }
        
        // Must have a source and a destination
        if (command.Item2.Length < 2)
        {
            fileSystem.SendOutput("usage: mv [-f | -i | -n] [-v] source target \n" +
                                  "           mv [-f | -i | -n] [-v] source ... directory", false);
            return;
        }

        // Gets and checks the destination - deals with path and returns a tuple with one or both items null
            // - If the node exists it's returned as item1 and item2 is null
            // If no node exists, item1 is null, item2 is the string name
        Tuple<Node, string> dest = GetDest(command.Item2[^1]);
        if (dest == null)
        {
            // Invalid destination
            // TODO error message
            fileSystem.SendOutput("Error in dest", false);
            return;
        }

        // All but last nodes are a source, check they exist -- 'GetValidSource' deals with paths
        List<Node> validSrcNodes = GetValidSource(command.Item2.SkipLast(1).ToArray());

        // If there's more than one source --> must be wanting to move multiple directories to another directory
        // TODO TEST -- move multiple FILES and directories?
        if (validSrcNodes.Count > 1)
        {
            // If the node doesn't exist
            if (dest.Item1 == null)
            {
                // TODO error message
                fileSystem.SendOutput("Not a directory 1", false);
                return;
            }
            // Node should be a directory
            if (dest.Item1.GetType() == typeof(FileNode))
            {
                // TODO error message
                fileSystem.SendOutput("Not a directory 2", false);
                return;
            }
            
            // Call to move all the valid source nodes
            Move(validSrcNodes, (DirectoryNode)dest.Item1);
            return;
        }
        
        // If dest == name --> RENAME
        if (dest.Item2 != null)
        {
            Rename(validSrcNodes[0], dest.Item2);
            return;
        }
        
        if (dest.Item1 != null)
        {
            // if source = <file> and dest = <dir> --> MOVE
            if (validSrcNodes[0].GetType() == typeof(FileNode) && dest.Item1.GetType() == typeof(DirectoryNode))
            {
                Move(new List<Node> { validSrcNodes[0] }, (DirectoryNode)dest.Item1);
                return;
            }
            
            // if source = <file> and dest = <file> --> OVERWRITE
            if (validSrcNodes[0].GetType() == typeof(FileNode) && dest.Item1.GetType() == typeof(FileNode))
            {
                Overwrite((FileNode)validSrcNodes[0], (FileNode)dest.Item1);
                return;
            }
            
            // if source = <dir> and dest = <dir> --> MOVE
            if (validSrcNodes[0].GetType() == typeof(DirectoryNode) && dest.Item1.GetType() == typeof(DirectoryNode))
            {
                Move(new List<Node> { validSrcNodes[0] }, (DirectoryNode)dest.Item1);
            }
        }
    }

    // Method to return the destination node as tuple to signify if the destination is an existing node or the name of a node to be created
        // One of {<Node>, <name>} will be null, the other will be the value --> destination can be a string name (new node) or an existing node
    private Tuple<Node, string> GetDest(string destination)
    {
        string[] dest = destination.Split('/');
        if (dest.Length > 1)
        {
            // Check path
            List<Node> destPath = fileSystem.CheckPath(fileSystem.GetCurrentNode(), dest, 0, new List<Node>());
            if (destPath == null)
            {
                List<Node> testPath = fileSystem.CheckPath(fileSystem.GetCurrentNode(), dest.SkipLast(1).ToArray(),
                    0, new List<Node>());
                if (testPath == null)
                {
                    // TODO error message
                    fileSystem.SendOutput("Error - invalid path", false);
                    return null;
                }
                return new Tuple<Node, string>(null, dest[^1]);
            }
            
            return new Tuple<Node, string>(destPath[^1], null);
            
        }
        
        // Go up a directory
        if (dest[0] == "..")
        {
            return new Tuple<Node, string>(fileSystem.GetCurrentPath()[^2], null);
        }
    
        // If not a path
        Node validNode = fileSystem.GetCurrentNode().SearchChildren(dest[0]);
        
        // Check if exists and return the relevant tuple
        return validNode == null ? new Tuple<Node, string>(null, dest[0]) : 
            new Tuple<Node, string>(validNode, null);
    }

    // Method to take in a list of potential file or directory names/paths and check if they exist - return all that exist, skip those that don't
    private List<Node> GetValidSource(string[] arguments)
    {
        List<Node> validSrcNodes = new List<Node>();
        
        foreach (string arg in arguments)
        {
            string[] path = arg.Split('/');

            if (path.Length > 1)
            {
                // Check path
                List<Node> validPath = fileSystem.CheckPath(fileSystem.GetCurrentNode(), path, 0, new List<Node>());
                if (validPath == null)
                {
                    // TODO error message
                    fileSystem.SendOutput("Invalid source 1", false);
                }
                else
                {
                    validSrcNodes.Add(validPath[^1]);
                }
            }
            else
            {
                // Not a path, check and add if exists
                Node target = fileSystem.GetCurrentNode().SearchChildren(path[0]);
                if (target == null)
                {
                    // TODO error message
                    fileSystem.SendOutput("Invalid source 2", false);
                }
                else
                {
                    validSrcNodes.Add(target);
                }
            }
        }
        
        return validSrcNodes;
    }

    // Method to rename a node
    private void Rename(Node src, string dest)
    {
        src.name = dest;
        
        fileSystem.SendOutput("", false);
    }

    // Method to overwrite one file with another
    // TODO reset contents?
    private void Overwrite(FileNode src, FileNode dest)
    {
        // Dest takes contents of src, src removed
        dest.SetContents(src.GetContents());
        fileSystem.RemoveNode(src.GetParent(), src);
        
        fileSystem.SendOutput("", false);
    }

    // Method to move multiple nodes to a new directory
    private void Move(List<Node> srcList, DirectoryNode dest)
    {

        foreach (Node src in srcList)
        {
            src.GetParent().RemoveNeighbour(src);
            src.SetParent(dest);
        }
        
        fileSystem.SendOutput("", false);
    }
}

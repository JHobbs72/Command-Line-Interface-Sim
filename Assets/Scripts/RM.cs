/*
 * Author: Jacob Hobbs - 190161842
 * Date : July 2022
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RM : MonoBehaviour
{
    // Root command for 'remove' - remove a file

    public GraphManager fileSystem;

    public void rm(string option)
    {
        if (option == "")
        {
            fileSystem.SendOutput("usage: rm [-f | -i] [-rv] file ... \n           unlink file");
            return;
        }
        
        // -f | -i --> if used together (pointless) the second takes precedence
        // -rv     --> both execute individually

        // -------------------------------------------------------------------------------------------------------------
        DirectoryNode currentNode = fileSystem.GetCurrentNode();
        List<Node> neighbours = currentNode.GetNeighbours();
        
        // ------------------------------------------------------------------------------------------------------------- 

        // Separate '-x' options and remaining commands
            // MaxLength = '-x -x -x -x'
        Tuple<char[], string[]> commands = fileSystem.SeparateOptions(option, 4);
        char[] options = commands.Item1;
        string[] remCommands = commands.Item2;
        
        Debug.Log("Options: " + String.Join(',', remCommands));
        
        // LIST OR SINGLE FILE/DIRECTORY
            // Skip over any invalid files/dirs - Don't break on error, just skip onto the next
        // Init empty list to fill with valid nodes that will be removed
        List<Node> toRemove = new List<Node>();
        // Iterate through each file/dir given in command (may be just one)
        foreach (string file in remCommands)
        {
            bool fileFound = false;
            // Attempt to find given node in neighbours of current node
            foreach (Node node in neighbours)
            {
                // If what's given is a valid directory
                if (node.name == file && node.GetType() == typeof(DirectoryNode))
                {
                    // If it's a directory the '-r' option must be included
                    if (options == null || !options.Contains('r'))
                    {
                        // Error
                        fileSystem.SendOutput("rm: " + file + ": is a directory");
                    }
                    // Add to list of nodes to be removed
                    toRemove.Add(node);
                    fileFound = true;
                }
                // If what's given is a valid file 
                else if (node.name == file && node.GetType() == typeof(FileNode))
                {
                    // Add to list of nodes to be removed
                    toRemove.Add(node);
                    fileFound = true;
                }
            }

            if (!fileFound)
            {
                // Error on invalid input
                fileSystem.SendOutput("rm: " + file + ": No such file or directory");
            }
        }

        // IF options == null CATCH
        foreach (Node node in toRemove)
        {
            if (node.GetType() == typeof(FileNode))
            {
                // FileNode
                RemoveSingle(currentNode, (FileNode)node, options);
            }
            else if (node.GetType() == typeof(DirectoryNode) && node.GetNeighbours().Count == 0)
            {
                // DirectoryNode with no children
                RemoveSingle(currentNode, (FileNode)node, options);
            }
            else
            {
                // DirectoryNode with children
                RemoveTree(currentNode, (DirectoryNode)node, options);
            }
        }
    }
    
    // -----------------------------------------------------------------------------------------------------------------

    
    // RemoveSingle must be identified by number of neighbours, not 'is FileNode' else directories will never be removed 
    
    
    private void RemoveSingle(DirectoryNode currentNode, Node target, char[] opts)
    {
        // TODO opts
        fileSystem.RemoveNode(currentNode, target);
        fileSystem.SendOutput("");
    }

    private void RemoveTree(DirectoryNode localCurrentNode, DirectoryNode target, char[] opts)
    {
        // TODO opts
        Debug.Log("LCN: " + localCurrentNode + "\nTarget: " + target + "\nOptions: " + opts);
        List<Node> neighbours = localCurrentNode.GetNeighbours();
        foreach (Node node in neighbours)
        {
            if (node.name == target.name)
            {
                if (node.GetType() == typeof(FileNode))
                {
                    RemoveSingle(localCurrentNode, node, opts);
                }
                else
                {
                    RemoveTree(localCurrentNode, (DirectoryNode)node, opts);
                }
            }
        }
    }
}

using System;
using System.Collections;
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
        bool found = false;
        bool isDirectory = false;
        // ------------------------------------------------------------------------------------------------------------- 

        string[] optionComponents = option.Split(' ');
        List<string> opts = new List<string>();
        // null if no options given, else character array of individual options
        char[] finalOpts = null;

        // OPTIONS DETECTION AND SEPARATION
        // Does the command contain at least one option?
        if (optionComponents[0].StartsWith('-'))
        {
            // If yes add to options list and remove from rest of the command
            opts.Add(optionComponents[0]);
            optionComponents = optionComponents.Skip(1).ToArray();
            
            // 'Longest' combination of options = '-x -x -x -x' with no file in between 
            for (int i = 0; i < 3; i++)
            {
                // Is the next part within given range an option?
                if (optionComponents[0].StartsWith('-'))
                {
                    // If yes add to options list and remove from rest of the command
                    opts.Add(optionComponents[0]);
                    optionComponents = optionComponents.Skip(1).ToArray();
                }
            }

            // Convert to string, remove '-' characters then convert to character array
            string stringOpts = opts.ToString();
            stringOpts = stringOpts.Trim(new [] { '-' });
            finalOpts = stringOpts.ToCharArray();
        }
        
        // LIST OR SINGLE FILE/DIRECTORY
            // Skip over any invalid files/dirs
        // Iterate through each file/dir given in command (may be just one)
        foreach (string file in optionComponents)
        {
            // Attempt to find given node in neighbours of current node
            foreach (Node node in neighbours)
            {
                // If what's given is a valid directory
                if (node.name == file && node.GetType() == typeof(DirectoryNode))
                {
                    // If it's a directory the '-r' option must be included
                    if (finalOpts == null || !finalOpts.Contains('r'))
                    {
                        // Error
                        fileSystem.SendOutput("rm: " + file + ": is a directory");
                    }
                    // Call to remove this directory and all nested files and directories
                    RemoveTree(currentNode, (DirectoryNode)node, finalOpts);
                }
                // If what's given is a valid file 
                else if (node.name == file && node.GetType() == typeof(FileNode))
                {
                    // Call to remove single file
                    RemoveSingle(currentNode, (FileNode)node, finalOpts);
                }
                else
                {
                    // Error on invalid input -- No break, just skip onto the next
                    fileSystem.SendOutput("rm: " + file + ": No such file or directory");
                }
            }
        }
        
        
        
        // -------------------------------------------------------------------------------------------------------------
        
    //     foreach (Node targetNode in neighbours)
    //     {
    //         if (targetNode.name == option && targetNode.GetType() == typeof(FileNode))
    //         {
    //             fileSystem.RemoveNode(currentNode, targetNode);
    //             found = true;
    //             fileSystem.SendOutput("");
    //             break;
    //         }
    //
    //         if (targetNode.name == option && targetNode.GetType() == typeof(DirectoryNode))
    //         {
    //             isDirectory = true;
    //         }
    //     }
    //
    //     if (!found && isDirectory)
    //     {
    //         fileSystem.SendOutput("rm: " + option + ": is a directory");
    //     }
    //     else if (!found)
    //     {
    //         fileSystem.SendOutput("rm: " + option + ": No such file or directory");
    //     }
    }
    
    // -----------------------------------------------------------------------------------------------------------------

    private void RemoveSingle(DirectoryNode currentNode, FileNode target, char[] opts)
    {
        // TODO opts
        fileSystem.RemoveNode(currentNode, target);
        fileSystem.SendOutput("");
    }

    private void RemoveTree(DirectoryNode currentNode, DirectoryNode target, char[] opts)
    {
        // TODO opts
        List<Node> neighbours = currentNode.GetNeighbours();
        foreach (Node node in neighbours)
        {
            if (node.name == target.name)
            {
                if (node.GetType() == typeof(FileNode))
                {
                    RemoveSingle(currentNode, (FileNode)node, opts);
                }
                else
                {
                    RemoveTree(currentNode, (DirectoryNode)node, opts);
                }
            }
        }
    }
}

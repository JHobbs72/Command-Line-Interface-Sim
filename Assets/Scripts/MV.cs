using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MV : MonoBehaviour
{
    // Root command for 'move' - move a file from one directory to another

    public GraphManager fileSystem;

    public void mv(string options)
    {
        string[] optionsString = options.Split(' ');
        string mvOpt = null;
        string[] sourceList = null;
        string source = null;
        string dest = null;

        // Separate '-x' option, source and destination
        if (optionsString[0].Contains('-') && optionsString[0].Length == 2)
        {
            // Variable for '-x' option
            mvOpt = optionsString[0];
            // Remove this from the command
            optionsString = optionsString.Skip(1).ToArray();
        }
        // Destination (name, file, dir or path)
        dest = optionsString[optionsString.Length - 1];
        // Remaining must be the source
        sourceList = optionsString.SkipLast(1).ToArray();
        // If source is not list use just single string
        if (sourceList.Length == 1)
        {
            source = sourceList[0];
            sourceList = null;
        }
        
        
        // if list of files & directories to be moved to valid directory 
        if (sourceList != null && validPath(dest) == 1)
        {
            // Get list of nodes as source
            Node[] nodeList = validSrcList(sourceList);
            // If sourceList contains a node that doesn't exist -> returns null
            if (nodeList != null)
            {
                // Move each node in source
                foreach (Node node in nodeList)
                {
                    move(mvOpt, node, dest);
                }
            }
        }
        
        // if is single source & source ends with '/' and source is a directory node
        // destination must be name to rename directory source 
        if (source != null && source.EndsWith('/'))
        {
            // Remove '/' from name
            source.TrimEnd('/');
            if (exists(source).GetType() == typeof(DirectoryNode))
            {
                rename(mvOpt, exists(source), dest);
            }
        }
        
        
    }

    private Node exists(string target)
    {
        Node doesExist = null;
        List<Node> neighbours = fileSystem.getCurrentNode().getNeighbours();
        foreach (Node node in neighbours)
        {
            if (node.name == target)
            {
                doesExist = node;
            }
        }

        return doesExist;
    }

    private Node[] validSrcList(string[] targets)
    {
        // If going to return null (invalid source list) throw error here to display which is causing the problem
    }

    // 1 -> valid directory
    // 0 -> valid file
    // -1 -> doesn't exist
    // Helper to check next in path
    private int validDest(string dest)
    {
        
    }

    // -1 -> invalid path
    // 0 -> valid bar last 
    // 1 -> fully valid
    // Will check single dir or full path
    private int validPath(string destPath)
    {
        // split on '/'
        // foreach in list send to validDest
    }

    private void rename(string mvOpt, Node src, string dest)
    {
        
    }

    private void overwrite(string mvOpt, Node src, Node dest)
    {
        
    }

    private void move(string mvOpt, Node src, string dest)
    {
        
    }
}

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
        Debug.Log("In MOVE");
        // TODO must be at least 2 long?
        string[] optionsString = options.Split(' ');
        string mvOpt = null;
        string[] sourceList = null;
        string source = null;
        string dest = null;

        if (options == "")
        {
            fileSystem.SendOutput("usage: mv [-f | -i | -n] [-v] source target \n" +
                                  "           mv [-f | -i | -n] [-v] source ... directory");
            return;
        }

        // Separate '-x' option, source and destination
        if (optionsString[0].Contains('-') && optionsString[0].Length == 2)
        {
            // Variable for '-x' option
            mvOpt = optionsString[0];
            // Remove this from the command
            optionsString = optionsString.Skip(1).ToArray();
        }
        // Destination is last element (name, file, dir or path)
        dest = optionsString[optionsString.Length - 1];
        // Remaining must be the source
        sourceList = optionsString.SkipLast(1).ToArray();
        // If source is not list (case 1 only) use just single string
        if (sourceList.Length == 1)
        {
            source = sourceList[0];
            sourceList = null;
        }
        
        // --------------
        // 1
        // if list of files & directories to be moved to valid directory 
        if (sourceList != null && validPath(dest) == 1)
        {
            // Get list of nodes as source
            Node[] nodeList = validSrcList(fileSystem.GetCurrentNode(), sourceList);
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
        
        // --------------
        // 2
        // if is single source & source ends with '/' and source is a directory node
        // destination must be name to rename directory source 
        if (source != null && source.EndsWith('/'))
        {
            // Remove '/' from name
            source.TrimEnd('/');
            if (exists(fileSystem.GetCurrentNode(), source).GetType() == typeof(DirectoryNode))
            {
                rename(mvOpt, exists(fileSystem.GetCurrentNode(), source), dest);
            }
        }
        
        // --------------
        // 3
        // Renaming a file
        // if source is a file & exists && dest doesn't exist
        // error if new name includes '/' 
        if (source != null)
        {
            if (exists(fileSystem.GetCurrentNode(), source).GetType() == typeof(FileNode) &&
                exists(fileSystem.GetCurrentNode(), dest) == null)
            {
                rename(mvOpt, exists(fileSystem.GetCurrentNode(), source), dest);
            }
        }
        
        // --------------
        // 4
        // Overwrite within current dir
        // if both source and dest are valid files
        if (source != null)
        {
            if (exists(fileSystem.GetCurrentNode(), source).GetType() == typeof(FileNode) &&
                exists(fileSystem.GetCurrentNode(), dest).GetType() == typeof(FileNode))
            {
                overwrite(mvOpt, exists(fileSystem.GetCurrentNode(), source), 
                    exists(fileSystem.GetCurrentNode(), dest));
            }
        }
        
        // --------------
        // 5
        // Move or overwrite
        // if source is valid file and dest is valid path to end
        if (source != null)
        {
            Node srcNode = exists(fileSystem.GetCurrentNode(), source);
            bool movetoDest = false;
            if (srcNode.GetType() == typeof(FileNode) && validPath(dest) == 1)
            {
                // does this work?
                string[] separatedPath = dest.Split('/');

                // recursively call searchNeighbours to follow path and get last node
                DirectoryNode endNode = searchNeighbours((DirectoryNode)srcNode, 0, separatedPath);

                foreach (Node node in endNode.getNeighbours())
                {
                    if (node.name == dest)
                    {
                        //move
                        movetoDest = true;
                        move(mvOpt, srcNode, endNode.name);
                        break;
                    }
                }

                if (movetoDest == false)
                {
                    // overwrite
                    overwrite(mvOpt, srcNode, endNode);
                }
            }
        }
        
        // --------------
        // 6
        // move valid file or dir to new dir
        if (source != null)
        {
            Node srcNode = exists(fileSystem.GetCurrentNode(), source);
            if ((srcNode.GetType() == typeof(DirectoryNode) || srcNode.GetType() == typeof(FileNode)) &&
                validPath(dest) == 0)
            {
                string[] path = dest.Split('/');
                DirectoryNode newDir = DirectoryNode.Create<DirectoryNode>(path[path.Length - 1]);
                // Add newDir to relevant neighbours list
                move(mvOpt, srcNode, newDir.name);
                
            }
        }
    }

    private DirectoryNode searchNeighbours(DirectoryNode dir, int item, string[] path)
    {
        DirectoryNode found = null;
        List<Node> currentNeighbours = dir.getNeighbours();
        foreach (Node node in currentNeighbours)
        {
            if (node.name == path[item] && node.GetType() == typeof(DirectoryNode))
            {
                found = (DirectoryNode)node;
                break;
            }
        }

        if (item < path.Length)
        {
            found = searchNeighbours(found, item + 1, path);
        }

        return found;
    }

    // Pass single string (target) as 'node' to be checked under what 'directory node' (searchArea)
    // Returns the node if it exists else returns null
    private Node exists(DirectoryNode searchArea, string target)
    {
        Node doesExist = null;
        List<Node> neighbours = searchArea.getNeighbours();
        foreach (Node node in neighbours)
        {
            if (node.name == target)
            {
                doesExist = node;
            }
        }

        return doesExist;
    }

    // Pass string array to check that each is a valid node
    // return array of nodes if all are valid, else throw error on the first node that doesn't exist
    private Node[] validSrcList(DirectoryNode searchArea, string[] targets)
    {
        // Will call exists(searchArea, target[0,1,...])
        // If going to return null (invalid source list) throw error here to display which is causing the problem
        return null;
    }

    // Needed?
    // 1 -> valid directory
    // 0 -> valid file
    // -1 -> doesn't exist
    // Helper to check next in path
    private int validDest(string dest)
    {
        return 0;
    }

    // -1 -> invalid path
    // 0 -> valid bar last 
    // 1 -> fully valid
    // Will check single dir or full path
    private int validPath(string destPath)
    {
        // split on '/'
        // foreach in list send to validDest
        return 0;
    }

    private void rename(string mvOpt, Node src, string dest)
    {
        // Cannot have '-x' options i.e. mvOpt should be null
    }

    private void overwrite(string mvOpt, Node src, Node dest)
    {
        
    }

    private void move(string mvOpt, Node src, string dest)
    {
        
    }
}

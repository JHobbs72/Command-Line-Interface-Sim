using System;
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
        dest = optionsString[^1];
        // Remaining must be the source
        sourceList = optionsString.SkipLast(1).ToArray();
        // If source is not list (case 1 only) use single string
        if (sourceList.Length == 1)
        {
            source = sourceList[0];
            sourceList = null;
        }
        
        Debug.Log("Option: " + mvOpt + "\nSources: " + string.Join(',', sourceList) + "\nDestination: " + dest);
        Debug.Log(ValidPath(dest));
        
        // -------------------------------------------------------------------------------------------------------------
        // 1
        // if list of valid files &/or directories to be moved to valid directory
        List<Node> nodeList = ValidSrcList(fileSystem.GetCurrentNode(), sourceList);
        if (nodeList != null)
        {
            Node validity = ValidPath(dest);
            if (validity.GetType() == typeof(DirectoryNode))
            {
                // If sourceList contains a node that doesn't exist -> returns null
                foreach (Node node in nodeList)
                {
                    Move(mvOpt, node, dest);
                }
            } else if (validity.GetType() == typeof(FileNode))
            {
                // Add invalid file
                fileSystem.SendOutput("mv: " + validity.name + " is not a directory");
            }
        }
        
        // -------------------------------------------------------------------------------------------------------------
        // 2
        // if is single source & source ends with '/' and source is a directory node
        // destination must be name to rename directory source 
        if (source != null && source.EndsWith('/'))
        {
            // Remove '/' from name
            source.TrimEnd('/');
            if (Exists(fileSystem.GetCurrentNode(), source).GetType() == typeof(DirectoryNode))
            {
                Rename(mvOpt, Exists(fileSystem.GetCurrentNode(), source), dest);
            }
        }
        
        // -------------------------------------------------------------------------------------------------------------
        // 3
        // Renaming a file
        // if source is a file & exists && dest doesn't exist
        // error if new name includes '/' 
        if (source != null)
        {
            if (Exists(fileSystem.GetCurrentNode(), source).GetType() == typeof(FileNode) &&
                Exists(fileSystem.GetCurrentNode(), dest) == null)
            {
                Rename(mvOpt, Exists(fileSystem.GetCurrentNode(), source), dest);
            }
        }
        
        // -------------------------------------------------------------------------------------------------------------
        // 4
        // Overwrite within current dir
        // if both source and dest are valid files
        if (source != null)
        {
            if (Exists(fileSystem.GetCurrentNode(), source).GetType() == typeof(FileNode) &&
                Exists(fileSystem.GetCurrentNode(), dest).GetType() == typeof(FileNode))
            {
                Overwrite(mvOpt, Exists(fileSystem.GetCurrentNode(), source), 
                    Exists(fileSystem.GetCurrentNode(), dest));
            }
        }
        
        // -------------------------------------------------------------------------------------------------------------
        // 5
        // Move or overwrite
        // if source is valid file and dest is valid path to end
        if (source != null)
        {
            Node srcNode = Exists(fileSystem.GetCurrentNode(), source);
            bool movetoDest = false;
            if (srcNode.GetType() == typeof(FileNode) && ValidPath(dest).GetType() == typeof(DirectoryNode))
            {
                // does this work?
                string[] separatedPath = dest.Split('/');

                // recursively call searchNeighbours to follow path and get last node
                DirectoryNode endNode = SearchNeighbours((DirectoryNode)srcNode, 0, separatedPath);

                foreach (Node node in endNode.GetNeighbours())
                {
                    if (node.name == dest)
                    {
                        //move
                        movetoDest = true;
                        Move(mvOpt, srcNode, endNode.name);
                        break;
                    }
                }

                if (movetoDest == false)
                {
                    // overwrite
                    Overwrite(mvOpt, srcNode, endNode);
                }
            }
        }

        // -------------------------------------------------------------------------------------------------------------
        // 6
        // move valid file or dir to new dir
        if (source != null)
        {
            Node srcNode = Exists(fileSystem.GetCurrentNode(), source);
            if ((srcNode.GetType() == typeof(DirectoryNode) || srcNode.GetType() == typeof(FileNode)) &&
                ValidPath(dest).GetType() == typeof(FileNode))
            {
                string[] path = dest.Split('/');
                DirectoryNode newDir = Node.Create<DirectoryNode>(path[^1]);
                // Add newDir to relevant neighbours list
                Move(mvOpt, srcNode, newDir.name);
                
            }
        }
    }
        // -------------------------------------------------------------------------------------------------------------

    private DirectoryNode SearchNeighbours(DirectoryNode dir, int item, string[] path)
    {
        DirectoryNode found = null;
        List<Node> currentNeighbours = dir.GetNeighbours();
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
            found = SearchNeighbours(found, item + 1, path);
        }

        return found;
    }

    // Pass single string (target) as 'node' to be checked under what 'directory node' (searchArea)
    // Returns the node if it exists else returns null
    private Node Exists(DirectoryNode searchArea, string target)
    {
        Node doesExist = null;
        List<Node> neighbours = searchArea.GetNeighbours();
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
    // return Node array if all are valid, else return null & throw error on the first node that doesn't exist
    private List<Node> ValidSrcList(DirectoryNode searchArea, string[] targets)
    {
        if (targets == null || searchArea == null)
        {
            return null;
        }
        
        List<Node> validNodes = new List<Node>();
        foreach (string node in targets)
        {
            Node thisNode = Exists(searchArea, node);
            if (thisNode != null)
            {
                validNodes.Add(thisNode);
            }
            else
            {
                // Error, non-existent node
                fileSystem.SendOutput(thisNode.name + ": no such file or directory");
                return null;
            }
        }

        return validNodes;
        // Check not null
        // Will call exists(searchArea, target[0,1,...])
        // If going to return null (invalid source list) throw error here to display which is causing the problem
    }

    // Needed?
    // 1 -> valid directory
    // 0 -> valid file
    // -1 -> doesn't exist
    // Helper to check next in path
    private Node ValidDest(DirectoryNode searchArea, string dest)
    {
        List<Node> neighbours = searchArea.GetNeighbours();

        foreach (Node node in neighbours)
        {
            if (node.name == dest)
            {
                return node;
            }
        }
        
        return null;
    }
    private Node ValidPath(string destPath)
    {
        string[] pathElements = destPath.Split('/');

        return NextStep(fileSystem.GetCurrentNode(), pathElements, 0);

    }

    private Node NextStep(DirectoryNode searchArea, string[] path, int index)
    {
        Node validity = ValidDest(searchArea, path[index]);
        Debug.Log(validity);
        
        if (validity == null)
        {
            // Invalid node
            fileSystem.SendOutput(path[index] + ": no such file or directory");
            return null;
        }
        
        if (validity.GetType() == typeof(DirectoryNode))
        {
            if (index == path.Length)
            {
                // All nodes valid, last node = Directory Node
                return validity;
            }
            NextStep((DirectoryNode)validity, path, index++);
        }
        else if (validity.GetType() == typeof(FileNode))
        {
            // All nodes valid, last node = File Node
            return validity;
        }

        return validity;
    }
    
    private void Rename(string mvOpt, Node src, string dest)
    {
        // Cannot have '-x' options i.e. mvOpt should be null
        fileSystem.SendOutput("RENAMING");
    }

    private void Overwrite(string mvOpt, Node src, Node dest)
    {
        fileSystem.SendOutput("OVERWRITING");
        // Moving a file to where there's a duplicate overwrites the old file
    }

    private void Move(string mvOpt, Node src, string dest)
    {
        // Check for duplicates at destination --> overwrite
        fileSystem.SendOutput("MOVING");
    }
}

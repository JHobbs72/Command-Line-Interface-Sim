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

    public GraphManager fileSystem;

    public void mv(string options)
    {
        if (options == "")
        {
            fileSystem.SendOutput("usage: mv [-f | -i | -n] [-v] source target \n" +
                                  "           mv [-f | -i | -n] [-v] source ... directory", false);
            return;
        }

        Tuple<char[], string[]> command = fileSystem.SeparateOptions(options, 4);

        if (command.Item2.Length < 2)
        {
            fileSystem.SendOutput("usage: mv [-f | -i | -n] [-v] source target \n" +
                                  "           mv [-f | -i | -n] [-v] source ... directory", false);
            return;
        }

        Tuple<Node, string> dest = GetDest(command.Item2[^1]);
        if (dest == null)
        {
            fileSystem.SendOutput("Error in dest", false);
            return;
        }

        List<Node> validSrcNodes = GetValidSource(command.Item2.SkipLast(1).ToArray());

        if (validSrcNodes.Count > 1)
        {
            if (dest.Item1 == null)
            {
                fileSystem.SendOutput("Not a directory 1", false);
                return;
            }

            if (dest.Item1.GetType() == typeof(FileNode))
            {
                fileSystem.SendOutput("Not a directory 2", false);
                return;
            }
            
            Move(validSrcNodes, (DirectoryNode)dest.Item1);
            return;
        }
        
        //if dest == name --> RENAME
        if (dest.Item2 != null)
        {
            Rename(validSrcNodes[0], dest.Item2);
            return;
        }
        
        if (dest.Item1 != null)
        {
            // if <file> and <dir> --> MOVE
            if (validSrcNodes[0].GetType() == typeof(FileNode) && dest.Item1.GetType() == typeof(DirectoryNode))
            {
                Move(new List<Node> { validSrcNodes[0] }, (DirectoryNode)dest.Item1);
                return;
            }
            
            // if <file> and <file> --> OVERWRITE
            if (validSrcNodes[0].GetType() == typeof(FileNode) && dest.Item1.GetType() == typeof(FileNode))
            {
                Overwrite((FileNode)validSrcNodes[0], (FileNode)dest.Item1);
                return;
            }
            
            // if <dir> and <dir> --> MOVE
            if (validSrcNodes[0].GetType() == typeof(DirectoryNode) && dest.Item1.GetType() == typeof(DirectoryNode))
            {
                Move(new List<Node> { validSrcNodes[0] }, (DirectoryNode)dest.Item1);
            }
        }
    }

    // Returns destination as tuple
    // one of {<Node>(<dir> or <file>), <name>}
    private Tuple<Node, string> GetDest(string destination)
    {
        string[] dest = destination.Split('/');
        if (dest.Length > 1)
        {
            List<Node> destPath = fileSystem.CheckPath(fileSystem.GetCurrentNode(), dest, 0, new List<Node>());
            if (destPath == null)
            {
                List<Node> testPath = fileSystem.CheckPath(fileSystem.GetCurrentNode(), dest.SkipLast(1).ToArray(),
                    0, new List<Node>());
                if (testPath == null)
                {
                    fileSystem.SendOutput("Error - invalid path", false);
                    return null;
                }
                return new Tuple<Node, string>(null, dest[^1]);
            }
            
            return new Tuple<Node, string>(destPath[^1], null);
            
        }
        
        if (dest[0] == "..")
        {
            return new Tuple<Node, string>(fileSystem.GetCurrentPath()[^2], null);
        }
    
        Node validNode = fileSystem.GetCurrentNode().SearchChildren(dest[0]);
        
        return validNode == null ? new Tuple<Node, string>(null, dest[0]) : 
            new Tuple<Node, string>(validNode, null);
    }

    private List<Node> GetValidSource(string[] arguments)
    {
        List<Node> validSrcNodes = new List<Node>();
        
        foreach (string arg in arguments)
        {
            string[] path = arg.Split('/');

            if (path.Length > 1)
            {
                List<Node> validPath = fileSystem.CheckPath(fileSystem.GetCurrentNode(), path, 0, new List<Node>());
                if (validPath == null)
                {
                    fileSystem.SendOutput("Invalid source 1", false);
                }
                else
                {
                    validSrcNodes.Add(validPath[^1]);
                }
            }
            else
            {
                Node target = fileSystem.GetCurrentNode().SearchChildren(path[0]);
                if (target == null)
                {
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

    private void Rename(Node src, string dest)
    {
        src.name = dest;
        
        fileSystem.SendOutput("", false);
    }

    private void Overwrite(FileNode src, FileNode dest)
    {
        // Dest takes contents of src, src removed
        dest.SetContents(src.GetContents());
        fileSystem.RemoveNode(src.GetParent(), src);
        
        fileSystem.SendOutput("", false);
    }

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

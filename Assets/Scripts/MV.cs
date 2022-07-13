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
        Debug.Log("Options: " + string.Join(',', command.Item1));
        Debug.Log("Arguments: " + string.Join(',', command.Item2));
        
        
    }

    private Tuple<List<Node>, Node, string, int> GetDest(string destination)
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
                return new Tuple<List<Node>, Node, string, int>(null, null, dest[^1], 3);
            }
            
            return new Tuple<List<Node>, Node, string, int>(destPath, null, null, 1);
            
        }
        
        if (dest[0] == "..")
        {
            return new Tuple<List<Node>, Node, string, int>(null, fileSystem.GetCurrentPath()[^2], null, 2);
        }
        
        List<Node> neighbours = fileSystem.GetCurrentNode().GetNeighbours();
        foreach (Node node in neighbours)
        {
            if (node.name == dest[0])
            {
                return new Tuple<List<Node>, Node, string, int>(null, node, null, 2);
            }
        }

        return new Tuple<List<Node>, Node, string, int>(null, null, dest[0], 3);
        
    }

    private void Rename()
    {
        
    }

    private void Overwrite()
    {
        fileSystem.SendOutput("OVERWRITING", false);
    }

    private void Move()
    {
        fileSystem.SendOutput("MOVING", false);
    }
}

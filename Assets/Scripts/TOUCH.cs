/*
 * Author: Jacob Hobbs - 190161842
 * Date : July 2022
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Animations;

public class TOUCH : MonoBehaviour
{
    // Root command for touch - create new file

    public GraphManager fileSystem;

    public void touch(string input)
    {
        // TODO Usage
        Tuple<char[], string[]> command = fileSystem.SeparateAndValidate(input, "touch", new char[]{}, "touch usage");
        
        if (command == null) { return; }

        string[] arguments = command.Item2;

        // iterate through each argument, check the path or check for name
        foreach (string file in arguments)
        {
            string[] splitPath = file.Split('/');
            
            if (splitPath.Length > 1)
            {
                // If is a path
                List<Node> validPath = fileSystem.CheckPath(fileSystem.GetCurrentNode(), splitPath.SkipLast(1).ToArray(), 0, new List<Node>());
                if (validPath != null && validPath[^1].GetType() == typeof(DirectoryNode))
                {
                    // Check there's not a file or directory that already exists with that name
                    if (!CheckDuplicate((DirectoryNode)validPath[^1], splitPath[^1]))
                    {
                        fileSystem.AddFileNode((DirectoryNode)validPath[^1], splitPath[^1]);
                    }
                }
                // TODO if path isn't valid? if last in path is a file?
            }
            else
            {
                // If is a single file
                if (!CheckDuplicate(fileSystem.GetCurrentNode(), splitPath[0]))
                {
                    fileSystem.AddFileNode(fileSystem.GetCurrentNode(), splitPath[0]);
                    
                }
            }
        }
        
        fileSystem.SendOutput("", false);
    }

    // Method to check if a node already exists with the given name (target) under the directory parent
    private bool CheckDuplicate(DirectoryNode parent, string target)
    {
        List<Node> neighbours = parent.GetNeighbours();

        foreach (Node node in neighbours)
        {
            if (node.name == target)
            {
                return true;
            }
        }

        return false;
    }
}

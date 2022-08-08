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
    private string _usage = "usage: touch file ...";
    private List<string> _toOutput;

    public void touch(string input)
    {
        _toOutput = new List<string>();

        if (string.IsNullOrEmpty(input))
        {
            fileSystem.SendOutput(_usage, false);
            return;
        }
        
        Tuple<List<char>, List<string>, List<Tuple<string, string>>> command = fileSystem.ValidateOptions(input, new char[]{}, "touch");

        if (command.Item3.Count > 0)
        {
            fileSystem.SendOutput(command.Item3[0].Item2 + "\n" + _usage, false);
            return;
        }

        List<string> arguments = new List<string>();
        foreach (string arg in command.Item2)
        {
            if (arg.StartsWith('-'))
            {
                _toOutput.Add("touch : " + arg + ": invalid file name");
            }
            else
            {
                arguments.Add(arg);
            }
        }
        
        // Iterate through each argument, check the path or check for name
        foreach (string file in arguments)
        {
            string[] splitPath = file.Split('/');

            if (splitPath.Length > 1)
            {
                Tuple<List<Node>, string> path = fileSystem.CheckPath(fileSystem.GetCurrentNode(), splitPath.SkipLast(1).ToArray(), 0, new List<Node>());
                if (path.Item2 != null)
                {
                    // Invalid path
                    List<string> names = new List<string>();
                    foreach (Node node in path.Item1)
                    {
                        names.Add(node.name);
                    }
                    
                    _toOutput.Add("touch: " + string.Join('/', names) + "/" + splitPath[path.Item1.Count] + ": " + path.Item2);
                    break;
                }

                // Invalid path - last node is a directory - can't create a child of a directory
                if (path.Item1[^1].GetType() != typeof(DirectoryNode))
                {
                    _toOutput.Add("touch: " + path.Item1[^1].name + ": is not a directory");
                    break;
                }
                
                CheckDuplicateAndAdd((DirectoryNode)path.Item1[^1], splitPath[^1]);
            }
            else
            {
                CheckDuplicateAndAdd(fileSystem.GetCurrentNode(), splitPath[0]);
            }
        }
        
        fileSystem.SendOutput(string.Join('\n', _toOutput), false);
    }

    // Method to check if a node already exists with the given name (target) under the directory parent
    private void CheckDuplicateAndAdd(DirectoryNode parent, string target)
    {
        bool duplicate = false;
        
        List<Node> neighbours = parent.GetNeighbours();

        foreach (Node node in neighbours)
        {
            if (node.name == target)
            {
                duplicate = true;
            }
        }

        if (!duplicate)
        {
            fileSystem.AddFileNode(parent, target);
        }
    }
}

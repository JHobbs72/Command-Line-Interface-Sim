/*
 * Author: Jacob Hobbs - 190161842
 * Date : July 2022
 */

using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RM : MonoBehaviour
{
    // Root command for 'remove' - remove a node

    public GraphManager fileSystem;
    private bool _rOption;
    private bool _vOption;
    private const string Usage = "rm [-rv] file ...";
    private List<string> _toOutput;

    public void rm(string input)
    {
        _rOption = false;
        _vOption = false;
        _toOutput = new List<string>(); 
        
        if (string.IsNullOrEmpty(input))
        {
            fileSystem.SendOutput(Usage, false);
            return;
        }
        
        Tuple<List<char>, List<string>, List<Tuple<string, string>>> command = fileSystem.ValidateOptions(input, new[] {'r', 'v'}, "rm");

        if (command.Item1.Contains('r'))
        {
            _rOption = true;
        }
        
        if (command.Item1.Contains('v'))
        {
            _vOption = true;
        }

        if (command.Item3.Count > 0)
        {
            fileSystem.SendOutput(command.Item3[0].Item2 + '\n' + Usage, false);
            return;
        }

        if (command.Item2.Count < 1)
        {
            fileSystem.SendOutput(Usage, false);
            return;
        }

        Tuple<List<Node>, List<Tuple<string, string>>> arguments = fileSystem.ValidateArgs(command.Item2, "rm");

        foreach (Tuple<string, string> error in arguments.Item2)
        {
            _toOutput.Add(error.Item2);
        }

        if (arguments.Item1.Count > 0)
        {
            foreach (Node node in arguments.Item1)
            {
                if (_rOption)
                {
                    RecursiveRemove(node);
                }
                else
                {
                    RemoveSingle(node);
                }
            }
        }
        else
        {
            fileSystem.SendOutput(_toOutput.Count > 0 ? string.Join('\n', _toOutput) : Usage, false);
            return;
        }
        
        fileSystem.SendOutput(string.Join('\n', _toOutput), false);
    }

    private void RemoveSingle(Node target)
    {
        // IS a Directory Node
        if (target.GetType() == typeof(DirectoryNode))
        {
            // '-r' option not applied
            if (!_rOption)
            {
                _toOutput.Insert(0, "rm: " + target.name + ": is a directory");
            }
            // '-r' option applied
            else
            {
                // Has neighbours
                if (target.GetNeighbours().Count > 0)
                {
                    RecursiveRemove(target);
                }
                // No neighbours
                else
                {
                    fileSystem.RemoveNode(target.GetParent(), target);
                    if (_vOption)
                    {
                        _toOutput.Add(target.name);
                    }
                }
            }
        }
        // If is FileNode
        else 
        {
            fileSystem.RemoveNode(target.GetParent(), target);
            if (_vOption)
            {
                _toOutput.Add(target.name);
            }
        }
    }

    private void RecursiveRemove(Node current)
    {
        // If the argument is a FileNode
        if (current.GetType() == typeof(FileNode))
        {
            RemoveSingle(current); return;
        }
        
        // If the argument is a DirectoryNode
        List<Node> neighbours = current.GetNeighbours();

        // If the Directory has children --> Call RecursiveRemove on each child
        if (neighbours.Count > 0)
        {
            foreach (Node node in new List<Node>(neighbours))
            {
                RecursiveRemove(node);
            }
        }
        
        // If the directory has no children
        RemoveSingle(current);
    }
}

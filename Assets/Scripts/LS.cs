/*
 * Author: Jacob Hobbs - 190161842
 * Date : July 2022
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LS : MonoBehaviour
{
    // Root command for 'list' - list contents of current directory

    public GraphManager fileSystem;
    private bool _fOption;
    private List<string> _toOutput;

    public void ls(string input)
    {
        _fOption = false;
        _toOutput = new List<string>();
        
        Tuple<List<char>, List<string>, List<Tuple<string, string>>> command = fileSystem.ValidateOptions(input,
            new[] { 'a', 'b' }, "ls");

        if (command.Item3 != null)
        {
            // TODO - print error
            foreach (Tuple<string, string> tuple in command.Item3)
            {
                if (tuple.Item2.Contains("illegal option"))
                {
                    fileSystem.SendOutput(tuple.Item2, false);
                    return;
                }
            }
        }

        if (command.Item2 == null || command.Item2.Count == 0)
        {
            ListNeighbours(fileSystem.GetCurrentNode());
        }
        else
        {
            Tuple<List<Node>, List<Tuple<string, string>>> arguments = fileSystem.ValidateArgs(command.Item2, "ls");
            
            if (arguments.Item2 != null)
            {
                foreach (Tuple<string, string> tuple in arguments.Item2)
                {
                    if (tuple.Item1 == "$HOME")
                    {
                        ListNeighbours(fileSystem.GetRootNode());
                    }
                    else
                    {
                        _toOutput.Insert(0, tuple.Item2);
                    }
                }
            }
            
            foreach (Node node in arguments.Item1)
            {
                ListNeighbours(node);
            }
        }
        
        fileSystem.SendOutput(string.Join('\n', _toOutput), false);
    }

    private void ListNeighbours(Node node)
    {
        if (node.GetType() == typeof(FileNode))
        {
            _toOutput.Add(node.name);
        }
        else
        {
            if (node.GetNeighbours().Count > 1)
            {
                string output = node.name + ": \n";
                
                foreach (Node neighbour in node.GetNeighbours())
                {
                    if (_fOption)
                    {
                        output += neighbour.name + "/" + "\n";
                    }
                    else
                    {
                        output += neighbour.name + "\n";
                    }
                }
                _toOutput.Add(output);
            }
            else
            {
                if (_fOption)
                {
                    _toOutput.Add(node.name + "/");
                }
                else
                {
                    _toOutput.Add(node.name);
                }
            }
        }
    }
}

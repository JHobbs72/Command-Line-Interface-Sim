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
    private bool _multipleArgs;

    public void ls(string input)
    {
        _fOption = false;
        _toOutput = new List<string>();
        _multipleArgs = false;

        if (string.IsNullOrEmpty(input))
        {
            ListNeighbours(fileSystem.GetCurrentNode(), false);
            fileSystem.SendOutput(string.Join('\n', _toOutput), false);
            return;
        }
        
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
            ListNeighbours(fileSystem.GetCurrentNode(), false);
        }
        else
        {
            Tuple<List<Node>, List<Tuple<string, string>>> arguments = fileSystem.ValidateArgs(command.Item2, "ls");

            if (arguments.Item1.Count > 1)
            {
                _multipleArgs = true;
            }
            
            if (arguments.Item2 != null)
            {
                foreach (Tuple<string, string> tuple in arguments.Item2)
                {
                    if (tuple.Item1 == "$HOME")
                    {
                        ListNeighbours(fileSystem.GetRootNode(), _multipleArgs);
                    }
                    else
                    {
                        _toOutput.Insert(0, tuple.Item2);
                    }
                }
            }
            
            foreach (Node node in arguments.Item1)
            {
                ListNeighbours(node, _multipleArgs);
            }
        }
        
        fileSystem.SendOutput(string.Join('\n', _toOutput), false);
    }

    private void ListNeighbours(Node node, bool multipleArguments)
    {
        if (node.GetType() == typeof(FileNode))
        {
            _toOutput.Add(node.name);
        }
        else
        {
            List<Node> neighbours = node.GetNeighbours();
            if (multipleArguments)
            {
                string output = node.name + ": \n";
                
                foreach (Node neighbour in neighbours)
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
                    _toOutput.Add(neighbours[0].name + "/");
                }
                else
                {
                    _toOutput.Add(neighbours[0].name);
                }
            }
        }
    }
}

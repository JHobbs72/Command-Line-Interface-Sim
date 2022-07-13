/*
 * Author: Jacob Hobbs - 190161842
 * Date : July 2022
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class LS : MonoBehaviour
{
    // Root command for 'list' - list contents of current directory

    public GraphManager fileSystem;
    private bool _fOption = false;
    private bool _multiple = false;
    private bool _printCommand = true;
    private bool _isLast = false;

    public void ls(string options)
    {
        if (options == "")
        {
            GetNeighbourNames(fileSystem.GetCurrentNode().GetNeighbours(), _fOption, _multiple, null);
            return;
        }
        
        string[] arguments = options.Split(' ');
            
        if (arguments[0] == "-F")
        {
            _fOption = true;
            arguments = arguments.Skip(1).ToArray();
        }
        
        if (arguments.Length == 0)
        {
            GetNeighbourNames(fileSystem.GetCurrentNode().GetNeighbours(), _fOption, _multiple, null);
            return;
        }

        if (arguments.Length == 1)
        {
            if (arguments[0] == "$HOME")
            {
                GetNeighbourNames(fileSystem.GetRootNode().GetNeighbours(), _fOption, _multiple, null);
                return;
            }
        }

        _multiple = true;

        foreach (string arg in arguments)
        {
            if (arg == arguments[^1])
            {
                _isLast = true;
            }
            
            string[] path = arg.Split('/');
            if (path.Length == 1)
            {
                Node found = fileSystem.GetCurrentNode().SearchChildren(path[0]);
                if (found == null)
                {
                    fileSystem.SendOutput("Error not exist", false);
                }
                else if (found.GetType() == typeof(DirectoryNode))
                {
                    GetNeighbourNames(found.GetNeighbours(), _fOption, _multiple, (DirectoryNode)found);
                }
                else if (found.GetType() == typeof(FileNode))
                {
                    fileSystem.SendOutput(found.name, false);
                }
            }
            else
            {
                List<Node> validPath = fileSystem.CheckPath(fileSystem.GetCurrentNode(), path, 0, new List<Node>());
                if (validPath != null)
                {
                    if (validPath[^1].GetType() == typeof(DirectoryNode))
                    {
                        GetNeighbourNames(validPath[^1].GetNeighbours(), _fOption, _multiple, (DirectoryNode)validPath[^2]);
                    }
                    else
                    {
                        fileSystem.SendOutput(validPath[^1].name, false);
                    }
                }
                else
                {
                    fileSystem.SendOutput("Error --> invalid path", false);
                }
            }
        }
    }

    private void GetNeighbourNames(List<Node> neighbours, bool fOption, bool multiple, DirectoryNode parent)
    {
        List<string> names = new List<string>();

        foreach (Node node in neighbours)
        {
            if (node.GetType() == typeof(DirectoryNode) && fOption)
            {
                names.Add(node.name + '/');
            }
            else
            {
                names.Add(node.name);
            }
        }

        if (multiple)
        {
            if (_printCommand)
            {
                fileSystem.SendOutput(parent.name + ": \n" + string.Join(' ', names) + "\n \n", false);
                _printCommand = false;
                return;
            }

            if (_isLast)
            {
                fileSystem.SendSpecialOutput(parent.name + ": \n" + string.Join(' ', names) + "\n>>");
                return;
            }
            
            fileSystem.SendSpecialOutput(parent.name + ": \n" + string.Join(' ', names) + "\n \n");
            return;
        }
        
        fileSystem.SendOutput(string.Join(' ', names), false);
    }
}

/*
 * Author: Jacob Hobbs - 190161842
 * Date : July 2022
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LS : MonoBehaviour
{
    // Root command for 'list' - list contents of current directory

    public GraphManager fileSystem;
    private bool _fOption;
    private bool _printCommand;
    private bool _isLast;

    public void ls(string options)
    {
        _fOption = false;
        _printCommand = true;
        _isLast = false;
        
        // If no arguments or options are given, display all neighbours of current directory
        if (options == "")
        {
            GetNeighbourNames(fileSystem.GetCurrentNode().GetNeighbours(), false, false, null);
            return;
        }
        
        string[] arguments = options.Split(' ');
        
        // Only option available on ls command
        if (arguments[0] == "-F")
        {
            _fOption = true;
            arguments = arguments.Skip(1).ToArray();
        }

        // No other arguments should start with '-'
        foreach (string str in arguments)
        {
            if (str.StartsWith('-'))
            {
                // TODO Usage
                fileSystem.SendOutput("Error - invalid option: " + str + "\nls usage", false);
                return;
            }
        }

        // If the only argument was the '-F' option continue as if no arguments were given
        if (arguments.Length == 0)
        {
            GetNeighbourNames(fileSystem.GetCurrentNode().GetNeighbours(), _fOption, false, null);
            return;
        }

        // If there's one argument and it's "$HOME", display the neighbours of the root directory
        if (arguments.Length == 1)
        {
            if (arguments[0] == "$HOME")
            {
                GetNeighbourNames(fileSystem.GetRootNode().GetNeighbours(), _fOption, false, null);
                return;
            }
        }
        
        // For any other arguments
        foreach (string arg in arguments)
        {
            if (arg == arguments[^1])
            {
                _isLast = true;
            }
            
            // Check if argument is a path
            string[] path = arg.Split('/');
            if (path.Length == 1)
            {
                // If it's not a path
                Node found = fileSystem.GetCurrentNode().SearchChildren(path[0]);
                if (found == null)
                {
                    // Directory doesn't exist under this node
                    fileSystem.SendOutput("Error -- no such file or directory", false);
                }
                else if (found.GetType() == typeof(DirectoryNode))
                {
                    // If it is found and is a directory - display it's neighbours
                    GetNeighbourNames(found.GetNeighbours(), _fOption, true, (DirectoryNode)found);
                }
                else if (found.GetType() == typeof(FileNode))
                {
                    // If it's found and is a file - print it's name
                    fileSystem.SendOutput(found.name, false);
                }
            }
            else
            {
                // If it's a path
                List<Node> validPath = fileSystem.CheckPath(fileSystem.GetCurrentNode(), path, 0, new List<Node>());
                if (validPath != null)
                {
                    if (validPath[^1].GetType() == typeof(DirectoryNode))
                    {
                        // If path is valid and the last node is a directory node - display it's neighbours
                        // TODO test -- should 'validPath[^2]' below be '...^1'?
                        GetNeighbourNames(validPath[^1].GetNeighbours(), _fOption, true, (DirectoryNode)validPath[^2]);
                    }
                    else
                    {
                        // If path is valid and the last node is a file node - display it's name
                        fileSystem.SendOutput(validPath[^1].name, false);
                    }
                }
                else
                {
                    // Invalid path
                    fileSystem.SendOutput("Error --> invalid path", false);
                }
            }
        }
    }

    // Method to get the names of neighbours and display them
        // neighbours = neighbours of the node in question as a list of nodes
        // fOption = bool, dictates whether the '-F' options has been applied or not
        // multiple = bool, dictates whether there's more than one argument (affects output)
        // parent = the parent node of the given node
    private void GetNeighbourNames(List<Node> neighbours, bool fOption, bool multiple, DirectoryNode parent)
    {
        List<string> names = new List<string>();

        foreach (Node node in neighbours)
        {
            if (node.GetType() == typeof(DirectoryNode) && fOption)
            {
                // If -F --> add '/' to the end of directories names
                names.Add(node.name + '/');
            }
            else
            {
                names.Add(node.name);
            }
        }

        // Change output based on whether one or multiple arguments were given
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
                fileSystem.SendSpecialOutput(parent.name + ": \n" + string.Join(' ', names));
                return;
            }
            
            fileSystem.SendSpecialOutput(parent.name + ": \n" + string.Join(' ', names) + "\n");
            return;
        }
        
        fileSystem.SendOutput(string.Join(' ', names), false);
    }
}

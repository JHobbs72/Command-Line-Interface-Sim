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

    public void ls(string options)
    {
        string[] commands = options.Split(' ');
        string[] arguments;
            
        if (commands[0] == "-F")
        {
            _fOption = true;
            arguments = commands.Skip(1).ToArray();
        }
        else
        {
            arguments = commands;
        }
        
        foreach (string arg in arguments)
        {
            string[] path = arg.Split('/');

            Debug.Log("YEEAA: " + path.Length);
            Debug.Log("YEEAA: " + path[0]);

            
            switch (path.Length)
            {
                case 0:
                {
                    Debug.Log("YYEEE" + string.Join(',', path));

                    List<string> neighbourNames = GetNeighbourNames(fileSystem.GetCurrentNode().GetNeighbours(), _fOption);
                    fileSystem.SendOutput(string.Join(", ", neighbourNames), false);
                    
                    break;
                }
                case 1:
                {
                    if (path[0] == "$HOME")
                    {
                        List<string> neighbourNames = GetNeighbourNames(fileSystem.GetRootNode().GetNeighbours(), _fOption);
                        fileSystem.SendOutput(string.Join(", ", neighbourNames), false);
                        break;
                    }

                    List<Node> neighbours = fileSystem.GetCurrentNode().GetNeighbours();
                    foreach (Node node in neighbours)
                    {
                        if (node.name == path[0])
                        {
                            if (node.GetType() == typeof(DirectoryNode))
                            {
                                List<string> neighbourNames = GetNeighbourNames(node.GetNeighbours(), _fOption);
                                fileSystem.SendOutput(string.Join(", ", neighbourNames), false);
                            }
                            else
                            {
                                fileSystem.SendOutput(string.Join(", ", path), false);
                            }
                        }
                    }

                    break;
                }
                case > 1:
                {
                    List<Node> nodePath = fileSystem.CheckPath(fileSystem.GetCurrentNode(), path, 0, new List<Node>());
                    if (nodePath[^1] == null)
                    {
                        fileSystem.SendOutput("Error", false);
                    }
                    else if (nodePath[^1].GetType() == typeof(FileNode))
                    {
                        fileSystem.SendOutput(arg, false);
                    }
                    else if (nodePath[^1].GetType() == typeof(DirectoryNode))
                    {
                        List<string> neighbourNames = GetNeighbourNames(fileSystem.GetCurrentNode().GetNeighbours(), _fOption);
                        fileSystem.SendOutput(string.Join(", ", neighbourNames), false);
                    }

                    break;
                }
            }
        }

        fileSystem.SendOutput("ls: " + options + ": No such file or Directory", false);
    }

    private List<string> GetNeighbourNames(List<Node> neighbours, bool fOption)
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

        return names;
    }
}

/*
 * Author: Jacob Hobbs - 190161842
 * Date : July 2022
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;

public class MKDIR : MonoBehaviour
{
    public GraphManager fileSystem;
    private bool _pOption;
    private bool _vOption;
    
    public void mkdir(string input)
    {
        //     fileSystem.SendOutput("usage: mkdir [-pv] directory ...", false);
        // TODO Abstraction --> '{' '}' and ',' are allowed in certain conditions
        // TODO             --> for this purpose it's valid to remove them at all times  

        Tuple<char[], string[]> command = fileSystem.SeparateAndValidate(input, "mkdir", new[] { 'p', 'v' }, 
            "usage: mkdir [-pv] directory ...");
        if (command == null) { return; }
        
        char[] charOptions = command.Item1;
        string[] arguments = command.Item2;
        
        for (int i = 0; i < arguments.Length; i++)
        {
            arguments[i] = Regex.Replace(arguments[i], @"['\{},']+", "");
        }

        if (charOptions != null)
        {
            if (charOptions.Contains('p')) { _pOption = true; }
            if (charOptions.Contains('v')) { _vOption = true; }
        }
        
        if (arguments.Length == 0)
        {
            fileSystem.SendOutput("usage: mkdir [-pv] directory ...", false);
            return;
        }


        fileSystem.SendOutput("", false);
        
        foreach (string arg in arguments)
        {
            string[] path = arg.Split('/');

            if (path.Length > 1)
            {
                if (_pOption)
                {
                    AddParentDirs(fileSystem.GetCurrentNode(), path, 0);
                }
                else
                {
                    List<Node> validPath = fileSystem.CheckPath(fileSystem.GetCurrentNode(), path.SkipLast(1).ToArray(), 
                        0, new List<Node>());
                    if (validPath[^1].GetType() == typeof(DirectoryNode))
                    {
                        AddDir((DirectoryNode)validPath[^1], path[^1]);
                    }
                }
            }
            else
            {
                AddDir(fileSystem.GetCurrentNode(), path[0]);
            }
        }
    }

    private void AddParentDirs(DirectoryNode lcn, string[] path, int step)
    {
        if (step == path.Length)
        {
            return;
        }
        
        foreach (Node node in lcn.GetNeighbours())
        {
            if (node.name == path[step] && node.GetType() == typeof(DirectoryNode))
            {
                AddParentDirs((DirectoryNode)node, path, step + 1);
            }
            else if (node.name == path[step] && node.GetType() == typeof(FileNode))
            {
                fileSystem.SendOutput("Invalid path", false);
                return;
            }
            else
            {
                AddParentDirs(AddDir((DirectoryNode)node, path[step]), path, step + 1);
                break;
            }
        }
    }

    private DirectoryNode AddDir(DirectoryNode parent, string newNode)
    {
        List<Node> neighbours = parent.GetNeighbours();
        foreach (Node node in neighbours)
        {
            if (node.name == newNode)
            {
                fileSystem.SendOutput("mkdir: " + newNode + ": File exists", false);
                return null;
            }
        }

        DirectoryNode created = fileSystem.AddDirectoryNode(parent, newNode);
        if (_vOption)
        {
            fileSystem.SendOutput("mkdir: created directory '" + newNode + "'", false);
        }

        return created;
    }
}

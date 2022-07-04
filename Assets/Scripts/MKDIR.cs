/*
 * Author: Jacob Hobbs - 190161842
 * Date : July 2022
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class MKDIR : MonoBehaviour
{
    public GraphManager fileSystem;
    private bool _pOption;
    private bool _vOption;
    
    public void mkdir(string options)
    {
        // TODO catch only enter '-x' command(s)
        if (options == "")
        {
            fileSystem.SendOutput("usage: mkdir [-pv] directory ...");
            return;
        }

        // Separate '-x' options from remaining commands
        Tuple<char[], string[]> commands = fileSystem.SeparateOptions(options, 2);
        char[] charOptions = commands.Item1;
        string[] arguments = commands.Item2;

        if (charOptions != null)
        {
            if (charOptions.Contains('p'))
            {
                _pOption = true;
            }
            
            if (charOptions.Contains('v'))
            {
                _vOption = true;
            }
            
        }
        
        if (arguments.Length == 0)
        {
            fileSystem.SendOutput("usage: mkdir [-pv] directory ...");
            return;
        }

        // Remove illegal character (\) from commands
        for (int i = 0; i < arguments.Length; i++)
        {
            arguments[i] = Regex.Replace(arguments[i], @"['\']+", "");
        }
        
        
        foreach (string comm in arguments)
        {
            Next(fileSystem.GetCurrentNode(), comm.Split('/'), 0);
        }

        if (!_vOption)
        {
            fileSystem.SendOutput("");
        }
    }

    private void Next(DirectoryNode lcn, string[] path, int step)
    {
        if (path.Length == step)
        {
            return;
        }

        Node node = fileSystem.SearchChildren(lcn, path[step]);
        // Null --> no directory or file with that name exists under this parent
        int scenario = -1;
        
        // A DirectoryNode with this name exists under this parent
        if (node.GetType() == typeof(DirectoryNode))
        {
            scenario = 0;
        }
        // A FileNode with this name exists under this parent
        else if (node.GetType() == typeof(FileNode))
        {
            scenario = 1;
        }
        
        switch (scenario)
        {
            case -1:
                Debug.Log("No directory or file exists with that name --> create new dir and enter");
                DirectoryNode newNode = fileSystem.AddDirectoryNode(lcn, path[step]);
                if (_vOption)
                {
                    fileSystem.SendOutput("mkdir: created directory '" + newNode.name + "'");
                }
                Next(newNode, path, step + 1);
                break;
            case 0:
                Debug.Log("A DirectoryNode with this name exists under this parent");
                Next((DirectoryNode)node, path, step + 1);
                break;
            case 1:
                Debug.Log("A FileNode with this name exists under this parent");
                fileSystem.SendOutput("mkdir: " + node.name + ": Not a directory"); // Add path up to this point to 'node.name' e.g. dir/dir/<failed-file>
                break;
        }
    }
}

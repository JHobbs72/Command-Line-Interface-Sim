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
            fileSystem.SendOutput("usage: mkdir [-pv] directory ...", false);
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
            fileSystem.SendOutput("usage: mkdir [-pv] directory ...", false);
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
        
        if (node == null)
        {
            scenario = -1;
        }
        // A DirectoryNode with this name exists under this parent
        else if (node.GetType() == typeof(DirectoryNode))
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
                // If last node in path --> create and return
                if (step == path.Length - 1)
                {
                    DirectoryNode lastNode = fileSystem.AddDirectoryNode(lcn, path[step]);
                    fileSystem.SendOutput("", false);
                    
                    if (_vOption)
                    {
                        fileSystem.SendOutput("mkdir: created directory '" + lastNode.name + "'", false);
                    }
                    return;
                }
                
                // If not last node in path but -p --> create and next
                if (_pOption)
                {
                    DirectoryNode pNode = fileSystem.AddDirectoryNode(lcn, path[step]);
                    Next(pNode, path, step + 1);
                    
                    if (_vOption)
                    {
                        fileSystem.SendOutput("mkdir: created directory '" + pNode.name + "'", false);
                    }
                }

                // If not last node in path and !-p
                fileSystem.SendOutput("mkdir: " + path[step] + ": No such file or directory", false);
                break;
            
            case 0:
                Next((DirectoryNode)node, path, step + 1);
                break;
            
            case 1:
                fileSystem.SendOutput("mkdir: " + node.name + ": Not a directory", false); // Add path up to this point to 'node.name' e.g. dir/dir/<failed-file>
                break;
        }
    }
}

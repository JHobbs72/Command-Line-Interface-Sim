/*
 * Author: Jacob Hobbs - 190161842
 * Date : July 2022
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class RM : MonoBehaviour
{
    // Root command for 'remove' - remove a file

    public GraphManager fileSystem;
    private char[] _options;

    public void rm(string option)
    {
        if (option == "")
        {
            fileSystem.SendOutput("usage: rm [-f | -i] [-rv] file ... \n           unlink file");
            return;
        }

        // -f | -i --> if used together (pointless) the second takes precedence
        // -rv     --> both execute individually


        // Separate '-x' options and remaining commands
        // MaxLength = '-x -x -x -x'
        Tuple<char[], string[]> commands = fileSystem.SeparateOptions(option, 4);
        _options = commands.Item1;
        string[] arguments = commands.Item2;

        // If options -i and -f are both given the second is used
        if (_options != null)
        {
            if (_options.Contains('i') && _options.Contains('f'))
            {
                _options = InteractAndForce(_options);
            }
        }

        foreach (string arg in arguments)
        {
            DoRemoving(fileSystem.GetCurrentNode(), arg.Split('/'), 0);
        }


    }

    private void DoRemoving(DirectoryNode lcn, string[] path, int step)
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
                fileSystem.SendOutput("rm: " + node.name + ": No such file or directory");
                break;
            case 0:
                if (!_options.Contains('r'))
                {
                    fileSystem.SendOutput("rm: " + node.name + ": is a directory");
                    break;
                }

                if (_options.Contains('i') && Prompt(node, 0))
                {
                    DoRemoving((DirectoryNode)node, path, step + 1);
                }
                
                break;
            case 1:
                if (Prompt(node, 1))
                {
                    fileSystem.RemoveNode(lcn, node);
                    if (_options.Contains('v'))
                    {
                        fileSystem.SendOutput(node.name);
                    }
                    fileSystem.SendOutput("");
                }
                break;
        }
    }
    
    private char[] InteractAndForce(char[] options)
    {
        int priority = Array.IndexOf(options, 'i') - Array.IndexOf(options, 'f');
        if (priority > 0)
        {
            // -i is after -f so use -i
            List<char> charList = options.ToList();
            charList.Remove('f');
            return charList.ToArray();
        }
        else
        {
            // -f is after -i so use -f
            List<char> charList = options.ToList();
            charList.Remove('i');
            return charList.ToArray();
        }
    }

    private bool Prompt(Node node, int promptPoint)
    {
        // for -i option
        // use promptPoint for what message to display - enter dir or remove xyz

        // Enter directory
        if (promptPoint == 0)
        {
            fileSystem.SendOutput("Examine file in directory '" + node.name + "'?");
            // Get input
            return true;
        }

        if (promptPoint == 1)
        {
            fileSystem.SendOutput("Remove " + node.name + "?");
            return true;
        }

        return false;
    }
}

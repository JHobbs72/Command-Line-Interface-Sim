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
        else
        {
            _options = new char[]{'x'};
        }
        
        fileSystem.SendOutput("");

        foreach (string arg in arguments)
        {
            DoRemoving(fileSystem.GetCurrentNode(), arg.Split('/'), 0);
        }
    }

    private void DoRemoving(DirectoryNode lcn, string[] path, int step)
    {
        if (path.Length == step){ return; }
        Debug.Log("PEEP");
        Node node = fileSystem.SearchChildren(lcn, path[step]);

        // Null --> no directory or file with that name exists under this parent
        int scenario = -1;
        if (node == null) { scenario = -1; }
        // A DirectoryNode with this name exists under this parent
        else if (node.GetType() == typeof(DirectoryNode)) { scenario = 0; }
        // A FileNode with this name exists under this parent
        else if (node.GetType() == typeof(FileNode)) { scenario = 1; }
        
        Debug.Log("Scenario: " + scenario + "\nOptions: " + string.Join(',', _options));

        switch (scenario)
        {
            case -1:
                fileSystem.SendOutput("rm: " + path[step] + ": No such file or directory");
                break;
            case 0:
                if (!_options.Contains('r'))
                {
                    fileSystem.SendOutput("rm: " + node.name + ": is a directory");
                    break;
                }
                if (_options.Contains('i'))
                {
                    if (Prompt(node, 0))
                    {
                        DoRemoving((DirectoryNode)node, path, step + 1);
                    }
                }
                else
                {
                    if (node.GetNeighbours().Count == 0)
                    {
                        if (_options.Contains('i'))
                        {
                            if (Prompt(node, 1))
                            {
                                fileSystem.RemoveNode(lcn, node);
                        
                                if (_options.Contains('v'))
                                {
                                    fileSystem.SendOutput(node.name);
                                }
                            }
                        }
                        fileSystem.RemoveNode(lcn, node);
                        
                        if (_options.Contains('v'))
                        {
                            fileSystem.SendOutput(node.name);
                        }
                    }
                    else
                    {
                        DoRemoving((DirectoryNode)node, path, step);
                    }
                }
                break;
            case 1:
                if (_options.Contains('i'))
                {
                    if (Prompt(node, 1))
                    {
                        fileSystem.RemoveNode(lcn, node);
                        
                        if (_options.Contains('v'))
                        {
                            fileSystem.SendOutput(node.name);
                        }
                    }
                }
                else
                {
                    fileSystem.RemoveNode(lcn, node);
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
        switch (promptPoint)
        {
            case 0:
                fileSystem.SendOutput("Examine file in directory '" + node.name + "'?");
                // Get input
                return true;
            case 1:
                fileSystem.SendOutput("Remove " + node.name + "?");
                return true;
        }

        return false;
    }
}

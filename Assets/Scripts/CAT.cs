using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CAT : MonoBehaviour
{

    public GraphManager fileSystem;

    public void cat(string input)
    {
        Tuple<char[], string[]> command = fileSystem.SeparateAndValidate(input, "cat", new[] {'n', 'E'}, "Cat Usage...");
        if (command == null) { return; }
        
        char[] options = command.Item1;
        string[] arguments = command.Item2;
        
        
        if (arguments.Contains("<<"))
        {
            fileSystem.SendOutput("Error - invalid argument '<<'", false);
            return;
        }
        
        if (arguments.Contains(">") || arguments.Contains(">>"))
        {
            WriteToFile(arguments);
            return;
        }

        if (arguments[0] == "<")
        {
            WriteToStdOut(arguments);
            return;
        }
        
        if (arguments.Contains("<"))
        {
            fileSystem.SendOutput("Error - invalid argument '<'", false);
            return;
        }

        ShowContents(arguments);
    }

    private void ShowContents(string[] arguments)
    {
        List<FileNode> validFiles = new List<FileNode>();
        foreach (string arg in arguments)
        {
            string[] path = arg.Split('/');
            if (path.Length > 1)
            {
                List<Node> nodePath = fileSystem.CheckPath(fileSystem.GetCurrentNode(), path, 0, new List<Node>());
                if (nodePath != null)
                {
                    if (nodePath[^1].GetType() != typeof(FileNode))
                    {
                        fileSystem.SendOutput("cat: " + nodePath[^1].name + ": Is a directory", false);
                    }
                    else
                    {
                        validFiles.Add((FileNode)nodePath[^1]);
                    }
                }
                else
                {
                    fileSystem.SendOutput("cat: " + path[^1] + ": No such file or directory", false);
                }
            }
            else
            {
                Node file = fileSystem.GetCurrentNode().SearchChildren(arg);
                if (file != null)
                {
                    if (file.GetType() == typeof(FileNode))
                    {
                        validFiles.Add((FileNode)file);
                    }
                    else
                    {
                        fileSystem.SendOutput("cat: " + file.name + ": Is a directory", false);
                    }
                }
                else
                {
                    fileSystem.SendOutput("cat: " + arg + ": No such file or directory", false);
                }
            }
        }

        fileSystem.SendOutput("", false);
        foreach (FileNode node in validFiles)
        {
            fileSystem.SendSpecialOutput("\n" + node.GetContents());
        }
    }

    private void WriteToStdOut(string[] arguments)
    {
        Node nodeContents = fileSystem.GetCurrentNode().SearchChildren(arguments[0]);
        if (nodeContents != null)
        {
            if (nodeContents.GetType() == typeof(FileNode))
            {
                fileSystem.SendOutput(nodeContents.GetContents(), false);
            }
            else
            {
                fileSystem.SendOutput("cat: " + nodeContents.name + ": Is a directory", false);
            }
        }
        else
        {
            fileSystem.SendOutput("cat: " + arguments[0] + ": No such file or directory", false);
        }
    }

    private void WriteToFile(string[] arguments)
    {
        
    }

    private void OutputWithOptions(string[] output)
    {
        // Add line numbers if -n
        // Add $ at end of file if -E
        // If not options STD out
        fileSystem.SendOutput(string.Join(' ', output), false);
    }
}

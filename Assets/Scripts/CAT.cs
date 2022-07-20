using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class CAT : MonoBehaviour
{

    public GraphManager fileSystem;
    private bool _nOption = false;
    private bool _eOption = false;
    private bool _bOption = false;
    private bool _sOption = false;

    public void cat(string input)
    {
        if (string.IsNullOrEmpty(input)) { fileSystem.SendOutput("Cat usage", false); return; }
        Tuple<char[], string[]> command = fileSystem.SeparateAndValidate(input, "cat", new[] {'n', 'E', 'b', 's'}, "Cat Usage...");
        if (command == null) { return; }
        
        char[] options = command.Item1;
        string[] arguments = command.Item2;
        
        if (options != null)
        {
            if (options.Contains('E'))
            {
                _eOption = true;
            }

            if (options.Contains('n'))
            {
                _nOption = true;
            }
            
            if (options.Contains('b'))
            {
                _bOption = true;
            }

            if (options.Contains('s'))
            {
                _sOption = true;
            }
        }

        if (options == null && arguments.Length == 1 && arguments[0] == "*")
        {
            List<string> output = new List<string>();
            foreach (Node node in fileSystem.GetCurrentNode().GetNeighbours())
            {
                if (node.GetType() == typeof(DirectoryNode))
                {
                    output.Add("cat: " + node.name + ": is a directory");
                }
                else
                {
                    output.Add(node.GetContents());
                }
            }
            
            fileSystem.SendOutput(string.Join('\n', output), false);
        }

        if (arguments.Contains("<<"))
        {
            fileSystem.SendOutput("Error -> invalid argument '<<'", false);
            return;
        }

        if (arguments.Contains("<") || !arguments.Contains(">>") || !arguments.Contains(">") || !arguments.Contains("<"))
        {
            WriteToStdOut(arguments);
        }

        if (arguments.Contains(">") || arguments.Contains(">>"))
        {
            WriteToFile(arguments);
        }
    }

    private FileNode GetNode(string input, bool createNewEndNode)
    {
        string[] path = input.Split('/');
        if (path.Length > 1)
        {
            List<Node> nodePath = fileSystem.CheckPath(fileSystem.GetCurrentNode(), path, 0, new List<Node>());
            if (nodePath == null)
            {
                List<Node> testPath = fileSystem.CheckPath(fileSystem.GetCurrentNode(), path.SkipLast(1).ToArray(), 0, new List<Node>());
                if (testPath == null)
                {
                    fileSystem.SendOutput("Error - invalid path", false);
                    return null;
                }

                // If the target node is a destination and doesn't exist in the path -> create it
                if (createNewEndNode)
                {
                    if (testPath[^1].GetType() == typeof(DirectoryNode))
                    {
                        fileSystem.AddFileNode((DirectoryNode)testPath[^1], path[^1]);
                        return (FileNode)fileSystem.CheckPath(fileSystem.GetCurrentNode(), path, 0, new List<Node>())[^1];
                    }
                
                    fileSystem.SendOutput("Error --> invalid path", false);
                    return null;
                }
                
                // If the target node is a source and doesn't exist -> error
                fileSystem.SendOutput("Error --> invalid path", false);
                return null;
            }

            if (nodePath[^1].GetType() == typeof(FileNode))
            {
                return (FileNode)nodePath[^1];
            }
            
            fileSystem.SendOutput("Error --> is Directory", false);
            return null;
        }
        
        Node target = fileSystem.GetCurrentNode().SearchChildren(path[0]);
        if (target.GetType() == typeof(FileNode))
        {
            return (FileNode)target;
        }

        return null;
    }

    private void WriteToStdOut(string[] arguments)
    {
        List<string> input = arguments.ToList();
        List<string> content = new List<string>();

        if (input.Contains("<"))
        {
            while (input.Contains("<"))
            {
                int op = input.IndexOf("<");

                FileNode node = GetNode(input[op + 1], false);
                if (node == null)
                {
                    return;
                }

                content.Add(node.GetContents());

                input.RemoveAt(op);
                input.RemoveAt(op);
            }
        }
        else
        {
            foreach (string src in input)
            {
                FileNode node = GetNode(src, false);
                if (node == null)
                {
                    return;
                }

                content.Add(node.GetContents());

            }
        }
        
        OutputWithOptions(content);
    }

    private void WriteToFile(string[] arguments)
    {
        List<string> input = arguments.ToList();
        List<Tuple<FileNode, string>> destinations = new List<Tuple<FileNode, string>>();
        List<FileNode> sources = new List<FileNode>();
        string content = "";
        
        if (input[0] == ">" || input[0] == ">>" || input[^1] == ">" || input[^1] == ">>")
        {
            fileSystem.SendOutput("Error --> parse error", false);
            return;
        }

        while (input.Contains(">") || input.Contains(">>"))
        {
            int op = input.IndexOf(">");
            if (op < 0)
            {
                op = input.IndexOf(">>");
            }

            FileNode dest = GetNode(input[op + 1], true);
            if (dest == null)
            {
                return;
            }
            
            destinations.Add(new Tuple<FileNode, string>(dest, input[op]));
            // Remove operand and operator e.g: > file2.txt
            input.RemoveAt(op);
            // Indexes change when first removed so same index twice
            input.RemoveAt(op);
        }
        
        foreach (string target in input)
        {
            FileNode source = GetNode(target, false);
            if (source == null)
            {
                fileSystem.SendOutput("Error --> No such file or directory", false);
                return;
            }
            
            sources.Add(source);
            if (content == "")
            {
                content = source.GetContents();
            }
            else
            {
                content = content + "\n" + source.GetContents();
            }
        }

        foreach (Tuple<FileNode, string> dest in destinations)
        {
            if (dest.Item2 == ">")
            {
                dest.Item1.SetContents(content);
            }
            else
            {
                dest.Item1.SetContents(dest.Item1.GetContents() + "\n" + content);
            }
        }
        
        fileSystem.SendOutput("", false);
    }

    private void OutputWithOptions(List<string> output)
    {
        // Add line numbers if -n
        // Add line numbers on non whitespace if -b
        // Add $ at end of file if -E
        // Suppress whitespace to max 1 line if -s
        // If no options STD out

        if (_sOption)
        {
            for (int i = 0; i < output.Count; i++)
            {
                if (string.IsNullOrEmpty(output[i]) && string.IsNullOrEmpty(output[i + 1]))
                {
                    output.RemoveAt(i);
                    i -= 1;
                }
            }
        }

        if (_eOption)
        {
            int count = 1;
            for (int i = 1; i <= output.Count; i++)
            {
                if (string.IsNullOrEmpty(output[i]) && _bOption)
                {
                    // Skip
                }
                else
                {
                    output[i] = count + output[i];
                    count++;
                }
            }
        }

        if (_nOption)
        {
            output[^1] += "$";
        }
        
        fileSystem.SendOutput(string.Join('\n', output), false);
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class CAT : MonoBehaviour
{
    // Root command for 'cat' 
        // Display contents in file, write text to file, write from one file to another
    
    public GraphManager fileSystem;
    private bool _nOption = false;
    private bool _eOption = false;
    private bool _bOption = false;
    private bool _sOption = false;
    private string _usage = "usage: cat [-nEbs] [file ...]";

    public void cat(string input)
    {
        // If no command entered display usage
        if (string.IsNullOrEmpty(input)) { fileSystem.SendOutput(_usage, false); return; }
        // Call function to separate options and arguments
        Tuple<char[], string[]> command = fileSystem.SeparateAndValidate(input, "cat", new[] {'n', 'E', 'b', 's'}, _usage);
        if (command == null) { return; }
        
        char[] options = command.Item1;
        string[] arguments = command.Item2;
        
        // If any valid options are given, set the bool variables as such
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

        // If command == "cat *" display contents of all files under current node
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
                    string contents = node.GetContents();
                    if (contents != null)
                    {
                        output.Add(contents);
                    }
                }
            }
            
            fileSystem.SendOutput(string.Join('\n', output), false);
            return;
        }

        // Invalid command
        if (arguments.Contains("<<"))
        {
            fileSystem.SendOutput("Error -> invalid argument '<<'", false);
            return;
        }

        // TODO --> this and the next if correct? Needs testing thoroughly 
        // If arguments contains '<' or DOESN'T contain any of '>>' '>' '<' then write contents of arguments to standard output
        if (arguments.Contains("<") || !arguments.Contains(">>") || !arguments.Contains(">") || !arguments.Contains("<"))
        {
            WriteToStdOut(arguments);
        }

        // If arguments contains '>' or '>>' write contents to a file
        if (arguments.Contains(">") || arguments.Contains(">>"))
        {
            WriteToFile(arguments);
        }
    }

    // Method called when contents of file(s) is to be written to standard output
        // arguments = the arguments submitted in the command
    private void WriteToStdOut(string[] arguments)
    {
        // Lists created to aid execution
            // input = arguments array as a list
            // content = The content of the requested Nodes to be written
        List<string> input = arguments.ToList();
        List<string> content = new List<string>();

        // Always true on first pass
        if (input.Contains("<"))
        {
            // While the input still has a '<' in it, take the next element and search for a node with that name or path
            while (input.Contains("<"))
            {
                int op = input.IndexOf("<");

                FileNode node = GetNode(input[op + 1], false);
                if (node == null)
                {
                    // Error message in GetNode
                    return;
                }

                // TODO don't add contents of this node?
                    // e.g. cat file1.txt < file2.txt file3.txt doesn't print contents of file2.txt
                // Add contents of the node to the list
                string nodeContents = node.GetContents();
                if (nodeContents != null)
                {
                    content.Add(nodeContents);
                }

                // Remove the operator and operand from the 'input' list i.e. "< file1"
                    // Same index as indexes change after the first is removed
                input.RemoveAt(op);
                input.RemoveAt(op);
            }
        }
        else
        {
            // After all '<' have been removed i.e. all nodes that aren't having their contents written have been removed
            foreach (string src in input)
            {
                // Check all remaining nodes are valid and add contents to the list
                FileNode node = GetNode(src, false);
                if (node == null)
                {
                    return;
                }

                string nodeContents = node.GetContents();
                if (nodeContents != null)
                {
                    content.Add(nodeContents);
                }
            }
        }
        
        // Send contents list to be output
        OutputWithOptions(content);
    }

    // Method called to write text to a file
        // arguments = the arguments from the command
    private void WriteToFile(string[] arguments)
    {
        List<string> input = arguments.ToList();
        // Create a list of Tuples to contain the files to be written to and the operator assigned to them in the command
        List<Tuple<FileNode, string>> destinations = new List<Tuple<FileNode, string>>();
        // TODO sources warning
        List<FileNode> sources = new List<FileNode>();
        string content = "";
        
        // Command cannot start with any of these operators
        if (input[0] == ">" || input[0] == ">>" || input[^1] == ">" || input[^1] == ">>")
        {
            fileSystem.SendOutput("Error --> parse error", false);
            return;
        }

        // While input contains '>' or '>>'
        while (input.Contains(">") || input.Contains(">>"))
        {
            // If input doesn't contain '>' search for '>>'
            int op = input.IndexOf(">");
            if (op < 0)
            {
                op = input.IndexOf(">>");
            }

            // Get node with the name of the element next to operator i.e. "> file1.txt" find node with name "file1.txt"
                // Set createNewEndNode to true --> each element following an operator is a destination & can be created at run time
            FileNode dest = GetNode(input[op + 1], true);
            // If dest is null i.e. an invalid path -- error message in GetNode
            if (dest == null)
            {
                return;
            }
            
            // Creat new tuple of the node that's been found and the operator then add to the list
            destinations.Add(new Tuple<FileNode, string>(dest, input[op]));
            // Remove operand and operator e.g: > file2.txt
            input.RemoveAt(op);
            // Indexes change when first removed so same index twice
            input.RemoveAt(op);
        }
        
        // Every element left in the input list is a source i.e. it's content will be written to a destination
        foreach (string target in input)
        {
            // TODO check if input is empty?
            // Find the node target
            FileNode source = GetNode(target, false);
            if (source == null)
            {
                // TODO doubling up error message?
                fileSystem.SendOutput("Error --> No such file or directory", false);
                return;
            }
            
            sources.Add(source);
            // Add the content of the file to the 'content' variable separated by a new line
            if (content == "")
            {
                content = source.GetContents();
            }
            else
            {
                content = content + "\n" + source.GetContents();
            }
        }

        // For each destination set new contents, append or overwrite
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

    // Method to return a Node when given a string name or path
    private FileNode GetNode(string input, bool createNewEndNode)
    {
        string[] path = input.Split('/');
        // If it's a path
        if (path.Length > 1)
        {
            // Call CheckPath
            List<Node> nodePath = fileSystem.CheckPath(fileSystem.GetCurrentNode(), path, 0, new List<Node>());
            if (nodePath == null)
            {
                // If the path fails check again but without the last element
                List<Node> testPath = fileSystem.CheckPath(fileSystem.GetCurrentNode(), path.SkipLast(1).ToArray(), 0, new List<Node>());
                if (testPath == null)
                {
                    // If path fails still --> invalid path
                    fileSystem.SendOutput("Error - invalid path", false);
                    return null;
                }

                // If the target node is a destination and doesn't exist in the path, create it
                if (createNewEndNode)
                {
                    // If the second last element in the path (or last element in testPath) is a directory node, create a new file node under it
                    if (testPath[^1].GetType() == typeof(DirectoryNode))
                    {
                        fileSystem.AddFileNode((DirectoryNode)testPath[^1], path[^1]);
                        // return the new node
                        return (FileNode)fileSystem.CheckPath(fileSystem.GetCurrentNode(), path, 0, new List<Node>())[^1];
                    }
                
                    // If the last element in testPath is not a directory, cannot make a child of it
                    fileSystem.SendOutput("Error --> invalid path", false);
                    return null;
                }
                
                // If the target node is a source and doesn't exist -> error
                fileSystem.SendOutput("Error --> invalid path", false);
                return null;
            }

            // For a valid path, if the last node is a file node, return that node.
            if (nodePath[^1].GetType() == typeof(FileNode))
            {
                return (FileNode)nodePath[^1];
            }
            
            // If the last node's a directory it cannot be a source or destination under the cat command
            fileSystem.SendOutput("Error --> is Directory", false);
            return null;
        }
        
        // If it's a single node
        Node target = fileSystem.GetCurrentNode().SearchChildren(path[0]);
        if (target.GetType() == typeof(FileNode))
        {
            return (FileNode)target;
        }
        
        // TODO error message? --> is a directory
        
        return null;
    }
    
    // Method to output content to standard output. Takes a list of strings and processes them before output in line
        // with the options specified in the command
    private void OutputWithOptions(List<string> output)
    {
        // n --> print all line numbers
        // E --> Display $ at the end of the file 
        // b --> print line numbers of non empty lines
        // s --> Suppress repeated empty output lines
        // none --> standard output

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

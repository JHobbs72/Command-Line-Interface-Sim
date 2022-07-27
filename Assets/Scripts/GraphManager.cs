/*
 * Author: Jacob Hobbs - 190161842
 * Date : July 2022
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    // Create and manage the graph file structure.
        // Includes all helper functions called from multiple files.

    private Graph _graph;
    private DirectoryNode _currentNode;
    private List<Node> _currentPath;
    private outputText _outputSource;
    private string _currentCommand;
    private prompt _prompt;

    void Start()
    {
        _graph = Graph.Create("FileSystemGraph");

        // Default file structure created from start up
        DirectoryNode rootNode = Node.Create<DirectoryNode>("UserA");
        DirectoryNode documents = Node.Create<DirectoryNode>("Documents");
        DirectoryNode dir1 = Node.Create<DirectoryNode>("dir1");
        DirectoryNode dir2 = Node.Create<DirectoryNode>("dir2");
        FileNode file1 = Node.Create<FileNode>("file1.txt");
        FileNode file2 = Node.Create<FileNode>("file2.txt");
        _graph.AddRoot(rootNode);
        _graph.AddNode(rootNode, documents);
        _graph.AddNode(documents, dir1);
        _graph.AddNode(dir1, dir2);
        _graph.AddNode(dir2, file1);
        _graph.AddNode(documents, file2);

        _currentNode = rootNode;
        _currentPath = new List<Node>();
        AddToCurrentPath(rootNode);

        _outputSource = FindObjectOfType<outputText>();
        _currentCommand = "";

        _prompt = FindObjectOfType<prompt>();
        _prompt.UpdatePrompt();
    }
    
    // Remove a node
    public void RemoveNode(DirectoryNode parentNode, Node targetNode)
    {
        // Call method in graph.cs
        _graph.RemoveNode(parentNode, targetNode);
    }
    
    // Add a file node
    public void AddFileNode(DirectoryNode sourceNode, string nodeName)
    {
        // Create the new file (leaf) node, set it's parent then call method in graph.cs
        FileNode newFile = Node.Create<FileNode>(nodeName);
        newFile.SetParent(sourceNode);
        _graph.AddNode(sourceNode, newFile);
    }

    // Add a directory node
    public DirectoryNode AddDirectoryNode(DirectoryNode parent, string name)
    {
        // Create the new directory node, call method in graph.cs and return the node
        DirectoryNode newDir = Node.Create<DirectoryNode>(name);
        _graph.AddNode(parent, newDir);
        return newDir;
    }
    
    // Return root node
    public DirectoryNode GetRootNode()
    {
        return _graph.GetRootNode();
    }

    // Return current node
    public DirectoryNode GetCurrentNode()
    {
        return _currentNode;
    }

    // Set current node
    public void SetCurrentNode(DirectoryNode node)
    {
        _currentNode = node;
    }

    // Return current path through file system
    public List<Node> GetCurrentPath()
    {
        return _currentPath;
    }

    // Append node to current path --> traversing into next directory
    public void AddToCurrentPath(DirectoryNode directory)
    {
        _currentPath.Add(directory);
    }

    // Set new path starting from root node
    public void SetNewPathFromOrigin(List<Node> path)
    {
        _currentPath = path;
        SetCurrentNode((DirectoryNode)path[^1]);
    }

    // One step back in current path through file system
    public DirectoryNode StepBackInPath()
    {
        if (_currentPath.Count > 1)
        {
            _currentPath.RemoveAt(_currentPath.Count - 1);
            SetCurrentNode((DirectoryNode)_currentPath[^1]);
            return (DirectoryNode)_currentPath[^1];
        }
        
        SendOutput("At root", false);
        return null;
    }

    // Store the last command entered
    public void SetCurrentCommand(string command)
    {
        _currentCommand = command;
    }
    
    // Normal output -- includes '>>' and new line
    public void SendOutput(string content, bool flag)
    {
        // Call method in outputText.cs
        _outputSource.AddOutput(_currentCommand, content, flag);
    }
    
    // Directly sends content to output for full flexibility
    public void SendSpecialOutput(string content)
    {
        _outputSource.SpecialOutput(content);
    }

    // Check validity of a path
        // lcn = The directory currently being visited
        // path = The path being checked
        // step = How far through the path the check is i.e. once step == path.length path is fully visited
        // validPath = The path so far, valid nodes added to it as they're checked
    // Return valid path: null if invalid, return list of nodes if valid
    public List<Node> CheckPath(DirectoryNode lcn, string[] path, int step, List<Node> validPath)
    {
        // If step == path lenght isLast = true
        // else isLast = false
        bool isLast = step == path.Length - 1;

        // Look for next node in given path in children of the local current node (the current directory)
        Node nextNode = lcn.SearchChildren(path[step]);
        
        // Scenario indicates whether the node being searched for exists & what type it is
        int scenario = -1;
        if (nextNode == null) { scenario = -1; }
        else if (nextNode.GetType() == typeof(DirectoryNode)) { scenario = 0; }
        else if (nextNode.GetType() == typeof(FileNode)) { scenario = 1; }

        switch (scenario)
        {
            case -1:
                // Node no found under this directory
                SendOutput("zsh: no such file or directory: " + string.Join('/', validPath) + "/" + path[step], false);
                
                return null;
            case 0:
                // Node found & is a directory
                if (isLast)
                {
                    validPath.Add(nextNode);
                    
                    return validPath;
                }
                validPath.Add(nextNode);
                
                return CheckPath((DirectoryNode)nextNode, path, step + 1, validPath);
            case 1:
                // Node found & is a file
                //      If it is a file node it must be the last in the path else the path is in valid
                if (isLast)
                {
                    validPath.Add(nextNode);
                    return validPath;
                }
                SendOutput("zsh: not a directory: " + string.Join('/', validPath) + "/" + path[step], false);
                
                return null;
        }

        return null;
    }

    // Split a command into '-x' options from arguments and valid each component ready for processing
        // input = The command string submitted without the root e.g. "[rm] -r thisDirectory"
        // rootCommand = The string root command, name of file this method has been called from e.g. rm
        // options = The options allowed under this root command as char array e.g. [f, i, r, v]
        // usage = The string to be displayed if the command is invalid, showing the format and valid options of the current root command
    // Returns the options given and the arguments given as a Tuple if valid or null on error
    public Tuple<char[], string[]> SeparateAndValidate(string input, string rootCommand, char[] options, string usage)
    {
        if (string.IsNullOrEmpty(input))
        {
            SendOutput(usage, false);
            return null;
        }
        
        // Call to do the separation of options and arguments
        Tuple<char[], string[]> command = SeparateOptions(input, options);

        // If the command is invalid the first item in the tuple is set to null, the second item is the relevant error message
        // If the command is valid but no options are entered item one is an empty array
        if (command.Item1 == null)
        {
            // e.g. rm: illegal option: -- q    usage ...
            SendOutput(rootCommand + ": " + command.Item2[1] + ": -- "+ command.Item2[0] +  "\n" + usage, false);
            return null;
        }
        
        char[] opts = command.Item1;
        string[] arguments = command.Item2;

        // If no arguments are given i.e. just options --> invalid command
        if (arguments == null || arguments.Length == 0)
        {
            SendOutput(rootCommand + ": " + command.Item2[1] + ": -- "+ command.Item2[0] +  "\n" + usage, false);
            return null;
        }

        return Tuple.Create(opts, arguments);
    }
    
    // Separating '-x' options from the arguments - could be in '-xyz' or '-x -y -z' format or any combination
        // input = The string command submitted without the root command e.g. "[rm] -r thisDirectory"
        // allowedCharacters = Char array of valid options
    // Return char array of options (empty array if none) and string array with arguments as a Tuple
    // On invalid option --> return new Tuple, item1 as null item 2 as error message {invalid char, error message}
    private Tuple<char[], string[]> SeparateOptions(string input, char[] allowedCharacters)
    {
        string[] command = input.Split(' ');
        int count = 0;
        // Two lists used to contain the option characters entered and the valid options
        List<char> candidateCharacters = new List<char>();
        List<char> validAndPresent = new List<char>();

        // Select elements of the command that start with '-' and add it's characters to the candidate list
        // TODO --> adds '-' character? incorrect
        // TODO --> adds even if options come after the arguments? incorrect, should only add up to the first argument, error on any others
        foreach (string str in command)
        {
            if (str.StartsWith('-'))
            {
                count++;
                foreach (char character in str)
                {
                    candidateCharacters.Add(character);
                }
            }
            else
            {
                break;
            }
        }

        // Compare candidate characters (those in the command) to allowed characters
        foreach (char ch in candidateCharacters)
        {
            if (allowedCharacters.Contains(ch))
            {
                if (!validAndPresent.Contains(ch))
                {
                    // Option is in the command and is valid --> add to valid list
                    validAndPresent.Add(ch);
                }
                else
                {
                    // TODO throws error if the same option is entered more than once? incorrect, just don't add to list
                    return Tuple.Create((char[])null, new []{ch.ToString(), "no such file or directory"});
                }
            }
            else
            {
                // Invalid option, return a new Tuple -- item one = null, item two = error message with the option that caused the error
                return Tuple.Create((char[])null, new []{ch.ToString(), "invalid option"});
            }
        }

        // Remove first 'count' number of items from the command
            // count = the number of elements that are options
        command.ToList().RemoveRange(0, Math.Min(count, command.Length));
        foreach (string str in command)
        {
            // If any of the remaining elements start with '-' --> error
            if (str.StartsWith('-'))
            {
                return Tuple.Create((char[])null, new []{str, "no such file or directory"});
            }
        }

        // Return valid options and commands
        return Tuple.Create(validAndPresent.ToArray(), command);
    }
}

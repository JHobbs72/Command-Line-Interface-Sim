/*
 * Author: Jacob Hobbs - 190161842
 * Date : July 2022
 */

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    // Create and manage the graph file structure.
        // Includes all helper functions called from multiple files.

    private Graph _graph = null!;
    private DirectoryNode _currentNode = null!;
    private List<Node> _currentPath = null!;
    private outputText _outputSource = null!;
    private string _currentCommand = null!;
    private prompt _prompt = null!;

    private void Start()
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
    public DirectoryNode AddDirectoryNode(DirectoryNode parent, string newName)
    {
        // Create the new directory node, call method in graph.cs and return the node
        DirectoryNode newDir = Node.Create<DirectoryNode>(newName);
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
    private void AddToCurrentPath(DirectoryNode directory)
    {
        _currentPath.Add(directory);
    }

    // Set new path starting from root node
    public void SetNewPathFromOrigin(List<Node> path)
    {
        _currentPath = path;
        SetCurrentNode((DirectoryNode)path[^1]);
    }

    // Store the last command entered
    public void SetCurrentCommand(string command)
    {
        _currentCommand = command;
    }
    
    // Normal output -- includes '>>' and new line
    public void SendOutput(string content)
    {
        // Call method in outputText.cs
        _outputSource.AddOutput(_currentCommand, content);
    }

    // Check validity of a path
        // lcn = The directory currently being visited
        // path = The path being checked
        // step = How far through the path the check is i.e. once step == path.length path is fully visited
        // validPath = The path so far, valid nodes added to it as they're checked
    // Return Tuple<Node list, string>: 
        // Node list = the valid path up to, not including, any invalid node if there is one
        // string = the error message, if there is one (null if not), describing why the path is invalid
    public Tuple<List<Node>, string?> CheckPath(DirectoryNode lcn, string[] path, int step, List<Node> validPath)
    {
        // Remove null values
        List<string> listPath = path.ToList();
        listPath.RemoveAll(string.IsNullOrEmpty);
        path = listPath.ToArray();

        // If step == path lenght isLast = true
        // else isLast = false
        bool isLast = step == path.Length - 1;

        // Look for the next node in the path - in children of the local current node (the current directory) or the
            // root node, or the LCNs parent (directory above)
        Node nextNode;
        if (step == 0 && path[0] == "~")
        {
            nextNode = GetRootNode();
        }
        else if (path[step] == "..")
        {
            nextNode = lcn.GetParent();
            if (nextNode == null)
            {
                nextNode = GetRootNode();
            }
            
            // If the path is long enough remove the last two nodes as stepping back in path
            if (validPath.Count > 0)
            {
                validPath.RemoveAt(validPath.Count - 1);
            }
            
            if (validPath.Count > 0)
            {
                validPath.RemoveAt(validPath.Count - 1);
            }
        }
        else
        {
            nextNode = lcn.SearchChildren(path[step]);
        }
        
        // Scenario indicates whether the next node exists and what type it is
        int scenario = -1;
        if (nextNode == null) { scenario = -1; }
        else if (nextNode.GetType() == typeof(DirectoryNode)) { scenario = 0; }
        else if (nextNode.GetType() == typeof(FileNode)) { scenario = 1; }

        switch (scenario)
        {
            // Node no found under this directory
            case -1:
                // Return the path that is valid so far
                return new Tuple<List<Node>, string?>(validPath, "No such file or directory");
            
            // Node found & is a directory
            case 0:
                // If is last node - add to valid path and return valid path
                if (isLast)
                {
                    validPath.Add(nextNode!);
                    
                    return new Tuple<List<Node>, string?>(validPath, null);
                }
                
                // If is not last node - add to valid path and call self to continue
                validPath.Add(nextNode!);
                
                return CheckPath((DirectoryNode)nextNode!, path, step + 1, validPath);
            
            case 1:
                // Node found & is a file
                //      If it is a file node it must be the last in the path else the path is invalid
                if (isLast)
                {
                    validPath.Add(nextNode!);
                    return new Tuple<List<Node>, string?>(validPath, null);
                }

                // Not the end of the path - invalid path - return path so far (the valid part) and the error
                return new Tuple<List<Node>, string?>(validPath, "not a directory");
            
        }

        return null!;
    }
    
    // Split a command into '-x' options and arguments and validate each option ready for processing by the root command
        // input = The command string submitted without the root e.g. "[rm] -r thisDirectory"
        // allowed options = The options allowed by the root command that called this function
        // rootCommand = the root command that called this function
    // Returns a Tuple -- <List of characters, List of string, List of Tuples<string, string>>
        // List of characters --> the valid options
        // List of string --> command without options (unvalidated arguments)
        // List of tuples<string, string> --> list of error messages and the option that caused the error

    public Tuple<List<char>, List<string>, List<Tuple<string, string>>> ValidateOptions(string input, char[] allowedOptions, string rootCommand)
    {
        // To be returned as a tuple
        List<char> options = new List<char>();
        List<Tuple<string, string>> errorMessages = new List<Tuple<string, string>>();
        // ---

        // Split the input on spaces for processing
        List<string> command = input.Split(' ').ToList();
        List<char> candidateOptions = new List<char>();
        
        // count - the number of elements that start with a '-' before the first element with out a '-' at the start
        // i.e. the number of options in the command
            // e.g. -a -b file -c
            // count = 2
        int count = 0;

        foreach (string str in command)
        {
            if (str.StartsWith('-'))
            {
                count++;
                
                // Add the options in the command to the list of options to be checked
                if (str.Length == 1)
                {
                    candidateOptions.Add(str.ToCharArray()[0]);
                }
                else
                {
                    foreach (char ch in str.Skip(1))
                    {
                        candidateOptions.Add(ch);
                    }
                }
            }
            else
            {
                // If the element doesn't start with '-' --> stop iterating through
                break;
            }
        }

        // Remove options from the command -- left with just the arguments
        command = command.Skip(count).ToList();

        foreach (char ch in candidateOptions)
        {
            if (allowedOptions.Contains(ch))
            {
                // Add valid option to options to be returned - ignore duplicates
                if (!options.Contains(ch))
                {
                    options.Add(ch);
                }
            }
            else
            {
                // Add error message to list to be returned
                errorMessages.Add(new Tuple<string, string>(ch.ToString(), rootCommand + ": -- " + ch + ": illegal option"));
            }
        }

        // return validated options and arguments with any error messages
        return new Tuple<List<char>, List<string>, List<Tuple<string, string>>>(options, command, errorMessages);
    }

    // Validate arguments
        // candidates = the arguments to be validated 
        // rootCommand = the root command that called this function
        // createNonExisting = mkdir only - for '-p' option on mkdir
    // Returns a Tuple -- <List of Nodes, List of Tuples<string, string>>
        // List of Nodes --> the valid arguments
        // List of tuples<string, string> --> list of error messages and the argument that caused the error
    public Tuple<List<Node>, List<Tuple<string, string>>> ValidateArgs(List<string> candidates, string rootCommand)
    {
        List<Node> arguments = new List<Node>();
        List<Tuple<string, string>> errorMessages = new List<Tuple<string, string>>();

        foreach (string str in candidates)
        {
            // No file or directory should start with '-'
            if (str.StartsWith('-'))
            {
                errorMessages.Add(new Tuple<string, string>(str,
                    rootCommand + ": " + str + ": No such file or directory"));
            }
            else
            {
                // Check argument, path or single node can e passed to CheckPath()
                string[] toCheck = str.Split('/');
                Tuple<List<Node>, string?> path = CheckPath(GetCurrentNode(), toCheck, 0, new List<Node>());

                if (path.Item2 == null)
                {
                    // If valid argument the single node or last node in path is added to the arguments list
                    arguments.Add(path.Item1[^1]);
                }
                else
                {
                    // Construct and add relevant error message to list
                    List<string> names = new List<string>();
                    foreach (Node node in path.Item1)
                    {
                        names.Add(node.name);
                    }

                    if (path.Item1.Count > 0)
                    {
                        errorMessages.Add(new Tuple<string, string>(toCheck[path.Item1.Count], "ls: " +
                            string.Join('/', names) + "/" + toCheck[path.Item1.Count] + ": " + path.Item2));
                    }
                    else
                    {
                        // Fails on first element in path
                        errorMessages.Add(
                            new Tuple<string, string>(toCheck[0], "ls: " + toCheck[0] + ": " + path.Item2));
                    }
                }
            }
        }

        return new Tuple<List<Node>, List<Tuple<string, string>>>(arguments, errorMessages);
    }
}

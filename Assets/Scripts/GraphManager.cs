/*
 * Author: Jacob Hobbs - 190161842
 * Date : July 2022
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    // Create and manage the graph file structure

    private Graph _graph;
    private DirectoryNode _currentNode;
    private List<Node> _currentPath;
    private outputText _outputSource;
    private string _currentCommand;

    void Start()
    {
        _graph = Graph.Create("FileSystemGraph");

        // Nodes created and added from start up
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

        // Helper variables initialised
        _currentNode = rootNode;
        _currentPath = new List<Node>();
        AddToCurrentPath(rootNode);

        _outputSource = FindObjectOfType<outputText>();
        _currentCommand = "";
    }
    
    // Remove a node
    public void RemoveNode(DirectoryNode parentNode, Node targetNode)
    {
        _graph.RemoveNode(parentNode, targetNode);
    }
    
    // Add a file node
    public void AddFileNode(DirectoryNode sourceNode, string name)
    {
        FileNode newFile = Node.Create<FileNode>(name);
        newFile.SetParent(sourceNode);
        _graph.AddNode(sourceNode, newFile);
    }

    // Add a directory node
    public DirectoryNode AddDirectoryNode(DirectoryNode parent, string name)
    {
        DirectoryNode newDir = Node.Create<DirectoryNode>(name);
        _graph.AddNode(parent, newDir);
        return newDir;
    }
    
    public DirectoryNode GetRootNode()
    {
        return _graph.GetRootNode();
    }

    public DirectoryNode GetCurrentNode()
    {
        return _currentNode;
    }

    public void SetCurrentNode(DirectoryNode node)
    {
        _currentNode = node;
    }

    public List<Node> GetCurrentPath()
    {
        return _currentPath;
    }

    public void AddToCurrentPath(DirectoryNode directory)
    {
        _currentPath.Add(directory);
    }

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

    public void SetCurrentCommand(string command)
    {
        _currentCommand = command;
    }
    
    // Normal output -- includes '>>' and new line
    public void SendOutput(string content, bool flag)
    {
        _outputSource.AddOutput(_currentCommand, content, flag);
    }
    
    // Directly sends content to output for full flexibility
    public void SendSpecialOutput(string content)
    {
        _outputSource.SpecialOutput(content);
    }

    // Separating '-x' options from the rest of the command - could be in '-xyz' or '-x -y -z' format or any combination
    // with lenght up to maxLength
    // Return char array of options (null if none) and string array with remaining, separated commands
    // TODO can 'commands' (return string[]) be null? Should have length of 0 if not valid
    public Tuple<char[], string[]> SeparateOptions(string input, int maxLength)
    {
        string[] commands = input.Split(' ');
        List<string> opts = new List<string>();
        // null if no options given, else character array of individual options
        char[] finalOpts = null;

        // OPTIONS DETECTION AND SEPARATION
        // Does the command contain at least one option?
        if (commands[0].StartsWith('-'))
        {
            // If yes add to options list and remove from rest of the command
            opts.Add(commands[0]);
            commands = commands.Skip(1).ToArray();
            
            // 'Longest' combination of options = '-x -x -x -x' with no file in between 
            for (int i = 0; i < maxLength - 1; i++)
            {
                // Is the next part within given range an option?
                if (commands[0].StartsWith('-'))
                {
                    // If yes add to options list and remove from rest of the command
                    opts.Add(commands[0]);
                    commands = commands.Skip(1).ToArray();
                }
            }

            // Convert to string, remove '-' characters then convert to character array
            string stringOpts = string.Join(',', opts);
            stringOpts = stringOpts.Trim(new [] { '-' });
            finalOpts = stringOpts.ToCharArray();
        }

        return Tuple.Create(finalOpts, commands);
    }

    // Check validity of a path
    // Return null if invalid
    // Return list of nodes if valid
    public List<Node> CheckPath(DirectoryNode lcn, string[] path, int step, List<Node> validPath)
    {
        // If step == path lenght isLast = true
        // else isLast = false
        bool isLast = step == path.Length - 1;

        Node nextNode = lcn.SearchChildren(path[step]);
        
        int scenario = -1;
        if (nextNode == null) { scenario = -1; }
        else if (nextNode.GetType() == typeof(DirectoryNode)) { scenario = 0; }
        else if (nextNode.GetType() == typeof(FileNode)) { scenario = 1; }

        switch (scenario)
        {
            case -1:
                SendOutput("zsh: No Such file or directory: " + string.Join('/', validPath) + "/" + nextNode.name, false);
                
                return null;
            case 0:
                if (isLast)
                {
                    validPath.Add(nextNode);
                    
                    return validPath;
                }
                validPath.Add(nextNode);
                
                return CheckPath((DirectoryNode)nextNode, path, step + 1, validPath);
            case 1:
                if (isLast)
                {
                    validPath.Add(nextNode);
                    return validPath;
                }
                SendOutput("zsh: not a directory: " + string.Join('/', validPath) + "/" + nextNode.name, false);
                
                return null;
        }

        return null;
    }
}

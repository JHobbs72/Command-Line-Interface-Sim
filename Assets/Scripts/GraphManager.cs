using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    // Create and manage the graph file structure

    private Graph _graph;
    private DirectoryNode _currentNode;
    private List<Node> _currentPath;
    private outputText _outputSource;
    private string _currentCommand;
    private autoScroll scroll;

    void Start()
    {
        _graph = Graph.Create("FileSystemGraph");

        // Nodes created and added from start up
        DirectoryNode rootNode = Node.Create<DirectoryNode>("UserA");
        DirectoryNode documents = Node.Create<DirectoryNode>("Documents");
        FileNode file1 = Node.Create<FileNode>("file1.txt");
        FileNode file2 = Node.Create<FileNode>("file2.txt");
        _graph.AddRoot(rootNode);
        _graph.AddNode(rootNode, documents);
        _graph.AddNode(rootNode, file1);
        _graph.AddNode(documents, file2);

        // Helper variables initialised
        _currentNode = rootNode;
        _currentPath = new List<Node>();
        AddToCurrentPath(rootNode);

        _outputSource = FindObjectOfType<outputText>();
        _currentCommand = "";
        scroll = FindObjectOfType<autoScroll>();
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
        _graph.AddNode(sourceNode, newFile);
    }

    // Add a directory node
    public void AddDirectoryNode(string name)
    {
        DirectoryNode newDir = Node.Create<DirectoryNode>(name);
        _graph.AddNode(_currentNode, newDir);
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

    // One step back in current path through file system
    public DirectoryNode StepBackInPath()
    {
        if (_currentPath.Count > 1)
        {
            _currentPath.RemoveAt(_currentPath.Count - 1);
            return (DirectoryNode)_currentPath[^1];
        }
        
        SendOutput("At root");
        return null;
    }

    public void SetCurrentCommand(string command)
    {
        _currentCommand = command;
    }
    
    public void SendOutput(string content)
    {
        _outputSource.AddOutput(_currentCommand, content);
    }

    // Separating '-x' options from the rest of the command - could be in '-xyz' or '-x -y -z' format or any combination
    // with lenght up to maxLength
    // Return char array of options (null if none) and string array with remaining, separated commands
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
            string stringOpts = opts.ToString();
            stringOpts = stringOpts.Trim(new [] { '-' });
            finalOpts = stringOpts.ToCharArray();
        }

        return Tuple.Create(finalOpts, commands);
    }

    // Follow path from current node to last in input
    // return last node & bool - Use type of node to check if valid for each use case
    // Invalid path if bool == false, Node is the node it failed on
    public Tuple<Node, bool> followPath(string path)
    {
        string[] elems = path.Split('/');

        return DoPath(_currentNode, elems, 0);
    }

    // File and directory with same name?
    private Tuple<Node, bool> DoPath(DirectoryNode localCurrentNode, string[] path, int step)
    {
        
        if (step == path.Length)
        {
            return new Tuple<Node, bool>(localCurrentNode, true);
        }
        
        List<Node> neighbours = localCurrentNode.GetNeighbours();

        foreach (Node node in neighbours)
        {
            if (node.name == path[step] && node.GetType() == typeof(DirectoryNode))
            {
                DoPath((DirectoryNode)node, path, step++);
                break;
            }
            
            if (node.name == path[step] && node.GetType() == typeof(FileNode))
            {
                return new Tuple<Node, bool>(node, true);
            }
        }

        return null;
    }
}

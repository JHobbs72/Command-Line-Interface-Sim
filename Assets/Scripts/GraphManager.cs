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

    // Follow path from current node to last in input
    // return last node & bool - Use type of node to check if valid for each use case
    // Invalid path if bool == false, Node is the node it failed on (invalid node created to fulfil return)
    public Tuple<Node, bool> FollowPath(string path)
    {
        string[] elems = path.Split('/');

        return DoPath(_currentNode, elems, 0);
    }

    private Tuple<Node, bool> DoPath(DirectoryNode localCurrentNode, string[] path, int step)
    {
        
        if (step == path.Length)
        {
            return new Tuple<Node, bool>(localCurrentNode, true);
        }

        foreach (Node node in localCurrentNode.GetNeighbours())
        {
            // If a node exists in the localCurrentNode's neighbours with the correct name of the next element in the path
            if (node.name == path[step])
            {
                // If node is a DirectoryNode
                if (node.GetType() == typeof(DirectoryNode))
                {
                    return DoPath((DirectoryNode)node, path, step + 1);
                }
                
                // If node is a FileNode but not at the end of the path i.e. '<dir>/<dir>/<file>/<file>' --> invalid path
                if (node.GetType() == typeof(FileNode) && step < path.Length - 1)
                {
                    return new Tuple<Node, bool>(node, false);
                }
                
                // node is a FileNode and is the last Node in the path
                return new Tuple<Node, bool>(node, true);
            }
        }

        // Node in path does not exist in localCurrentNode's neighbours
        return new Tuple<Node, bool>(Node.Create<Node>(path[step]), false);
    }

    public Node SearchChildren(DirectoryNode parent, string target)
    {
        List<Node> neighbours = parent.GetNeighbours();

        foreach (Node node in neighbours)
        {
            if (node.name == target)
            {
                return node;
            }
        }
        return null;
    }
}

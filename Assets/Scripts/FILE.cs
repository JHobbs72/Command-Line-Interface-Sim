using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class FILE : MonoBehaviour
{
    // Root command 'file' -- Show the type of each node
    
    public GraphManager fileSystem;
    
    // TODO argument starting with '-' error

    public void file(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            // TODO usage
            fileSystem.SendOutput("usage: file [file ...]", false);
            return;
        }

        List<string> output = new List<string>();
        
        // Display type of all nodes in current context
        if (input == "*")
        {
            List<Node> neighbours = fileSystem.GetCurrentNode().GetNeighbours();
            foreach (Node node in neighbours)
            {
                output.Add(GetFileTypeOutput(node));
            }
        }
        else
        {
            // Check arguments exist
            string[] arguments = input.Split(' ');
            foreach (string str in arguments)
            {
                string[] path = str.Split('/');
                if (path.Length > 1)
                {
                    List<Node> nodePath = fileSystem.CheckPath(fileSystem.GetCurrentNode(), path, 0, new List<Node>());
                    if (nodePath == null)
                    {
                        // TODO error message
                        fileSystem.SendOutput("Error --> invalid path", false);
                        return;
                    }

                    output.Add(GetFileTypeOutput(nodePath[^1]));
                }
                else
                {
                    Node node = fileSystem.GetCurrentNode().SearchChildren(str);
                    if (node == null)
                    {
                        // TODO error message
                        fileSystem.SendOutput("No such file or directory", false);
                        return;
                    }
                    output.Add(GetFileTypeOutput(node));
                }
            }
        }
        
        fileSystem.SendOutput(string.Join('\n', output), false);
    }

    // Method that takes a node that exists and returns the string to be output to the user
    private string GetFileTypeOutput(Node node)
    {
        if (node.GetType() == typeof(DirectoryNode))
        {
            return node.name + ": directory";
        }
        
        if (node.GetType() == typeof(FileNode))
        {
            return node.name + ": ASCII text";
        }

        // Catch error
        return "No file?";
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class FILE : MonoBehaviour
{
    public GraphManager fileSystem;

    public void file(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            fileSystem.SendOutput("file usage", false);
            return;
        }

        List<string> output = new List<string>();

        
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
            string[] arguments = input.Split(' ');
            foreach (string str in arguments)
            {
                string[] path = str.Split('/');
                if (path.Length > 1)
                {
                    List<Node> nodePath = fileSystem.CheckPath(fileSystem.GetCurrentNode(), path, 0, new List<Node>());
                    if (nodePath == null)
                    {
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
                        fileSystem.SendOutput("No such file or directory", false);
                        return;
                    }
                    output.Add(GetFileTypeOutput(node));
                }
            }
        }
        
        fileSystem.SendOutput(string.Join('\n', output), false);
    }

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

        return "No file?";
    }
}

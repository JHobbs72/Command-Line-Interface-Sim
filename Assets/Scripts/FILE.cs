/*
 * Author: Jacob Hobbs - 190161842
 * Date : July 2022
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FILE : MonoBehaviour
{
    // Root command 'file' -- Show the type of each node
    
    public GraphManager fileSystem;
    private string _usage = "usage: file [file ...]";

    public void file(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            fileSystem.SendOutput(_usage);
            return;
        }

        // Catch any invalid arguments starting with '-'
        foreach (string str in input.Split(' '))
        {
            if (str.StartsWith('-'))
            {
                fileSystem.SendOutput("file: " + str + ": invalid option" + "\n" + _usage);
                return;
            }
        }

        // Initialize list of strings to be output when execution is complete
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
            // Find each arguments and add type to output list
            List<string> arguments = input.Split(' ').ToList();

            Tuple<List<Node>, List<Tuple<string, string>>> validArgs = fileSystem.ValidateArgs(arguments, "file");

            foreach (Node arg in validArgs.Item1)
            {
                output.Add(GetFileTypeOutput(arg));
            }

            foreach (Tuple<string, string> error in validArgs.Item2)
            {
                output.Add(error.Item2);
            }

        }

        // Join all output messages and display
        fileSystem.SendOutput(string.Join('\n', output));
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

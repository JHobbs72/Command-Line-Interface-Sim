using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class MKDIR : MonoBehaviour
{
    // Root command for 'make directory' - create new directory
    // Create new directory as child of current directory, don't allow '/', ',',
    // or '.' in directory name

    public GraphManager fileSystem;
    
    public void mkdir(string options)
    {
        if (options == "")
        {
            fileSystem.SendOutput("usage: mkdir [-pv] directory ...");
            return;
        }
        
        // TODO -p --> parent give path, create parents as necessary
        // TODO -v --> verbose, print name of each directory created
        // TODO Should allow multiple directories to be created

        Tuple<char[], string[]> commands = fileSystem.SeparateOptions(options, 2);
        char[] charOptions = commands.Item1;
        string[] remCommands = commands.Item2;
        
        bool duplicate = false;
        List<Node> neighbours = fileSystem.GetCurrentNode().GetNeighbours();

        // Remove illegal characters (\) from commands
        for (int i = 0; i < remCommands.Length; i++)
        {
            remCommands[i] = Regex.Replace(remCommands[i], @"['\']+", "");
        }
        
        
        
        
        

        foreach (Node neighbour in neighbours)
        {
            if (neighbour.GetType() == typeof(DirectoryNode) && neighbour.name == options)
            {
                duplicate = true;
            }
        }

        if (duplicate)
        {
            fileSystem.SendOutput("A directory called " + options + " already exists");
        }
        else
        {
            fileSystem.AddDirectoryNode(options);
            fileSystem.SendOutput("");
        }
    }
}

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
    private bool _pOption;
    private bool _vOption;
    
    public void mkdir(string options)
    {
        // Tuple<Node, bool> res = fileSystem.FollowPath(options);
        // Debug.Log("Node: " + res.Item1);
        // Debug.Log("Valid: " + res.Item2);
        
        // TODO -p --> parent give path, create parents as necessary
        // TODO -v --> verbose, print name of each directory created
        // TODO Should allow multiple directories to be created

        // TODO catch only enter '-x' command(s)
        if (options == "")
        {
            fileSystem.SendOutput("usage: mkdir [-pv] directory ...");
            return;
        }

        // Separate '-x' options from remaining commands
        Tuple<char[], string[]> commands = fileSystem.SeparateOptions(options, 2);
        char[] charOptions = commands.Item1;
        string[] remCommands = commands.Item2;

        if (charOptions != null)
        {
            if (charOptions.Contains('p'))
            {
                _pOption = true;
            }
            
            if (charOptions.Contains('v'))
            {
                _vOption = true;
            }
            
        }
        
        if (remCommands.Length == 0)
        {
            fileSystem.SendOutput("usage: mkdir [-pv] directory ...");
            return;
        }

        // Remove illegal character (\) from commands
        for (int i = 0; i < remCommands.Length; i++)
        {
            remCommands[i] = Regex.Replace(remCommands[i], @"['\']+", "");
        }
        
        List<Node> neighbours = fileSystem.GetCurrentNode().GetNeighbours();
        
        // TODO message to be sent to log --> only one, make sure it's the right one for the scenario
        // TODO '-x' options
        foreach (string comm in remCommands)
        {
            if (comm.Contains('/'))
            {
                // From path
                MkdirPath(comm, neighbours);
            }
            else
            {
                // Single directory
                MkdirSingle(comm, neighbours);
            }
        }
    }

    private void NewDir(DirectoryNode parent, string[] path, int step)
    {
        List<Node> localNeighbours = parent.GetNeighbours();
        
        Debug.Log("PATH LENGTH: " + path.Length);
        Debug.Log("STEP: " + step);
        // Last element in path
        if (step - 1 == path.Length)
        {
            bool duplicate = false;
            foreach (Node node in localNeighbours)
            {
                if (node.name == path[step])
                {
                    duplicate = true;
                }
            }

            if (!duplicate)
            {
                fileSystem.AddDirectoryNode(parent, path[step]);
                fileSystem.SendOutput("");
                return;
            }
            
            fileSystem.SendOutput("mkdir: " + path[step] + ": File exists");
            return;
        }

        foreach (Node node in localNeighbours)
        {
            if (node.name == path[step] && node.GetType() == typeof(DirectoryNode))
            {
                NewDir((DirectoryNode)node, path, step + 1);
                return;
            }
            
            if (node.name == path[step] && node.GetType() == typeof(FileNode))
            {
                fileSystem.SendOutput("mkdir: " + node.name + ": Not a directory");
                return;
            }
        }
        
        // Create needed directory
        fileSystem.AddDirectoryNode(parent, path[step]);
        List<Node> newNeighbours = parent.GetNeighbours();
        foreach (Node node in newNeighbours)
        {
            if (node.name == path[step] && node.GetType() == typeof(DirectoryNode))
            {
                NewDir((DirectoryNode)node, path, step + 1);
            }
        }
    }
    
    // NO '-p' OPTION
    private void MkdirPath(string dir, List<Node> neighbours)
    {
        if (_pOption)
        {
            string[] path = dir.Split('/');
            NewDir(fileSystem.GetCurrentNode(), path, 0);
        }
        
        // Path given must be valid apart from last (hence 'SkipLast(1)')
        string toValidate = string.Join('/', dir.Split('/').SkipLast(1));
        Tuple<Node, bool> validity = fileSystem.FollowPath(toValidate);
        if (validity.Item2)
        {
            List<Node> localNeighbours = validity.Item1.GetNeighbours();
            string newNode = dir.Split('/')[^1];
            foreach (Node node in localNeighbours)
            {
                if (node.name == newNode)
                {
                    fileSystem.SendOutput("mkdir: " + validity.Item1.name + ": File exists");
                    return;
                }
            }
            fileSystem.AddDirectoryNode((DirectoryNode)validity.Item1, newNode);
            fileSystem.SendOutput("");
        }
        else
        {
            // Invalid path given
            // File Node given in the middle
            if (validity.Item1.GetType() == typeof(FileNode))
            {
                fileSystem.SendOutput("mkdir: " + string.Join('/', dir.Split('/').SkipLast(1)) + ":" + validity.Item1.name + ": Not a directory");
            }
            // Non existent directory
            fileSystem.SendOutput("mkdir: " + validity.Item1.name + ": No such file or directory");
        }
    }

    private void MkdirSingle(string dir, List<Node> neighbours)
    {
        bool duplicate = false;
            
        foreach (Node neighbour in neighbours)
        {
            if (neighbour.GetType() == typeof(DirectoryNode) && neighbour.name == dir)
            {
                duplicate = true;
            }
        }
        
        if (duplicate)
        {
            fileSystem.SendOutput("A directory called " + dir + " already exists");
        }
        else
        {
            fileSystem.AddDirectoryNode(fileSystem.GetCurrentNode(), dir);
            fileSystem.SendOutput("");
        }
    }

    private void POptionPath(string dir)
    {
        
    }
}

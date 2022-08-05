/*
 * Author: Jacob Hobbs - 190161842
 * Date : July 2022
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CD : MonoBehaviour
{
    // Root command for 'change directory'
        // Take '..', '<path>' or '<directory>' and move to the appropriate directory

    public GraphManager fileSystem;

    public void cd(string command)
    {
        // No options for this root command therefore anything starting with '-' is invalid
        foreach (string opt in command.Split (' '))
        {
            if (opt.StartsWith('-'))
            {
                fileSystem.SendOutput("cd: " + opt + ": illegal argument", false);
                return;
            }
        }
        
        // Go to home directory
        if (command is "" or null)
        {
            List<Node> newPath = new List<Node> { fileSystem.GetRootNode() };
            fileSystem.SetNewPathFromOrigin(newPath);
            
            fileSystem.SendOutput("", false);
            return;
        }
        
        if (command == "..")
        {
            if (fileSystem.GetCurrentPath().Count > 1)
            {
                fileSystem.SetNewPathFromOrigin(fileSystem.GetCurrentPath().SkipLast(1).ToList());
                fileSystem.SendOutput("", false);
                return;
            }
            
            fileSystem.SendOutput("At root", false);
            return;
        }
        
        string[] arguments = command.Split(' ');
        // Should only ever be one command, more than 1 = error
        if (arguments.Length > 1)
        {
            fileSystem.SendOutput("cd: too many arguments", false);
            return;
        }
        
        string[] path = arguments[0].Split('/');

        // If a single argument is given i.e. no path
        if (path.Length == 1)
        {
            // If argument = '-' -- go to last visited directory
            // TODO will never get here as error thrown on any argument starting with '-'?
            if (path[0] == "-")
            {
                // TODO go to previous dir
                fileSystem.SendOutput("Previous dir ", false);
                return;
            }
            
            // If argument = '~' -- go to root
            if (path[0] == "~")
            {
                DirectoryNode root = fileSystem.GetRootNode();
                fileSystem.SetCurrentNode(root);
                fileSystem.SetNewPathFromOrigin(new List<Node> {root});
                fileSystem.SendOutput("", false);
                return;
            }
        }
        
        // If a path is given, check and set if valid
        Tuple<List<Node>, string> toCheck = fileSystem.CheckPath(fileSystem.GetCurrentNode(), path, 0, new List<Node>());

        List<Node> testPath = toCheck.Item1;
        
        if (toCheck.Item2 != null)
        {
            fileSystem.SendOutput(toCheck.Item2, false);
            return;
        }

        if (testPath[^1].GetType() == typeof(FileNode))
        {
            fileSystem.SendOutput("cd: not a directory: " + testPath[^1].name, false);
            return;
        }

        DirectoryNode lp = testPath[0].GetParent();
        while (lp != null)
        {
            testPath.Insert(0, lp);
            lp = testPath[0].GetParent();
        }
        
        fileSystem.SetNewPathFromOrigin(testPath);

        fileSystem.SendOutput("", false);
    }
}

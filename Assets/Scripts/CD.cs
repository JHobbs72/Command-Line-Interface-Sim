/*
 * Author: Jacob Hobbs - 190161842
 * Date : July 2022
 */

using System.Collections.Generic;
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
                fileSystem.SendOutput("invalid arguments -", false);
                return;
            }
        }
        
        // Go to home directory
        if (command == "")
        {
            for (int i = 0; i < fileSystem.GetCurrentPath().Count; i++)
            {
                fileSystem.StepBackInPath();
            }
            
            fileSystem.SendOutput(string.Join('/',fileSystem.GetCurrentPath()), false);
        }
        
        if (command == "..")
        {
            fileSystem.StepBackInPath();

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
                return;
            }
        }
        
        // If a path is given, check and set if valid
        List<Node> testPath = fileSystem.CheckPath(fileSystem.GetCurrentNode(), path, 0, new List<Node>()); 
        
        if (testPath == null)
        {
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

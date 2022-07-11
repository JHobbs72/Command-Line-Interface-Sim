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

    public void cd(string options)
    {
        // Go to home directory
        if (options == "")
        {
            // TODO Go to root
            for (int i = 0; i < fileSystem.GetCurrentPath().Count; i++)
            {
                fileSystem.StepBackInPath();
            }
            
            fileSystem.SendOutput(string.Join('/',fileSystem.GetCurrentPath()), false);
        }
        
        string[] arguments = options.Split(' ');
        if (arguments.Length > 1)
        {
            fileSystem.SendOutput("Error 1", false);
            return;
        }
        
        string[] path = arguments[0].Split('/');

        if (path.Length == 1)
        {
            if (path[0] == "-")
            {
                // TODO go to previous dir
                fileSystem.SendOutput("Previous dir ", false);
                return;
            }
            
            if (path[0] == "~")
            {
                // TODO go to home
                fileSystem.SendOutput("Home dir ", false);
                return;
            }
        }

        List<Node> oldPath = fileSystem.GetCurrentPath();
        bool validPath = CheckPath(fileSystem.GetCurrentNode(), path, 0);
        
        if (!validPath)
        {
            fileSystem.SetNewPathFromOrigin(oldPath);
            return;
        }
        
        fileSystem.SendOutput("", false);
    }

    private bool CheckPath(DirectoryNode lcn, string[] path, int step)
    {
        if (step == path.Length)
        {
            return true;
        }
        
        if (path[step] == "-")
        {
            fileSystem.SendOutput("Error --> path & error msg", false);
            return false;
        }

        if (path[step] == "..") { return CheckPath(fileSystem.StepBackInPath(), path, step + 1); }

        foreach (Node node in lcn.GetNeighbours())
        {
            if (node.name == path[step])
            {
                if (node.GetType() == typeof(DirectoryNode))
                {
                    fileSystem.SetCurrentNode((DirectoryNode)node);
                    fileSystem.AddToCurrentPath((DirectoryNode)node);
                    return CheckPath((DirectoryNode)node, path, step + 1);
                }
                
                fileSystem.SendOutput("cd: not a directory: " + node.name, false);
                return false;
            }
        }
        
        fileSystem.SendOutput("No such file or directory", false);
        
        return false;
    }
}

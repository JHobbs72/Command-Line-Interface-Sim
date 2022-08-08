/*
 * Author: Jacob Hobbs - 190161842
 * Date : July 2022
 */

using System.Collections.Generic;
using UnityEngine;

public class PWD : MonoBehaviour
{
    // Root command for 'print working directory'
        // Display current file path - path from root node to current node

    public GraphManager fileSystem;

    public void pwd(string input)
    {
        if (!string.IsNullOrEmpty(input))
        {
            fileSystem.SendOutput("pwd: too many arguments");
            return;
        }
        
        List<Node> currentPath = fileSystem.GetCurrentPath();
        List<string> currentPathNames = new List<string>();
        foreach (Node node in currentPath)
        {
            currentPathNames.Add(node.name);
        }
        
        fileSystem.SendOutput(string.Join('/', currentPathNames));
    }
}

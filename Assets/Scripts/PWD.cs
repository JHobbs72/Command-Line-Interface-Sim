using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PWD : MonoBehaviour
{
    // Root command for 'print working directory'
    // Display current file path - path from root node to current node

    public GraphManager fileSystem;

    public void pwd(string options)
    {
        List<Node> currentPath = fileSystem.getCurrentPath();
        List<string> currentPathNames = new List<string>();
        foreach (Node node in currentPath)
        {
            currentPathNames.Add(node.name);
        }
        
        fileSystem.sendOutput(string.Join('/', currentPathNames));
        Debug.Log(">> " + string.Join('/', currentPathNames));
    }
}

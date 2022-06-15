using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PWD : MonoBehaviour
{
    public GraphManager fileSystem;

    public void pwd(string options)
    {
        List<Node> currentPath = fileSystem.getCurrentPath();
        List<string> currentPathNames = new List<string>();
        foreach (Node node in currentPath)
        {
            currentPathNames.Add(node.name);
        }
        Debug.Log(">> " + string.Join('/', currentPathNames));
    }
}

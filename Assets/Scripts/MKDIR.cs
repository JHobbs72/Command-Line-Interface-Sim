using System.Collections;
using System.Collections.Generic;
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
        bool duplicate = false;
        List<Node> neighbours = fileSystem.getCurrentNode().getNeighbours();
        
        options = Regex.Replace(options, @"[\s/,.:'|]+", "");

        foreach (Node neighbour in neighbours)
        {
            if (neighbour.GetType() == typeof(DirectoryNode) && neighbour.name == options)
            {
                duplicate = true;
            }
        }

        if (duplicate)
        {
            fileSystem.sendOutput("A directory called " + options + " already exists");
        }
        else
        {
            fileSystem.addDirectoryNode(options);
            fileSystem.sendOutput("");
        }
    }
}

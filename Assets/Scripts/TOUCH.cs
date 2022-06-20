using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class TOUCH : MonoBehaviour
{
    // Rooot command for touch - create new file

    public GraphManager fileSystem;

    public void touch(string options)
    {
        bool duplicate = false;
        List<Node> neighbours = fileSystem.getCurrentNode().getNeighbours();
        
        // Remove white space & add 'file type' if none is given
        options = Regex.Replace(options, @"\s+", "");

        if (options.IndexOf('.') == -1)
        {
            options += ".txt";
        }

        foreach (Node neighbour in neighbours)
        {
            if (neighbour.GetType() == typeof(FileNode) && neighbour.name == options)
            {
                duplicate = true;
            }
        }

        if (duplicate)
        {
            fileSystem.sendOutput("A file called " + options + " already exists");
        }
        else
        {
            fileSystem.addLeafNode(options);
            fileSystem.sendOutput("");
        }
    }
}

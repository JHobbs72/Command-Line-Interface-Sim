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
        
        // Remove white space & add .txt file extension if no valid extension is given
        options = Regex.Replace(options, @"\s+", "");
        string lowerOptions = options.ToLower();

        if (!(lowerOptions.EndsWith(".txt") || lowerOptions.EndsWith(".doc") || lowerOptions.EndsWith(".html") || 
            lowerOptions.EndsWith(".xls") || lowerOptions.EndsWith(".ppt")))
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
            fileSystem.addLeafNode(fileSystem.getCurrentNode(), options);
            fileSystem.sendOutput("");
        }
    }
}

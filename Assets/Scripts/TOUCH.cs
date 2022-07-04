/*
 * Author: Jacob Hobbs - 190161842
 * Date : July 2022
 */

using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class TOUCH : MonoBehaviour
{
    // Root command for touch - create new file

    public GraphManager fileSystem;

    public void touch(string options)
    {
        bool duplicate = false;
        List<Node> neighbours = fileSystem.GetCurrentNode().GetNeighbours();
        
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
            fileSystem.SendOutput("A file called " + options + " already exists");
        }
        else
        {
            fileSystem.AddFileNode(fileSystem.GetCurrentNode(), options);
            fileSystem.SendOutput("");
        }
    }
}

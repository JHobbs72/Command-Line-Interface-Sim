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
        // Remove white space & add 'file type' if none is given
        options = Regex.Replace(options, @"\s+", "");

        if (options.IndexOf('.') == -1)
        {
            options += ".txt";
        }

        fileSystem.addLeafNode(options);
    }
}

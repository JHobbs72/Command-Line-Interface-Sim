using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class TOUCH : MonoBehaviour
{
    public GraphManager fileSystem;

    public void touch(string options)
    {
        options = Regex.Replace(options, @"\s+", "");

        if (options.IndexOf('.') == -1)
        {
            options += ".txt";
        }

        fileSystem.addLeafNode(options);
    }
}

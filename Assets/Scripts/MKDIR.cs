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
        options = Regex.Replace(options, @"[\s/,.]+", "");

        fileSystem.addDirectoryNode(options);
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class MKDIR : MonoBehaviour
{
    public GraphManager fileSystem;

    public void mkdir(string options)
    {
        options = Regex.Replace(options, @"[\s/,.]+", "");

        fileSystem.addDirectoryNode(options);
    }
}

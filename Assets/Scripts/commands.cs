using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Commands : MonoBehaviour
{
    public void pwd(string options)
    {
        Debug.Log("You're in a directory");
        Debug.Log(options);
    }

    public void ls(string options)
    {
        Debug.Log("Listing files");
        Debug.Log(options);
    }

    public void mkdir(string options)
    {
        Debug.Log("New directory");
        Debug.Log(options);
    }

    public void rmdir(string options)
    {
        Debug.Log("Dead directory");
        Debug.Log(options);
    }

    public void mv(string options)
    {
        Debug.Log("Moving");
        Debug.Log(options);
    }

    public void touch(string options)
    {
        Debug.Log("New thing");
        Debug.Log(options);
    }
}

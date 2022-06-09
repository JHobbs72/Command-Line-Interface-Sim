using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Commands : MonoBehaviour
{
    public void pwd(string options)
    {
        Debug.Log("You're in a directory");
        //Debug.Log(options[0]);
        //Debug.Log(options[1]);
        //Debug.Log(options[2]);
    }

    public void ls(string options)
    {
        Debug.Log("Listing files");
        Debug.Log(options);
    }

    public void mkdir()
    {
        Debug.Log("New directory");
    }

    public void rmdir()
    {
        Debug.Log("Dead directory");
    }

    public void mv()
    {
        Debug.Log("Moving");
    }

    public void touch()
    {
        Debug.Log("New thing");
    }
}

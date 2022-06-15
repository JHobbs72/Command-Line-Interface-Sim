using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MV : MonoBehaviour
{
    // Root command for 'move' - move a file from one directory to another

    public void mv(string options)
    {
        Debug.Log("Moving");
        Debug.Log(options);
    }
}

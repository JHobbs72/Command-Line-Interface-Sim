using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GIT : MonoBehaviour
{
    // Root command for all git sub commands

    public void git(string options)
    {
        Debug.Log("Gitting");
        Debug.Log(options);
    }
}

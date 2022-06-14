using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PWD : MonoBehaviour
{
    public GraphManager fileSystem;

    public void pwd(string options)
    {
        DirectoryNode currentNode = (DirectoryNode)fileSystem.getCurrentNode();
        Debug.Log(">> " + currentNode.name);
    }
}

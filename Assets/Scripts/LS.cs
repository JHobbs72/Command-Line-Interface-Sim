using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LS : MonoBehaviour
{
    public GraphManager fileSystem;

    public void ls(string options)
    {
        DirectoryNode currentNode = (DirectoryNode)fileSystem.getCurrentNode();
        List<Node> neighbours = currentNode.getNeighbours();
        List<string> neighbourNamesList = new List<string>();

        for (int i = 0; i < neighbours.Count; i++)
        {
            neighbourNamesList.Add(neighbours[i].name);
        }

        fileSystem.sendOutput(string.Join(", ", neighbourNamesList));
        Debug.Log(">> " + string.Join(", ", neighbourNamesList));

    }
}

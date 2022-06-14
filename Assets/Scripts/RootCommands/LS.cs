using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LS : MonoBehaviour
{
    public GraphManager fileSystem;

    public void ls(string options)
    {
        List<Node> neighbours = fileSystem.getCurrentNode().getNeighbours();
        List<string> neighbourNamesList = new List<string>();

        for (int i = 0; i < neighbours.Count; i++)
        {
            neighbourNamesList.Add(neighbours[i].name);
        }

        Debug.Log(">> " + string.Join(", ", neighbourNamesList));

    }
}

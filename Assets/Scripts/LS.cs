/*
 * Author: Jacob Hobbs - 190161842
 * Date : July 2022
 */

using System.Collections.Generic;
using UnityEngine;

public class LS : MonoBehaviour
{
    // Root command for 'list' - list contents of current directory

    public GraphManager fileSystem;

    public void ls(string options)
    {
        DirectoryNode currentNode = (DirectoryNode)fileSystem.GetCurrentNode();
        List<Node> neighbours = currentNode.GetNeighbours();
        List<string> neighbourNamesList = new List<string>();

        for (int i = 0; i < neighbours.Count; i++)
        {
            neighbourNamesList.Add(neighbours[i].name);
        }

        fileSystem.SendOutput(string.Join(", ", neighbourNamesList), false);
    }
}

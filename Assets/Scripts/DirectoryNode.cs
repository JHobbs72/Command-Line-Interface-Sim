/*
 * Author: Jacob Hobbs - 190161842
 * Date : July 2022
 */

using System.Collections.Generic;
using UnityEngine;

public class DirectoryNode : Node
{
    // Inherits from object 'Node' - describes a node that can have child nodes
        // In this context that must be a directory

    [SerializeField]
    private List<Node> neighbours;
    public List<Node> Neighbours
    {
        get
        {
            if (neighbours == null)
            {
                neighbours = new List<Node>();
            }
            return neighbours;
        }
    }

    // Returns all neighbours of the current directory node
    public override List<Node> GetNeighbours()
    {
        return Neighbours;
    }

    // Remove the given node from neighbours list
    public void RemoveNeighbour(Node node)
    {
        Neighbours.Remove(node);
    }
    
    // Search children of this node for a node named 'target', return the node if it exists else return null
    public Node SearchChildren(string target)
    {
        foreach (Node node in GetNeighbours())
        {
            if (node.name == target)
            {
                return node;
            }
        }
        return null;
    }
}

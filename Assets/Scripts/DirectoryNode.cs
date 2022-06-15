using System.Collections;
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

    public override List<Node> getNeighbours()
    {
        return Neighbours;
    }

    public void removeNeighbour(Node node)
    {
        Neighbours.Remove(node);
    }
}

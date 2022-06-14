using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectoryNode : Node
{
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
}

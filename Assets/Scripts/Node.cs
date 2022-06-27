using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Node : ScriptableObject
{
    // Generic node object

    public static T Create<T>(string name)
       where T : Node
    {
        T node = CreateInstance<T>();
        node.name = name;
        return node;
    }

    public virtual List<Node> GetNeighbours()
    {
        return null;
    }

}

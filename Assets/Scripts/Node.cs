/*
 * Author: Jacob Hobbs - 190161842
 * Date : July 2022
 */

using System.Collections.Generic;
using UnityEngine;

public class Node : ScriptableObject
{
    // Generic node object

    private DirectoryNode _parent = null;

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

    public virtual void SetContents(string contents)
    {
        return;
    }
    
    public virtual string GetContents()
    {
        return null;
    }

    public void SetParent(DirectoryNode parent)
    {
        _parent = parent;
    }

    public DirectoryNode GetParent()
    {
        return _parent;
    }

}

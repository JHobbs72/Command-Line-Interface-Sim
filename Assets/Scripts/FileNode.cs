/*
 * Author: Jacob Hobbs - 190161842
 * Date : July 2022
 */

using System.Collections.Generic;

public class FileNode : Node
{
    // Inherits from generic object 'Node' - describes a leaf node of the graph
    // i.e. no children and in this context must be a file not a directory

    private GraphManager _outputSource;
    private string _contents;

    public void Awake()
    {
        _outputSource = FindObjectOfType<GraphManager>();
    }

    // Protects from error when trying to get children of a leaf node
    public override List<Node> GetNeighbours()
    {
        _outputSource.SendOutput("Cannot get children of a file", false);
        return null;
    }

    // Set the contents of the current node
    public override void SetContents(string contents)
    {
        _contents = contents;
    }

    // Return the contents of the current node
    public override string GetContents()
    {
        return _contents;
    }
}

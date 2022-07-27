using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ECHO : MonoBehaviour {
    
    // Root command for 'echo'
        // Writing to a file

    public GraphManager fileSystem;
    
    public void echo(string options)
    {
        string[] arguments = options.Split(' ');
        
        // No '-x' options allowed
        foreach (string arg in arguments)
        {
            if (arg.StartsWith('-'))
            {
                // TODO error message
                fileSystem.SendOutput("Illegal option " + arg, false);
                // TODO usage
                fileSystem.SendOutput("usage ...", true);
                return;
            }
        }

        // Initialise list of tuples to store the name of the file to be written to and whether the text is to append or overwrite
        List<Tuple<string, string>> operatorAndDest = new List<Tuple<string, string>>();
        // List of elements to be removed = operators and file to be written to - left with just the text to write
        List<int> toRemove = new List<int>();
        
        // Iterate through the command separating text from operators from destination files
        for (int i = 0; i < arguments.Length; i++)
        {
            if (arguments[i] == ">" || arguments[i] == ">>")
            {
                operatorAndDest.Add(new Tuple<string, string>(arguments[i], arguments[i+1]));
                toRemove.Add(i);
                toRemove.Add(i + 1);
            }
        }

        // Initialize list of operators and destination file nodes
        List<Tuple<string, FileNode>> destinations = new List<Tuple<string, FileNode>>();

        // Iterate through each destination, check the node specified exists and add to destinations list
        foreach (Tuple<string, string> pair in operatorAndDest)
        {
            string[] path = pair.Item2.Split('/');
            // If the destination is a path
            if (path.Length > 1)
            {
                List<Node> nodePath = fileSystem.CheckPath(fileSystem.GetCurrentNode(), path, 0, new List<Node>());
                if (nodePath == null)
                {
                    // TODO TEST --> is having 'testPath' right? Looks like if the last node in 'nodePath' is invalid it's ignored and use the second last? Meant to create a new node of the last?
                    List<Node> testPath = fileSystem.CheckPath(fileSystem.GetCurrentNode(), path.SkipLast(1).ToArray(), 0, new List<Node>());
                    if (testPath == null)
                    {
                        // TODO error message
                        fileSystem.SendOutput("Error - incorrect path", false);
                        return;
                    }

                    if (testPath[^1].GetType() != typeof(FileNode))
                    {
                        // TODO error message
                        fileSystem.SendOutput("Error - incorrect path", false);
                        return;
                    }
                    destinations.Add(new Tuple<string, FileNode>(pair.Item1, (FileNode)testPath[^1]));
                }
                else
                {
                    // Last node in path must be a file node to write to it
                    if (nodePath[^1].GetType() != typeof(FileNode))
                    {
                        // TODO error message
                        fileSystem.SendOutput("Error - incorrect path", false);
                        return;
                    }
                    destinations.Add(new Tuple<string, FileNode>(pair.Item1, (FileNode)nodePath[^1]));
                }
            }
            else
            {
                // If the destination is a singel node
                Node dest = fileSystem.GetCurrentNode().SearchChildren(path[0]);
                if (dest == null)
                {
                    // If the last node doesn't exist one is created before adding to destinations list
                    fileSystem.AddFileNode(fileSystem.GetCurrentNode(), path[0]);
                    Node newFile = fileSystem.GetCurrentNode().SearchChildren(path[0]);
                    destinations.Add(new Tuple<string, FileNode>(pair.Item1, (FileNode)newFile));
                }
                else
                {
                    // If node exists but is a directory node
                    if (dest.GetType() != typeof(FileNode))
                    {
                        // TODO error message
                        fileSystem.SendOutput("Error", false);
                        return;
                    }
                    destinations.Add(new Tuple<string, FileNode>(pair.Item1, (FileNode)dest));
                }
            }
        }

        // Gather together the text to be written to each file
        List<string> contents = new List<string>();
        for (int i = 0; i < arguments.Length; i++)
        {
            if (!toRemove.Contains(i))
            {
                contents.Add(arguments[i]);
            }
        }

        // Write the contents to the relevant files, operators dictate appending or overwriting
        foreach (Tuple<string, FileNode> dest in destinations)
        {
            if (dest.Item1 == ">")
            {
                dest.Item2.SetContents(string.Join(' ', contents));
            }
            
            if (dest.Item1 == ">>")
            {
                dest.Item2.SetContents(dest.Item2.GetContents() + "\n" + string.Join(' ', contents));
            }
            
        }
        
        // TODO Correct outputs on error but continue
        fileSystem.SendOutput("", false);
    }
}

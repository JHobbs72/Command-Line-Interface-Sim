using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ECHO : MonoBehaviour
{

    public GraphManager fileSystem;
    
    public void echo(string options)
    {
        string[] arguments = options.Split(' ');
        
        foreach (string arg in arguments)
        {
            if (arg.StartsWith('-'))
            {
                fileSystem.SendOutput("Illegal option " + arg, false);
                fileSystem.SendOutput("usage ...", true);
                return;
            }
        }

        List<Tuple<string, string>> operatorAndDest = new List<Tuple<string, string>>();
        List<int> toRemove = new List<int>();
        
        for (int i = 0; i < arguments.Length; i++)
        {
            if (arguments[i] == ">" || arguments[i] == ">>")
            {
                operatorAndDest.Add(new Tuple<string, string>(arguments[i], arguments[i+1]));
                toRemove.Add(i);
                toRemove.Add(i + 1);
            }
        }

        List<Tuple<string, FileNode>> destinations = new List<Tuple<string, FileNode>>();

        foreach (Tuple<string, string> pair in operatorAndDest)
        {
            string[] path = pair.Item2.Split('/');
            if (path.Length > 1)
            {
                List<Node> nodePath = fileSystem.CheckPath(fileSystem.GetCurrentNode(), path, 0, new List<Node>());
                if (nodePath == null)
                {
                    List<Node> testPath = fileSystem.CheckPath(fileSystem.GetCurrentNode(), path.SkipLast(1).ToArray(), 0, new List<Node>());
                    if (testPath == null)
                    {
                        fileSystem.SendOutput("Error - incorrect path", false);
                        return;
                    }

                    if (testPath[^1].GetType() != typeof(FileNode))
                    {
                        fileSystem.SendOutput("Error - incorrect path", false);
                        return;
                    }
                    destinations.Add(new Tuple<string, FileNode>(pair.Item1, (FileNode)testPath[^1]));
                }
                else
                {
                    if (nodePath[^1].GetType() != typeof(FileNode))
                    {
                        fileSystem.SendOutput("Error - incorrect path", false);
                        return;
                    }
                    destinations.Add(new Tuple<string, FileNode>(pair.Item1, (FileNode)nodePath[^1]));
                }
            }
            else
            {
                Node dest = fileSystem.GetCurrentNode().SearchChildren(path[0]);
                if (dest == null)
                {
                    fileSystem.AddFileNode(fileSystem.GetCurrentNode(), path[0]);
                    Node newFile = fileSystem.GetCurrentNode().SearchChildren(path[0]);
                    destinations.Add(new Tuple<string, FileNode>(pair.Item1, (FileNode)newFile));
                }
                else
                {
                    if (dest.GetType() != typeof(FileNode))
                    {
                        fileSystem.SendOutput("Error", false);
                        return;
                    }
                    destinations.Add(new Tuple<string, FileNode>(pair.Item1, (FileNode)dest));
                }
            }
        }

        List<string> contents = new List<string>();
        for (int i = 0; i < arguments.Length; i++)
        {
            if (!toRemove.Contains(i))
            {
                contents.Add(arguments[i]);
            }
        }

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
        
        fileSystem.SendOutput("", false);
    }
}

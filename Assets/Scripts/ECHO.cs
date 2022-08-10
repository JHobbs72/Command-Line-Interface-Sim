using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ECHO : MonoBehaviour {
    
    // Root command for 'echo'
        // Writing to a file
        // Writing back to STD out

    public GraphManager fileSystem;
    private const string Usage = "usage: echo [text] [file ...]";
    private string _toOutput;

    public void echo(string input)
    {
        string[] arguments = input.Split(' ');
        
        // No '-x' options allowed
        foreach (string arg in arguments)
        {
            if (!arg.StartsWith('-')) continue;
            fileSystem.SendOutput("echo: illegal option " + arg + "\n" + Usage);
            return;
        }

        if (arguments.Contains("<"))
        {
            fileSystem.SendOutput("echo: illegal arguments '<'" + "\n" + Usage);
            return;
        }
        
        if (arguments.Contains(">") || arguments.Contains(">>"))
        {
            // WRITING
            if (arguments[^1] == ">" || arguments[^1] == ">>")
            {
                fileSystem.SendOutput("zsh: parse error near end line");
                return;
            }
            
            List<Tuple<string, FileNode>> dests = new List<Tuple<string, FileNode>>();
            List<string> content = new List<string>();

            for (int i = 0; i < arguments.Length; i++)
            {
                // Arg after '>' or '>>' will be a destination
                if (arguments[i] == ">" || arguments[i] == ">>")
                {
                    string[] splitPath = arguments[i + 1].Split('/');
                    // TODO is single --> not a path

                    if (splitPath.Length < 2)
                    {
                        Node node = fileSystem.GetCurrentNode().SearchChildren(splitPath[0]);
                        if (node == null)
                        {
                            fileSystem.AddFileNode(fileSystem.GetCurrentNode(), splitPath[0]);
                            node = fileSystem.GetCurrentNode().SearchChildren(splitPath[0]);
                            dests.Add(new Tuple<string, FileNode>(arguments[i], (FileNode)node));
                        } 
                        else if (node.GetType() == typeof(FileNode))
                        {
                            dests.Add(new Tuple<string, FileNode>(arguments[i], (FileNode)node));
                        }
                        else
                        {
                            fileSystem.SendOutput("echo: " + arguments[i] + ": is a directory");
                            return;
                        }
                    }
                    else
                    {
                        // Check path (not including last node in path)
                    Tuple<List<Node>, string> path = fileSystem.CheckPath(fileSystem.GetCurrentNode(), splitPath.SkipLast(1).ToArray(), 0, new List<Node>());
                    
                    // Valid path (not including last node)
                    if (path.Item2 == null)
                    {
                        // Is last node a Directory
                        if (path.Item1[^1].GetType() == typeof(DirectoryNode))
                        {
                            // Search for last node in full path
                            DirectoryNode penNode = (DirectoryNode)path.Item1[^1];
                            Node destNode = penNode.SearchChildren(splitPath[^1]);
                            
                            if (destNode == null)
                            {
                                // If Last node doesn't exist --> create it and add to destinations
                                fileSystem.AddFileNode(penNode, splitPath[^1]);
                                
                                dests.Add(new Tuple<string, FileNode>(arguments[i], (FileNode)penNode.SearchChildren(splitPath[^1])));
                            }
                            else
                            {
                                // If it does exist is it a FileNode?
                                if (destNode.GetType() == typeof(FileNode))
                                {
                                    // If it is add to destinations
                                    dests.Add(new Tuple<string, FileNode>(arguments[i], (FileNode)destNode));
                                }
                                else
                                {
                                    // Invalid path -- last is DIRECTORY
                                    fileSystem.SendOutput("echo: " + string.Join('/', splitPath) + ": is a directory");
                                    return;
                                }
                            }
                        }
                        else
                        {
                            // INVALID path --> ends in directory
                            fileSystem.SendOutput("echo: " + string.Join('/', splitPath) + ": is a directory");
                            return;
                        }
                    }
                    else
                    {
                        // If path is not valid --> no such file or directory
                        // TODO full correct error message?
                        fileSystem.SendOutput(path.Item2);
                        return;
                    }
                    }
                    
                    // Skip over dest node next to the operand '>' or '>>'
                    i++; 
                }
                else
                {
                    Debug.Log("I:: " + arguments[i] +  " :: " + (i-1));
                    if (i >= 0)
                    {
                        content.Add(arguments[i]);
                        Debug.Log("Adding: " + arguments[i]);
                    }
                    else
                    {
                        content.Add("\n");
                        Debug.Log("Adding: new line");
                    }
                    
                }
            }

            string finalContent = string.Join(' ', content);

            foreach (Tuple<string, FileNode> dest in dests)
            {
                if (dest.Item1 == ">")
                {
                    // Overwrite
                    dest.Item2.SetContents(finalContent);
                    Debug.Log("CONTENT: " + string.Join('-', content));
                }
                else
                {
                    // Append
                    dest.Item2.SetContents(dest.Item2.GetContents() + "\n" + finalContent);
                    Debug.Log("CONTENT2: " + dest.Item2.GetContents() + "\n" + finalContent);
                }
            }
            
            fileSystem.SendOutput("");

        }
        else
        {
            if (string.IsNullOrEmpty(input))
            {
                fileSystem.SendOutput(" ");
            }
            else
            {
                fileSystem.SendOutput(input);
            }
        }
    }
}

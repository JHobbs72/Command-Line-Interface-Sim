/*
 * Author: Jacob Hobbs - 190161842
 * Date : July 2022
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CAT : MonoBehaviour
{
    // Root command for 'cat' 
        // Display contents in file, write from one file to another
        // Abstraction of real cat command
    
    public GraphManager fileSystem;
    private bool _nOption;
    private bool _eOption;
    private string _usage = "usage: cat [-ne] [file ...]";
    private List<string> _toOutput;

    public void cat(string input)
    {
        // ---
        // SECTION ONE
        // ---
        
        _nOption = false;
        _eOption = false;
        _toOutput = new List<string>();
        
        // If no command entered display usage
        if (string.IsNullOrEmpty(input)) { fileSystem.SendOutput(_usage); return; }
        
        // Call function to separate options and arguments
        Tuple<List<char>, List<string>, List<Tuple<string, string>>> command = fileSystem.ValidateOptions(input, new[] {'n', 'e'}, "cat");

        // Display any errors caught when checking options
        if (command.Item3.Count > 0)
        {
            fileSystem.SendOutput(command.Item3[0].Item2 + "\n" + _usage);
            return;
        }
        
        // If no arguments are given
        if (command.Item2.Count == 0)
        {
            fileSystem.SendOutput(_usage);
            return;
        }
        
        // If any valid options are given, set the bool variables as such
        if (command.Item1.Contains('e'))
        {
            _eOption = true;
        }

        if (command.Item1.Contains('n'))
        {
            _nOption = true;
        }

        // If command == "cat *" display contents of all files under current node
        if (command.Item2.Count == 1 && command.Item2[0] == "*")
        {
            List<Node> sources = new List<Node>();
            
            foreach (Node node in fileSystem.GetCurrentNode().GetNeighbours())
            {
                sources.Add(node);
            }
            
            WriteToStdOut(sources);
            return;
        }

        // Invalid commands '<<' and '<' caught
        if (command.Item2.Contains("<<"))
        {
            fileSystem.SendOutput("cat: <<: invalid argument");
            return;
        }
        
        if (command.Item2.Contains("<"))
        {
            fileSystem.SendOutput("cat: <: invalid argument");
            return;
        }
        
        // '>' and '>>' cannot be the last argument
        if (command.Item2[^1] == ">" || command.Item2[^1] == ">>")
        {
            fileSystem.SendOutput("zsh: parse error near end line");
            return;
        }
        
        // '>' and '>>' cannot be the first argument
        if (command.Item2[0] == ">" || command.Item2[0] == ">>")
        {
            fileSystem.SendOutput("zsh: parse error near start line");
            return;
        }
        
        // ---
        // SECTION TWO
        // ---
        
        // If command contains either write operator
        if (command.Item2.Contains(">") || command.Item2.Contains(">>"))
        {
            if (command.Item2.Count > 3)
            {
                fileSystem.SendOutput("cat: Too many arguments");
                return;
            }
            if (command.Item2.Count < 3)
            {
                fileSystem.SendOutput("cat: Not enough arguments");
                return;
            }

            List<string> arguments = new List<string> { command.Item2[0], command.Item2[2] };

            Tuple<List<Node>, List<Tuple<string, string>>> source = fileSystem.ValidateArgs(arguments.SkipLast(1).ToList(), "cat");
            Tuple<List<Node>, List<Tuple<string, string>>> destination = fileSystem.ValidateArgs(arguments.Skip(1).ToList(), "cat");
            
            // Both source and destination nodes exist
            if (source.Item2.Count == 0 && destination.Item2.Count == 0)
            {
                if (source.Item1[0].GetType() == typeof(DirectoryNode))
                {
                    fileSystem.SendOutput("cat: " + source.Item1[0].name + ": is a directory");
                    return;
                }
                
                if (destination.Item1[0].GetType() == typeof(DirectoryNode))
                {
                    fileSystem.SendOutput("cat: " + destination.Item1[0].name + ": is a directory");
                    return;
                }
                
                WriteToFile((FileNode)source.Item1[0], command.Item2[1], (FileNode)destination.Item1[0]);
            }
            // One of the source and destination nodes doesn't exist
            else
            {
                // Source doesn't exist -- error - stop
                if (source.Item2.Count != 0)
                {
                    fileSystem.SendOutput(source.Item2[0].Item2);
                    return;
                }

                // Source is a directory -- error - stop
                if (source.Item1[0].GetType() == typeof(DirectoryNode))
                {
                    fileSystem.SendOutput("cat: " + source.Item1[0].name + ": is a directory");
                    return;
                }

                // Destination doesn't exist
                if (destination.Item2.Count != 0)
                {
                    List<string> newNodePath = arguments[1].Split('/').ToList();
                    // If destination doesn't exist and is a path
                    if (newNodePath.Count > 1)
                    {
                        // Check path except last element (node to be created)
                        Tuple<List<Node>, string> validPath = fileSystem.CheckPath(fileSystem.GetCurrentNode(), newNodePath.SkipLast(1).ToArray(), 0, new List<Node>());
                        
                        // Invalid path -- error before end
                        if (validPath.Item2 != null)
                        {
                            fileSystem.SendOutput("cat: " + validPath.Item2);
                            return;
                        }
                        
                        // Invalid path -- last node is a FileNode
                        if (validPath.Item1[^1].GetType() != typeof(DirectoryNode))
                        {
                            // TODO error message
                            fileSystem.SendOutput("cat: Invalid path");
                            return;
                        }
                        
                        // Valid path -- create new FileNode to add on the end
                        fileSystem.AddFileNode((DirectoryNode)validPath.Item1[^1], newNodePath[^1]);
                        DirectoryNode lastValid = (DirectoryNode)validPath.Item1[^1];
                        FileNode newNode = (FileNode)lastValid.SearchChildren(newNodePath[^1]);
                        
                        // Write from source to destination
                        WriteToFile((FileNode)source.Item1[0], command.Item2[1], newNode);
                    }
                    // If destination doesn't exist and is a single node
                    else
                    {
                        // Create the new FileNode
                        fileSystem.AddFileNode(fileSystem.GetCurrentNode(), newNodePath[0]);
                        FileNode newNode = (FileNode)fileSystem.GetCurrentNode().SearchChildren(newNodePath[0]);
                        
                        // Write from source to destination
                        WriteToFile((FileNode)source.Item1[0], command.Item2[1], newNode);
                    }
                }
            }

            fileSystem.SendOutput("");
            return;
        }
        
        // --- 
        // SECTION THREE
        // ---

        // Display file(s) content
        Tuple<List<Node>, List<Tuple<string, string>>> validArgs = fileSystem.ValidateArgs(command.Item2, "cat");

        foreach (Tuple<string, string> tuple in validArgs.Item2)
        {
            _toOutput.Add(tuple.Item2);
        }
        
        WriteToStdOut(validArgs.Item1);
    }

    // Method called when contents of file(s) is to be written to standard output
        // arguments = the arguments submitted in the command
    private void WriteToStdOut(List<Node> sources)
    {
        foreach (Node src in sources)
        {
            if (src.GetType() == typeof(DirectoryNode))
            {
                _toOutput.Add("cat: " + src.name + ": is a directory");
            }
            else
            {
                string contents = src.GetContents();
                _toOutput.Add(contents ?? "\n");
            }
        }
        
        OutputWithOptions(_toOutput);
    }

    // Method called to write text to a file
        // arguments = the arguments from the command
    // Tuple: Source : Operator : Destination
    private void WriteToFile(FileNode src, string op, FileNode dest)
    {
        if (op == ">")
        {
            dest.SetContents(src.GetContents());
        }
        else
        { 
            dest.SetContents(dest.GetContents() + "\n" + src.GetContents());
        }
    }

    // Method to output content to standard output. Takes a list of strings and processes them before output in line
        // with the options specified in the command
    private void OutputWithOptions(List<string> output)
    {
        // n --> print all line numbers
        // E --> Display $ at the end of the file 
        // none --> standard output

        if (_nOption)
        {
            List<string> finalOutput = new List<string>();

            foreach (string str in output)
            {
                if (!str.Contains("directory"))
                {
                    List<string> lines = str.Split('\n').ToList();

                    int count = 1;
                    foreach (string line in lines)
                    {
                        finalOutput.Add(count + "   " + line); 
                        count++;
                    }
                }
                else
                {
                    finalOutput.Add(str); 
                }
            }

            output = finalOutput;
        }

        if (_eOption)
        {
            output[^1] += "$";
        }
        
        fileSystem.SendOutput(string.Join('\n', output));
    }
}

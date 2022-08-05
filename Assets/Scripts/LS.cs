/*
 * Author: Jacob Hobbs - 190161842
 * Date : July 2022
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LS : MonoBehaviour
{
    // Root command for 'list' - list contents of current directory

    public GraphManager fileSystem;
    private bool _fOption;

    public void ls(string input)
    {
        Debug.Log("--------------");
        Debug.Log(input);
        Tuple<List<char>, List<string>, List<Tuple<string, string>>> command = fileSystem.ValidateOptions(input, new[] { 'a', 'b' }, "ls");
        
        Debug.Log("options " + string.Join('-', command.Item1));
        Debug.Log("arguments " + string.Join('-', command.Item2));
        Debug.Log("errors " + string.Join('-', command.Item3));
        Debug.Log("--------------");

        _fOption = false;
        List<string> listCommand = input.Split(' ').ToList();
        List<char> candidateOptions = new List<char>();
        List<string> output = new List<string>();
        int count = 0;
        
        // foreach (string str in listCommand)
        // {
        //     if (str.StartsWith('-'))
        //     {
        //         foreach (char op in str.Remove('-'))
        //         {
        //             candidateOptions.Add(op);
        //         }
        //         count++;
        //     }
        //     else
        //     {
        //         break;
        //     }
        // }
        
        // string[] candidateArguments = listCommand.Skip(count).ToArray();
        //
        // foreach (char op in candidateOptions)
        // {
        //     if (op == 'F')
        //     {
        //         _fOption = true;
        //     }
        //     else
        //     {
        //         fileSystem.SendOutput("ls: -- " + op + ": illegal option", false);
        //         break;
        //     }
        // }
        //
        // foreach (string str in candidateArguments)
        // {
        //     if (candidateArguments.Length == 0)
        //     {
        //         break;
        //     }
        //
        //     if (candidateArguments.Length == 1)
        //     {
        //         if (str == "$HOME")
        //         {
        //             // Return opts and args
        //             break;
        //         }
        //     }
        //
        //     string[] pathToCheck = str.Split('/');
        //     Tuple<List<Node>, string> path = fileSystem.CheckPath(fileSystem.GetCurrentNode(), pathToCheck, 0, new List<Node>(), false);
        //
        //     if (path.Item2 == null)
        //     {
        //         // valid path
        //         Debug.Log("VALID");
        //     }
        //     else
        //     {
        //         List<string> names = new List<string>();
        //         foreach (Node node in path.Item1)
        //         {
        //             names.Add(node.name);
        //         }
        //         
        //         if (path.Item1.Count > 0)
        //         {
        //             fileSystem.SendOutput("ls: " + string.Join('/', names) + "/" + pathToCheck[path.Item1.Count] + ": " + path.Item2, false);
        //         }
        //         else
        //         {
        //             // Fails on first element in path
        //             fileSystem.SendOutput("ls: " + pathToCheck[0] + ": " + path.Item2, false);
        //         }
        //         
        //     }
        // }
    }
}

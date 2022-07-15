/*
 * Author: Jacob Hobbs - 190161842
 * Date : July 2022
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class CMD : MonoBehaviour
{
    // Directs commands to relevant methods, 'Command' method called when an
    // input is made

    private readonly List<GameObject> _listeners = new List<GameObject>();
    private GraphManager _fileSystem;
    private TMP_InputField _cmdIn;
    private outputText _output;

    void Start()
    {
        AddListeners();
        _fileSystem = FindObjectOfType<GraphManager>();
        _output = FindObjectOfType<outputText>();
        _cmdIn = FindObjectOfType<TMP_InputField>();
        _cmdIn.onSubmit.AddListener(Command);
    }

    private void AddLocalListener(GameObject listener)
    {
        if (!_listeners.Contains(listener))
        {
            _listeners.Add(listener);
        }
    }

    // Split input into individual components then send the arguments to the correct root
    private void Command(string input)
    {
        if (input == "")
        {
            _output.EmptyOut();
            return;
        }
        
        string[] commands = input.Split(new char[] { ' ' });
        GameObject go = _listeners.SingleOrDefault(obj => obj.name == commands[0]);
        string[] options = commands.Skip(1).ToArray();
        string optionsString = string.Join(" ", options);
        
        _fileSystem.SetCurrentCommand(input);

        try
        {
            go.SendMessage(commands[0], optionsString, SendMessageOptions.RequireReceiver);
        }
        catch (NullReferenceException e)
        {
            _fileSystem.SendOutput("Command not found " + commands[0], false);
            Debug.Log(e);
            return;
        }
        
        GameObject cmd = _listeners.SingleOrDefault(obj => obj.name == "prevCmd");
        try
        {
            cmd.SendMessage("PrevCmd", input, SendMessageOptions.RequireReceiver);
        }
        catch (NullReferenceException e)
        {
            Debug.Log(e);
        }
    }

    // Add 'root commands' - used to determine if the input should be forwarded
    // and if so where to
    private void AddListeners()
    {
        GameObject prevCmd = GameObject.Find("prevCmd");
        AddLocalListener(prevCmd);
        GameObject pwd = GameObject.Find("pwd");
        AddLocalListener(pwd);
        GameObject ls = GameObject.Find("ls");
        AddLocalListener(ls);
        GameObject mkdir = GameObject.Find("mkdir");
        AddLocalListener(mkdir);
        GameObject rmdir = GameObject.Find("rmdir");
        AddLocalListener(rmdir);
        GameObject mv = GameObject.Find("mv");
        AddLocalListener(mv);
        GameObject touch = GameObject.Find("touch");
        AddLocalListener(touch);
        GameObject git = GameObject.Find("git");
        AddLocalListener(git);
        GameObject cd = GameObject.Find("cd");
        AddLocalListener(cd);
        GameObject rm = GameObject.Find("rm");
        AddLocalListener(rm);
        GameObject echo = GameObject.Find("echo");
        AddLocalListener(echo);
        GameObject cat = GameObject.Find("cat");
        AddLocalListener(cat);
        AddLocalListener(gameObject);
    }
}

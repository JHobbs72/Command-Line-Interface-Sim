using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using TMPro;

public class CMD : MonoBehaviour
{
    // Directs commands to relevant methods, 'Command' method called when an
    // input is made

    private List<GameObject> listeners = new List<GameObject>();
    private GraphManager fileSystem;
    private TMP_InputField cmdIn;
    private outputText output;
    private autoScroll scroll;

    void Start()
    {
        AddListeners();
        fileSystem = FindObjectOfType<GraphManager>();
        output = FindObjectOfType<outputText>();
        scroll = FindObjectOfType<autoScroll>();
        cmdIn = FindObjectOfType<TMP_InputField>();
        cmdIn.onSubmit.AddListener(Command);
    }

    public void AddListener(GameObject listener)
    {
        if (!listeners.Contains(listener))
        {
            listeners.Add(listener);
        }
    }

    // Split input into individual components then send the 'branch commands' to
    // the correct root
    public void Command(string input)
    {
        if (input == "")
        {
            output.emptyOut();
            return;
        }
        string[] commands = input.Split(new char[] { ' ' });
        GameObject go = listeners.Where(obj => obj.name == commands[0]).SingleOrDefault();
        string[] options = commands.Skip(1).ToArray();
        Debug.Log("Options: " + options);
        string optionsString = string.Join(" ", options);
        
        fileSystem.setCurrentCommand(input);

        try
        {
            go.SendMessage(commands[0], optionsString, SendMessageOptions.RequireReceiver);
        }
        catch (NullReferenceException)
        {
            fileSystem.sendOutput("Command not found " + commands[0]);
            Debug.Log("Not found command: " + string.Join(',', commands));
            return;
        }
        
        GameObject cmd = listeners.Where(obj => obj.name == "prevCmd").SingleOrDefault();
        try
        {
            cmd.SendMessage("prevCmd", input, SendMessageOptions.RequireReceiver);
        }
        catch (NullReferenceException e)
        {
            Debug.Log(e);
        }
        
        // scroll.updateScroll();
    }

    // Add 'root commands' - used to determine if the input should be forwarded
    // and if so where to
    private void AddListeners()
    {
        GameObject prevCmd = GameObject.Find("prevCmd");
        AddListener(prevCmd);
        GameObject pwd = GameObject.Find("pwd");
        AddListener(pwd);
        GameObject ls = GameObject.Find("ls");
        AddListener(ls);
        GameObject mkdir = GameObject.Find("mkdir");
        AddListener(mkdir);
        GameObject rmdir = GameObject.Find("rmdir");
        AddListener(rmdir);
        GameObject mv = GameObject.Find("mv");
        AddListener(mv);
        GameObject touch = GameObject.Find("touch");
        AddListener(touch);
        GameObject git = GameObject.Find("git");
        AddListener(git);
        GameObject cd = GameObject.Find("cd");
        AddListener(cd);
        GameObject rm = GameObject.Find("rm");
        AddListener(rm);
        AddListener(gameObject);
    }
}

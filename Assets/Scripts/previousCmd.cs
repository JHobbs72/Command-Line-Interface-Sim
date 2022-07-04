/*
 * Author: Jacob Hobbs - 190161842
 * Date : July 2022
 */

using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class previousCmd : MonoBehaviour
{
    public TMP_InputField cmdIn;
    private List<string> _cmds = new List<string>();
    private int? _point = null;
    
    // List of previous commands
    // Pointer
    // up arrow moves pointer from back to front of list
    // down arrow moves pointer other way
    // Reset pointer on press enter (command 'submitted')
    // Add command to end of list as it's 'submitted'

    public void EmptyInput()
    {
        cmdIn.text = "";
    }

    public void PrevCmd(string input)
    {
        // remove ""
        if (_cmds.Count > 1)
        {
            _cmds.RemoveAt(_cmds.Count - 1);
        }
        // Add new command
        _cmds.Add(input);
        _cmds.Add("");
        // Reset pointer
        _point = _cmds.Count - 1;
    }

    private void ShowCmd(int? pointer)
    {
        if (pointer != null)
        {
            // Write to textbox
            cmdIn.text = _cmds[(int)pointer];
            cmdIn.caretPosition = _cmds[(int)pointer].Length;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            // decrease pointer
            _point--;
            if (_point < 0)
            {
                _point = 0;
            }
            // Show command pointed at
            ShowCmd(_point);
        }
        
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            // increase pointer
            _point++;
            if (_point < _cmds.Count - 1)
            {
                // Show command pointed at
                ShowCmd(_point);
            }
            else
            {
                _point = _cmds.Count - 1;
                ShowCmd(_point);
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            EmptyInput();
            cmdIn.ActivateInputField();
        }
    }
}

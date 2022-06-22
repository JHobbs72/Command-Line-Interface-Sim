using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class previousCmd : MonoBehaviour
{
    public TMP_InputField cmdIn;
    private List<string> cmds = new List<string>();
    private int? point = null;
    
    // List of previous commands
    // Pointer
    // up arrow moves pointer from back to front of list
    // down arrow moves pointer other way
    // Reset pointer on press enter (command 'submitted')
    // Add command to end of list as it's 'submitted'

    public void emptyInput()
    {
        cmdIn.text = "";
    }

    public void prevCmd(string input)
    {
        // remove ""
        if (cmds.Count > 1)
        {
            cmds.RemoveAt(cmds.Count - 1);
        }
        // Add new command
        cmds.Add(input);
        cmds.Add("");
        // Reset pointer
        point = cmds.Count - 1;

        Debug.Log("previous commands: " + string.Join(',', cmds));
        Debug.Log("Point: " + point);
    }

    private void showCmd(int? pointer)
    {
        if (pointer != null)
        {
            Debug.Log(cmds[(int)pointer]);
            // Write to textbox
            cmdIn.text = cmds[(int)pointer];
            cmdIn.caretPosition = cmds[(int)pointer].Length;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            // decrease pointer
            point--;
            if (point < 0)
            {
                point = 0;
            }
            // Show command pointed at
            showCmd(point);
        }
        
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            // increase pointer
            point++;
            if (point < cmds.Count - 1)
            {
                // Show command pointed at
                showCmd(point);
            }
            else
            {
                point = cmds.Count - 1;
                showCmd(point);
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            emptyInput();
            cmdIn.ActivateInputField();
        }
    }
}

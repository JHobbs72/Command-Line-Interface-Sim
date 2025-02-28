/*
 * Author: Jacob Hobbs - 190161842
 * Date : July 2022
 */

using TMPro;
using UnityEngine;

public class outputText : MonoBehaviour
{
    // Adds output from system to standard output (user's display)

    private TextMeshProUGUI _output;
    private prompt _prompt;

    void Start()
    {
        _output = GetComponent<TextMeshProUGUI>();
        _prompt = FindObjectOfType<prompt>();
    }

    // Method called from GraphManager.cs
        // command = The full command the user entered
        // content = The result of executing that command to be displayed
        // flag = Boolean value dictating whether or not to display the command -- more flexibility 
    public void AddOutput(string command, string content)
    {
        if (content.Length == 0)
        {
            _output.text += "\n>> " + command;
        }
        else if (content.Length > 0)
        {
            _output.text += "\n>> " + command + "\n" + content;
        }

        _prompt.UpdatePrompt();
    }

    // Called after the command has been entered to empty the text box ready for the next command
    public void EmptyOut()
    {
        _output.text += "\n>>";
    }
}

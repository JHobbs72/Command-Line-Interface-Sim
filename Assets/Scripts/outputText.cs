using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class outputText : MonoBehaviour
{
    // Adds output from system to user's 'display'

    private TextMeshProUGUI output;

    void Start()
    {
        output = GetComponent<TextMeshProUGUI>();
        output.text = ">> ";
    }

    public void addOutput(string command, string content)
    {
        if (content.Length == 0)
        {
            output.text += "\n>> " + command;
        }
        else
        {
            output.text += "\n>> " + command + "\n" + content;
        }
    }
}

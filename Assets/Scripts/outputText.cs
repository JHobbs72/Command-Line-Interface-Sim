using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class outputText : MonoBehaviour
{
    // Adds output from system to user's 'display'

    private TextMeshProUGUI output;
    private autoScroll scroll;

    void Start()
    {
        output = GetComponent<TextMeshProUGUI>();
        output.text = ">> ";
        scroll = FindObjectOfType<autoScroll>();
    }

    public void addOutput(string command, string content)
    {
        if (content.Length == 0)
        {
            output.text += "\n>> " + command;
            doScroll();
        }
        else
        {
            output.text += "\n>> " + command + "\n" + content;
            doScroll();
        }
    }

    public void emptyOut()
    {
        output.text += "\n>>";
        
    }

    private void doScroll()
    {
        scroll.updateScroll();
    }
}

/*
 * Author: Jacob Hobbs - 190161842
 * Date : July 2022
 */

using TMPro;
using UnityEngine;

public class outputText : MonoBehaviour
{
    // Adds output from system to user's 'display'

    private TextMeshProUGUI _output;
    private autoScroll scroll;

    void Start()
    {
        _output = GetComponent<TextMeshProUGUI>();
        _output.text = ">> ";
        scroll = FindObjectOfType<autoScroll>();
    }

    public void AddOutput(string command, string content)
    {
        if (content.Length == 0)
        {
            _output.text += "\n>> " + command;
        }
        else
        {
            _output.text += "\n>> " + command + "\n" + content;
        }
    }

    public void EmptyOut()
    {
        _output.text += "\n>>";
    }
}

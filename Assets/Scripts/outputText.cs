using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class outputText : MonoBehaviour
{
    // Adds output from system to user's 'display'

    private TextMeshProUGUI output;

    void Start()
    {
        output = GetComponent<TextMeshProUGUI>();
        output.text = ">> ";
    }

    public void addOutput(string content)
    {
        output.text += "\n>> " + content;
    }
}

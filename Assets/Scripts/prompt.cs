using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class prompt : MonoBehaviour
{
    private TextMeshProUGUI _prompt;
    public GraphManager fileSystem;

    private void Start()
    {
        _prompt = GetComponent<TextMeshProUGUI>();
    }

    public void UpdatePrompt()
    {
        List<Node> currentPath = fileSystem.GetCurrentPath();
        List<string> pathNames = new List<string>();
        foreach (Node node in currentPath)
        {
            pathNames.Add(node.name);
        }
        _prompt.text = string.Join('/', pathNames) + " >> ";
    }
}

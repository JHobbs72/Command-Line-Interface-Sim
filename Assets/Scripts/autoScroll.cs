/*
 * Author: Jacob Hobbs - 190161842
 * Date : July 2022
 */

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class autoScroll : MonoBehaviour
{
    // Auto scroll to mimic real terminal window - latest commands added to the
    // bottom of the window, auto scroll so latest commands are visible

    private Scrollbar _bar;
    [Range(0f, 0.2f)] public float waitRange;

    private void Start()
    {
        _bar = GetComponent<Scrollbar>();
    }

    private void UpdateScroll()
    {
        _bar.value = 0f;
    }
    
    // When return key is hit (command submitted) wait for the command to execute and any output to be displayed
    // then scroll to bottom
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine(DoScroll());
        }
        
        IEnumerator DoScroll()
        {
            yield return new WaitForSeconds(waitRange);
            UpdateScroll();
        }
    }
}

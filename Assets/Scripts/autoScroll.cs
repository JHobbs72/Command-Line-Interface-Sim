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

    private Scrollbar bar;
    [Range(0f, 0.2f)] public float waitRange;

    private void Start()
    {
        bar = GetComponent<Scrollbar>();
    }

    private void UpdateScroll()
    {
        bar.value = 0f;
    }
    
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

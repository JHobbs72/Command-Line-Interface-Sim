using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class autoScroll : MonoBehaviour
{
    // Auto scroll to mimic real terminal window - latest commands added to the
    // bottom of the window, auto scroll so latest commands are visible

    private Scrollbar bar;

    private void Start()
    {
        bar = GetComponent<Scrollbar>();
    }

    public void UpdateScroll()
    {
        bar.value = 0f;
    }
}

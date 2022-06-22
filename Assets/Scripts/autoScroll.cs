using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class autoScroll : MonoBehaviour
{
    // Auto scroll to mimic real terminal window - latest commands added to the
    // bottom of the window, auto scroll so latest commands are visable

    [SerializeField]
    private Scrollbar bar;

    void Start()
    {
        bar.GetComponent<Scrollbar>();
    }

    public void updateScroll()
    {
        if (bar.value != 0)
        {
            bar.value = 0;
        }
    }
}

/*
 * Author: Jacob Hobbs - 190161842
 * Date : July 2022
 */

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

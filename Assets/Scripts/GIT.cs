/*
 * Author: Jacob Hobbs - 190161842
 * Date : July 2022
 */

using UnityEngine;

public class GIT : MonoBehaviour
{
    // Root command for all git sub commands

    public void git(string options)
    {
        Debug.Log("Gitting");
        Debug.Log(options);
    }
}

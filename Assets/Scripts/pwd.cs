using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pwd : MonoBehaviour
{
    void Start()
    {
        CMD go = new CMD();
        go.AddListener(gameObject);
    }
    
}

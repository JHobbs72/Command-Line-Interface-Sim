using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ls : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CMD go = new CMD();
        go.AddListener(gameObject);
    }
}

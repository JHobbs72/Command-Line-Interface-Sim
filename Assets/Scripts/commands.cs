using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class commands : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        CMD cmd = new CMD();
        cmd.AddListener(gameObject);
    }

    
}

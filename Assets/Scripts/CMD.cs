using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CMD : MonoBehaviour
{

    private List<GameObject> listeners = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        AddListener(gameObject);
    }

    public void AddListener(GameObject listener)
    {
        if (!listeners.Contains(listener))
        {
            listeners.Add(listener);
        }
    }

    public void Command(string input)
    {
        string[] commands = input.Split(new char[] { ' ', '(', ')' }, 4);
        GameObject go = listeners.Where(obj => obj.name == commands[0]).SingleOrDefault();
        if (go != null)
        {
            go.SendMessage(commands[1], commands[2], SendMessageOptions.DontRequireReceiver);
        }
    }

    //public void CreateSphere(string input)
    //{
    //    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    //    Debug.Log(input);
    //}

    public void pwd(string input)
    {
        Debug.Log("You're in a directory" + input);
    }
}

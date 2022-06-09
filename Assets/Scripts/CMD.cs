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
        string[] commands = input.Split(new char[] { ' ' }, 4);
        GameObject go = listeners.Where(obj => obj.name == commands[0]).SingleOrDefault();
        string[] options = commands.Skip(1).ToArray();
        string optionsString = string.Join(" ", options);
        if (go != null)
        {
            go.SendMessage(commands[0], options, SendMessageOptions.DontRequireReceiver);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugLevel : MonoBehaviour
{
    public GameObject debug_window;
    public GameObject title;
    public GameObject text;


    public static DebugLevel debug;

    private void Awake()
    {
        debug = this;
    }
    void Start()
    {
        
    }
    void Update()
    {
        
    }

    public static void CallDebug(string command)
    {
        if (command == "stop")
        {
            debug.gameObject.SetActive(false);
        }
        else debug.StartDebug(command);
    }
    public void StartDebug(string command)
    {
        //string[] arguments = command.Split();
    }
    public static void ChangeText(GameObject given_object, string text) // change text of a GameObject
    {
        TextMeshProUGUI textobject = given_object.GetComponent<TextMeshProUGUI>();
        textobject.text = text;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerController : MonoBehaviour
{
    public static float time;
    private static TextMeshProUGUI textobject;
    private void Awake()
    {
        time = 0.0f;
    }
    void Start()
    {
        textobject = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        textobject.text = FormatedTime();
    }
    public static string FormatedTime()
    {
        return FormatedTime(time);
    }
    public static string FormatedTime(float value)
    {
        int minutes, seconds;
        seconds = (int)value;
        minutes = seconds / 60;
        seconds = seconds - minutes * 60;
        string secs = seconds.ToString();
        if(secs.Length < 2)
        {
            secs = "0" + secs;
        }
        return minutes.ToString() + ":" + secs;
    }

    public static float ElapsedTime()
    {
        return time;
    } 
}

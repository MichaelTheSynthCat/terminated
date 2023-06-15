using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

// TaskBarController script is attached to taskbar ui objects that show current processes on operator of player 1 (UI)

public class TaskBarController : MonoBehaviour
{
    public int id = 0; // task bar number, can be 0-4 (example id = 0 is 1st task)

    // text objects and progress bar bject
    public GameObject info_object;
    public GameObject time_object;
    public GameObject progress_bar;
    private TextMeshProUGUI info_text, time_text;

    public const int max_tasks = 5; // maximum tasks present
    private static TaskBarController[] TaskBars = new TaskBarController[max_tasks];
    //private static List<TaskBarController> TaskBars = new List<TaskBarController>(); // container for all ui taskbars
    private void Awake()
    {
        TaskBars[id] = this;
        info_text = info_object.GetComponent<TextMeshProUGUI>();
        time_text = time_object.GetComponent<TextMeshProUGUI>();
    }
    public static void ActivateTaskbar(int id, bool active) // activate taskbar with id (1-5)
        // call when maximum task capacity of player 1 is changed
    {
        TaskBars[id].gameObject.SetActive(active);
    }
    public static void UpdateTaskBar(int id, string info, float time, float progress) // update taskbar info
        // id - position in ui
    {
        TaskBarController taskbar = TaskBars[id];
        taskbar.info_text.text = info;
        taskbar.time_text.text = Math.Round((double)time, 1).ToString("0.0");
        taskbar.progress_bar.transform.localScale = new Vector3(progress, 1, 1);
    }
    private static TaskBarController GetTaskBar(int id) // don't use
    {
        int index = 0;
        for (int i = 0; i < 5; i++)
        {
            if (TaskBars[i].id == id)
            {
                index = i;
                break;
            }
        }
        return TaskBars[index];
    }
}

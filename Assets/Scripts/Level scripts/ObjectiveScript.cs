using INT = System.Int32;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// ObjectiveScript is attached to every standalone objective.
//  Call Fulfilled() method to check if the objective requirements are met.
public class ObjectiveScript : MonoBehaviour
{
    public bool success_objective = true; // condition must be met to fulfill the objective
    private bool active_control = true;
    public string condition; // condition
    public float goal; // optional value slot
    public List<GameObject> targets; // container for devices
    public bool bonus_objective = false;
    public string fail_detail = "@common"; // detail to be displayed if the level is failed

    public List<string> after_completion_commands;
    public List<GameObject> after_completion_targets;

    public static List<ObjectiveScript> objectives = new List<ObjectiveScript>();
    public static int main_objectives = 0;
    public static int bonus_objectives = 0;

    private Dictionary<string, string> common_details = new Dictionary<string, string>()
    {
        {"common", "Objective unfullfilled" },
    };

    private TextMeshProUGUI text_component;
    private string text_template;
    private const string TOKEN = "@";
    private void Awake()
    {
        // clear static variables to prevent errors
        ClearObjectivesOnSceneLoad();
    }
    private void Start()
    {
        objectives.Add(this);
        if (bonus_objective) bonus_objectives++;
        else main_objectives++;
        text_component = GetComponent<TextMeshProUGUI>();
        text_template = text_component.text;
        if (condition == "") Debug.LogWarning("Empty objective condition: " + gameObject.name);
    }
    public bool Fulfilled() // check condition and return true if the requirements are met
    {
        if (condition == "") return !success_objective;
        if (!active_control) return success_objective;
        string[] arguments = condition.Split();
        if (arguments[0] == "overtake") // capture, own devices
        {
            if (arguments[1] == "targets")
            {
                int overtaken = 0;
                int needed = targets.Count;
                foreach(GameObject gobject in targets)
                {
                    if (gobject.GetComponent<DeviceScript>().player == 1) overtaken++;
                }
                FormatText(overtaken, needed);
                return GiveAnswer(needed == overtaken);
            }
            else
            {

            }
        }
        if (arguments[0] == "hold") // defense -> mostly in fail conditions
        {
            if (arguments[1] == "targets")
            {
                bool success = true;
                foreach(GameObject gobject in targets)
                {
                    if (gobject.GetComponent<DeviceScript>().player != 1) success = false;
                }
                return GiveAnswer(success);
            }
        }
        if (arguments[0] == "scan")
        {
            if (arguments[1] == "targets")
            {
                int scanned = 0;
                foreach (GameObject gobject in targets)
                {
                    if (gobject.GetComponent<DeviceScript>().CanPlayerSee(1, true))
                    {
                        scanned++;
                    }
                }
                FormatText(scanned, targets.Count);
                return GiveAnswer(scanned == targets.Count);
            }
        }
        if (arguments[0] == "installed")
        {
            if (arguments[2] == "targets")
            {
                int installed = 0;
                foreach (GameObject gobject in targets)
                {
                    if (gobject.GetComponent<DeviceScript>().player == 1 
                        && gobject.GetComponent<ComputerScript>().GetOwnTask().type == arguments[1])
                    {
                        installed++;
                    }
                }
                FormatText(installed, targets.Count);
                return GiveAnswer(installed == targets.Count);
            }
        }
        if (arguments[0] == "count")
            // count devices meeting the requirements
            // SYNTAX: >count {scope} {requirement} {goal reference}
            // scope: {"global", "targets"}
            // requirement: {all installations}
            // goal reference: {">", ">=", "==", "<=", "<"}
        {
            List<GameObject> scope = null;
            switch (arguments[1])
            {
                case "global":
                    scope = DeviceManagment.GetAllDevicesList();
                    break;
                case "targets":
                    scope = targets;
                    break;
            }
            if (arguments[1].StartsWith("player")) scope = DeviceManagment.AdminDevicesForPlayer(INT.Parse(arguments[1].Split('-')[1]));
            int count = 0;
            switch (arguments[2])
            {
                case "own":
                    foreach (GameObject gobject in scope)
                    {
                        if (gobject.GetComponent<DeviceScript>().player == 1)
                        {
                            count++;
                        }
                    }
                    break;
                case "own_clear_logs":
                    foreach (GameObject gobject in scope)
                    {
                        if (gobject.GetComponent<DeviceScript>().player == 1
                            && gobject.GetComponent<DeviceScript>().Attack_log == null)
                        {
                            count++;
                        }
                    }
                    break;
                case "reign":
                    count = scope.Count;
                    break;
                case CommandsDefinitions.botnet:
                    foreach (GameObject gobject in scope)
                    {
                        if (gobject.GetComponent<ComputerScript>().GetOwnTask().type == CommandsDefinitions.botnet)
                        {
                            count++;
                        }
                    }
                    break;
                case CommandsDefinitions.slave:
                    foreach (GameObject gobject in scope)
                    {
                        if (gobject.GetComponent<DeviceScript>().player == 1
                            && gobject.GetComponent<ComputerScript>().GetOwnTask().type == CommandsDefinitions.slave)
                        {
                            count++;
                        }
                    }
                    break;
            }
            FormatText(count, Mathf.FloorToInt(goal));
            int goal_int = (int)goal;
            switch (arguments[3])
            {
                case ">=": // more or equal goal
                    return GiveAnswer(count >= goal_int);
                case "<=":
                    return GiveAnswer(count <= goal_int);
            }
        }
        if (arguments[0] == "resources")
        // reach resources goal
        // SYNTAX: >resources {resource name}
        // resource name:
        {
            int stash = 0;
            switch (arguments[1])
            {
                case Resources.PCS:
                    stash = PlayerManager.GetPCS(1);
                    break;
            }
            FormatText(stash, (int)goal);
            return GiveAnswer(stash >= (int)goal);
        }
        Debug.LogError("Unknown objective condition - " + gameObject.name + " : " + condition);
        return true;
    }
    public bool GiveAnswer(bool question)
    {
        if(question == success_objective)
        {
            active_control = false;
            SFXPlayer.PlaySFX(SFXPlayer.COMPLETED_OBJECTIVE);
            RunAfterCompletionScript();
        }
        return question;
    }
    private void RunAfterCompletionScript()
    {
        foreach(string line in after_completion_commands)
        {
            UserController.ExecuteLevelCommand(line);
        }
    }
    private void FormatText(string inserted)
    {
        if (!text_template.Contains(TOKEN)) return;
        text_component.text = text_template.Replace(TOKEN, inserted);
    }
    private void FormatText(float value, string format = "0")
    {
        FormatText(value.ToString(format)+"%");
    }
    private void FormatText(int value, int max)
    {
        FormatText(value.ToString()+"/"+max.ToString());
    }
    public string GetFailDetail()
    {
        return fail_detail;
    }
    public void ChangeColor(Color color)
    {
        GetComponent<TextMeshProUGUI>().color = color;
    }
    public static void ClearObjectivesOnSceneLoad()
    {
        objectives.Clear();
        main_objectives = 0;
        bonus_objectives = 0;
    }
}

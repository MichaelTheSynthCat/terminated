using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// LoggerController script manages the Logger window, shows useful info about devices, commands and etc.

public class LoggerController : MonoBehaviour
{
    const float deadline = 10.0f; // deadline for showing command tips
    const string BASIC_HELP = ">help logger\nThis window shows useful informations.";
    private static string current_log; // current static string displayed in the Logger
    private static float time_left;

    private static TextMeshProUGUI textobject;
    void Start()
    {
        textobject = GetComponent<TextMeshProUGUI>();
        current_log = BASIC_HELP;
        ShowLog(current_log);
    }

    void Update()
    {
        if(time_left > 0.0f)
        {
            time_left -= Time.deltaTime;
            if (time_left <= 0.0f)
            {
                SetText(current_log);
            }
        }
    }

    public static void ShowLog(string text) // show static text/message
    {
        current_log = text;
        SetText(current_log);
    }
    public static void ShowQuickLog(string text) // show text only for given time
    {
        time_left = deadline;
        SetText(text);
    }
    public static void StopQuickLog() // stop showing time-based text/message
    {
        time_left = -1.0f;
        SetText(current_log);
    }

    public static void LogSelected(GameObject selected) // show log about a selected device
    {
        string output = "";
        if(selected == null)
        {
            ShowLog(BASIC_HELP);
        }
        else
        {
            DeviceScript device = selected.GetComponent<DeviceScript>();
            output += ">log " + device.GetIP();
            if (device.CanPlayerSee(1, true))
            {
                if(device.player == 1)
                {
                    output += " {login: admin}\n";
                }
                else
                {
                    output += " {login: authorized}\n";
                }
                output += ".name = " + selected.name + "\n" + 
                    ".admin = " + PlayerManager.GetPlayerName(device.player) + "\n" +
                    ".type = " + device.type + "\n";
                if (device.stealth_identity)
                {
                    output += ".identity_stealthed = true" + "\n";
                }
                else
                {
                    output += ".identity_stealthed = false" + "\n";
                }
                output += ".integrity = " + device.GetTrueIntegrity().ToString("0") + "/" 
                    + device.GetTrueIntegrity(true).ToString("0") + "\n" +
                    ".defense = " + device.GetTrueDefense().ToString() + "\n" +
                    ".security_lvl = " + device.GetTrueSecurity().ToString() + "\n";
                if(DeviceManagment.BelongsToCategory(selected, "Computer"))
                {
                    output += ".power = " + device.GetComponent<ComputerScript>().GetTruePower().ToString() + "\n";
                    if(device.player == 1 || device.HasSpyworm(1))
                    {
                        output += ".software = " + device.GetComponent<ComputerScript>().GetOwnTask().type + "\n";
                    }
                }
                if (DeviceManagment.BelongsToCategory(selected, DeviceManagment.ROUTER))
                {
                    RouterScript router = device.GetComponent<RouterScript>();
                    output += ".firewall = " + router.GetTrueFirewallHealth().ToString() + "/"
                        + router.GetTrueFirewallHealth(true).ToString() + "\n";
                    output += ".firewall_lvl = " + router.GetTrueFirewallLevel() + "\n";
                }
            }
            else
            {
                output += " {login: guest}\n";
                output += ".name = " + selected.name + "\n";
                if (device.stealth_identity)
                {
                    output += ".admin = null" + "\n" + 
                        ".identity_stealthed = true" + "\n";
                }
                else
                {
                    output += ".admin = " + PlayerManager.GetPlayerName(device.player) + "\n" + 
                        ".identity_stealthed = false" + "\n";
                }
                output += ".integrity = around " 
                    + (device.GetTrueIntegrity() / device.GetTrueIntegrity(true) * 100).ToString("0.0") + "%\n";
            }
            ShowLog(output);
        }
    }
    private static void SetText(string text) // change text in TextMeshProUGUI component
    {
        textobject.text = text;
    }
}

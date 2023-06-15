using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SysInfoWin : MonoBehaviour
{
    public GameObject operator_info;
    public GameObject codelevels;
    public GameObject bios_upgrades;
    public GameObject installed_count;
    public GameObject installed_text;
    public GameObject network_private;
    public GameObject network_public;
    public void Refresh()
    {
        RefreshSysInfo();
        RefreshInstalled();
        RefreshNetworkInfo();
    }
    public void RefreshSysInfo()
    {
        OperatorScript user_operator = DeviceManagment.GetOperator(1).GetComponent<OperatorScript>();
        string text = "\n";
        if (user_operator.This_device.IsSafe())
        {
            text += "ok\n";
        }
        else
        {
            text += "danger\n";
        }
        if (user_operator.This_device.IsVisibleToEnemies())
        {
            text += "visible\n";
        }
        else
        {
            text += "stealth\n";
        }
        text += user_operator.task_capacity.ToString() + "\n";
        text += ((int)user_operator.This_device.GetTrueIntegrity()).ToString() + "/" +
            ((int)user_operator.This_device.GetTrueIntegrity(true)).ToString() + "\n";
        text += user_operator.This_device.GetTrueDefense().ToString() + "\n";
        text += user_operator.This_device.GetTrueSecurity().ToString() + "\n";
        text += user_operator.GetSuperPower().ToString() + "\n";
        text += user_operator.slaves.Count.ToString() + "/" +
            user_operator.slaves_capacity.ToString() + "\n";
        text += user_operator.stasis_charges.ToString() + "/" +
            user_operator.stasis_charges_max.ToString() + "\n";
        text += user_operator.stasis_cooldown_charge.ToString("0.0") + "s\n";
        text += user_operator.stasis_duration.ToString("0.0") + "s\n";
        if(user_operator.vpn_connection == null)
        {
            text += "disconnected\n";
        }
        else
        {
            text += user_operator.vpn_connection.GetComponent<DeviceScript>().GetIP() + "\n";
        }
        ChangeText(operator_info, text);

        text = "\n";
        text += user_operator.scan_level.ToString() + "\n";
        text += user_operator.tcpcatch_level.ToString() + "\n";
        text += user_operator.antivirus_level.ToString() + "\n";
        text += user_operator.cyberattack_level.ToString() + "\n";
        text += user_operator.trojan_level.ToString() + "\n";
        text += user_operator.break_firewall_level.ToString() + "\n";
        text += user_operator.spyworm_level.ToString() + "\n";
        text += user_operator.botnet_level.ToString() + "\n";
        ChangeText(codelevels, text);

        text = "\n\n";
        text += user_operator.upgrade_boost["defense"].ToString("0.0") + "/" +
            user_operator.defense_upgrade_max.ToString() + "\n";
        text += user_operator.upgrade_boost["security"].ToString("0.0") + "/" +
            user_operator.security_upgrade_max.ToString() + "\n";
        text += user_operator.upgrade_boost["power"].ToString("0.0") + "/" +
            user_operator.power_upgrade_max.ToString() + "\n";
        text += user_operator.upgrade_boost["firewall"].ToString("0.0") + "/" +
            user_operator.firewall_upgrade_max.ToString() + "\n";
        ChangeText(bios_upgrades, text);
    }
    public void RefreshInstalled()
    {
        OperatorScript user_operator = DeviceManagment.GetOperator(1).GetComponent<OperatorScript>();
        ChangeText(installed_count, user_operator.Installed_torrents.Count.ToString()
            + "/" + user_operator.Installed_upgrades.Count.ToString());

        string text = "";
        
        foreach (string key in user_operator.Technology.device_type_bonus.Keys)
        {
            float bonus = user_operator.Technology.device_type_bonus[key];
            if (bonus != 0.0f)
            {
                text += (bonus * 100.0f).ToString() + "% bonus for processes on " + key + "s \n";
            }
        }
        foreach (string key in user_operator.Technology.plugin_bonuses.Keys)
        {
            float bonus = user_operator.Technology.plugin_bonuses[key];
            if(bonus != 0.0f)
            {
                text += (bonus * 100.0f).ToString() + "% " + key + " bonus\n";
            }
        }
        foreach (string key in user_operator.Technology.resources_bonuses.Keys)
        {
            float bonus = user_operator.Technology.resources_bonuses[key];
            if (bonus != 0.0f)
            {
                text += (bonus * 100.0f).ToString() + "% " + key + " generation bonus\n";
            }
        }
        foreach (string key in user_operator.Technology.cost_bonuses.Keys)
        {
            float bonus = user_operator.Technology.cost_bonuses[key];
            if (bonus != 0.0f)
            {
                text += (bonus * 100.0f).ToString() + "% cost of " + key + "\n";
            }
        }
        ChangeText(installed_text, text);
    }
    public void RefreshNetworkInfo()
    {
        int yours = 0, pub = 0, entity = 0, unknown = 0, computer = 0, server = 0, router = 0, other = 0, total=0;
        int safe = 0, danger = 0, damaged = 0, ucomputer = 0, userver = 0, urouter = 0, uother = 0, utotal=0;
        float integrity = 0.0f, defense = 0.0f, security = 0.0f, power = 0.0f, firewall = 0.0f;
        List<GameObject> devices = DeviceManagment.VisibleDevicesForPlayer(1);
        foreach(GameObject gobject in devices)
        {
            DeviceScript device = gobject.GetComponent<DeviceScript>();
            if (!device.CanPlayerSee(1, true) || !device.stealth_identity)
            {
                if (device.player == 1)
                {
                    yours++;
                    if (device.IsSafe())
                    {
                        if (device.GetTrueIntegrity() == device.GetTrueIntegrity(true)) safe++;
                        else damaged++;
                    }
                    else danger++;
                    utotal++;

                    integrity += device.GetTrueIntegrity();
                    defense += device.GetTrueDefense();
                    security += device.GetTrueSecurity();

                    if (DeviceManagment.BelongsToCategory(gobject, DeviceManagment.COMPUTER))
                    {
                        ucomputer++;
                        power += device.GetComponent<ComputerScript>().GetTruePower();
                    }
                    else if (DeviceManagment.BelongsToCategory(gobject, DeviceManagment.SERVER))
                    {
                        userver++;
                        power += device.GetComponent<ComputerScript>().GetTruePower();
                    }
                    else if (DeviceManagment.BelongsToCategory(gobject, DeviceManagment.ROUTER))
                    {
                        urouter++;
                        firewall += device.GetComponent<RouterScript>().GetTrueFirewallLevel();
                    }
                    else uother++;
                }
                else if (device.player == 0) pub++;
                else entity++;
            }
            else unknown++;
            total++;
            if (DeviceManagment.BelongsToCategory(gobject, DeviceManagment.COMPUTER)) computer++;
            if (DeviceManagment.BelongsToCategory(gobject, DeviceManagment.SERVER)) server++;
            if (DeviceManagment.BelongsToCategory(gobject, DeviceManagment.ROUTER)) router++;
            else other++;
        }
        string text = "\n" + safe.ToString() + "\n" + damaged.ToString() + "\n" + danger.ToString() + "\n\n"
            + ucomputer.ToString() + "\n" + userver.ToString() + "\n" + urouter.ToString() + "\n" + uother.ToString() + "\n\n"
            + (integrity / utotal).ToString("0.00") + "\n" + (defense / utotal).ToString("0.00") + "\n"
            + (security / utotal).ToString("0.00") + "\n" + (power / (ucomputer + userver)).ToString("0.00")
            + (firewall / urouter).ToString("0.00");
        ChangeText(network_private, text);
        text = "\n" + yours.ToString() + "\n" + pub.ToString() + "\n" + entity.ToString() + "\n" + other.ToString() + "\n\n"
            + computer.ToString() + "\n" + server.ToString() + "\n" + router.ToString() + "\n" + other.ToString();
        ChangeText(network_public, text);
    }
    public static void ChangeText(GameObject given_object, string text) // change text of a GameObject
    {
        TextMeshProUGUI textobject = given_object.GetComponent<TextMeshProUGUI>();
        textobject.text = text;
    }
}

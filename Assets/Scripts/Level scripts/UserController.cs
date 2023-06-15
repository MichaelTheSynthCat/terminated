using INT = System.Int32;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// UserController controls user's (player 1) most important interaction with the UI.

public class UserController : MonoBehaviour
{
    public List<string> level_commands;
    private static OperatorScript user_operator; // operator of player 1
    void Start()
    {
        // get operator of player 1
        user_operator = DeviceManagment.GetOperator(1).GetComponent<OperatorScript>();
        if(user_operator == null)
        {
            Debug.LogError("Player 1 has no operator device!");
        }
        for (int i = 0; i < 5; i++)
        {
            if(i >= user_operator.task_capacity)
            {
                TaskBarController.ActivateTaskbar(i, false);
            }
        }

        foreach(string command in level_commands)
        {
            ExecuteLevelCommand(command);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!ConsoleController.IsInputActive() && !PauseMenu.IsGamePaused() && !CommandsController.commands.console_input_only) 
            KeybindsController();
    }

    public static void GiveCommand(string command) // give operator a command to do
    {
        GameObject selected = MouseController.GetSelected(); // get selected device GameObject

        DeviceScript device = null; // device script of the selected Gameobject
        string address = "none";
        string output = "";
        if (selected != null)
        {
            device = selected.GetComponent<DeviceScript>();
            address = device.GetIP();
        }

        // find command
        switch (command)
        {
            case "scan":
                if (selected != null)
                {
                    if (device.CanPlayerSee(1))
                    {
                        ShowErrorInConsole(user_operator.Scan(selected));
                    }
                }
                break;
            case "tcpcatch":
                if (selected != null)
                {
                    if (device.CanPlayerSee(1))
                    {
                        ShowErrorInConsole(user_operator.TCPCatch(selected));
                    }
                }
                break;
            case "analyzetasks":
                if(selected != null)
                {
                    output += ">analyze -tasks " + address + "\n\n";
                    output += "Own process:\n";
                    if(device.GetOwnTask(1) == null)
                    {
                        output += "- null\n";
                    }
                    else
                    {
                        if(device.GetOwnTask(1).type == "idle")
                        {
                            output += "- " + device.GetOwnTask(1).type + "\n";
                        }
                        else
                        {
                            output += "- " + device.GetOwnTask(1).type + " on " +
                                device.GetOwnTask(1).target.GetComponent<DeviceScript>().GetIP() + "\n";
                        }
                    }
                    output += "Other tasks:\n";
                    foreach(AttachedTask attachedTask in device.attachedTasks)
                    {
                        if (device.CanPlayerSeeAttachedTask(1, attachedTask) && attachedTask.init_device != device.gameObject)
                        {
                            output += "- " + attachedTask.task_name + " from " +
                                attachedTask.init_device.GetComponent<DeviceScript>().GetIP() + "\n";
                        }
                    }
                    Debug.Log(device.attachedTasks.Count);
                    LoggerController.ShowLog(output);
                }
                break;
            case "antivirusscan":
                if (selected != null)
                {
                    if (device.CanPlayerSee(1))
                    {
                        ShowErrorInConsole(user_operator.AntivirusScan(selected));
                    }
                }
                break;
            case "cyberattack":
                if(selected != null)
                {
                    if(device.CanPlayerSee(1))
                    {
                        ShowErrorInConsole(user_operator.CyberAttack(selected));
                    }
                }
                break;
            case "trojanprocess":
                if (selected != null)
                {
                    if (device.CanPlayerSee(1))
                    {
                        ShowErrorInConsole(user_operator.TrojanProcess(selected));
                    }
                }
                break;
            case "breakfirewall":
                if (selected != null)
                {
                    if (device.CanPlayerSee(1))
                    {
                        ShowErrorInConsole(user_operator.BreakFirewall(selected));
                    }
                }
                break;
            case "injectspyworm":
                if (selected != null)
                {
                    if (device.CanPlayerSee(1))
                    {
                        ShowErrorInConsole(user_operator.InjectSpyworm(selected));
                    }
                }
                break;
            case "deletelog":
                if (selected != null)
                {
                    if (device.CanPlayerSee(1))
                    {
                        ShowErrorInConsole(user_operator.DeleteLog(selected));
                    }
                }
                break;
            case "counterattack":
                if (selected != null)
                {
                    if (device.CanPlayerSee(1))
                    {
                        ShowErrorInConsole(user_operator.Counterattack(selected));
                    }
                }
                break;
            case "createstasis":
                if (selected != null)
                {
                    if (device.CanPlayerSee(1))
                    {
                        ShowErrorInConsole(user_operator.CreateStasis(selected));
                    }
                }
                break;
            case "repair":
                if (selected != null)
                {
                    if (device.CanPlayerSee(1))
                    {
                        ShowErrorInConsole(user_operator.Repair(selected));
                    }
                }
                break;
            case "createvpn":
                if (selected != null)
                {
                    if (device.CanPlayerSee(1))
                    {
                        ShowErrorInConsole(user_operator.CreateVPN(selected));
                    }
                }
                break;
            case "createslave":
                if (selected != null)
                {
                    if (device.CanPlayerSee(1))
                    {
                        ShowErrorInConsole(user_operator.CreateSlave(selected));
                    }
                }
                break;
            case "generate_pcs":
                if (selected != null)
                {
                    if (device.CanPlayerSee(1))
                    {
                        ShowErrorInConsole(user_operator.InstallSoftware(selected, "generate_pcs"));
                    }
                }
                break;
            case "minecatcoin":
                if (selected != null)
                {
                    if (device.CanPlayerSee(1))
                    {
                        ShowErrorInConsole(user_operator.InstallSoftware(selected, "minecatcoin"));
                    }
                }
                break;
            case "installantivirus":
                if (selected != null)
                {
                    if (device.CanPlayerSee(1))
                    {
                        ShowErrorInConsole(user_operator.InstallSoftware(selected, "antivirus"));
                    }
                }
                break;
            case "installautorepair":
                if (selected != null)
                {
                    if (device.CanPlayerSee(1))
                    {
                        ShowErrorInConsole(user_operator.InstallSoftware(selected, "autorepair"));
                    }
                }
                break;
            case "botnet":
                if (selected != null)
                {
                    if (device.CanPlayerSee(1))
                    {
                        ShowErrorInConsole(user_operator.InstallSoftware(selected, "botnet"));
                    }
                }
                break;
            case "uninstall":
                if (selected != null)
                {
                    if (device.CanPlayerSee(1))
                    {
                        ShowErrorInConsole(user_operator.UninstallComputer(selected));
                    }
                }
                break;
            case "openinstall":
                CommandsController.commands.OpenInstallCommands(true);
                break;
            case "openupdatebios":
                CommandsController.commands.OpenUpdateBIOSCommands(true);
                break;
            case "downloaddata":
                if (selected != null)
                {
                    if (device.CanPlayerSee(1))
                    {
                        ShowErrorInConsole(user_operator.DownloadData(selected));
                    }
                }
                break;
            case "terminate":
                if (selected != null)
                {
                    if (device.CanPlayerSee(1))
                    {
                        ShowErrorInConsole(user_operator.TerminateDevice(selected));
                    }
                }
                break;
            case "ping":
                if(selected != null)
                {
                    ConsoleController.ShowText("You said hi to \"" + selected.name + "\" and it's glad it met you!");
                    SFXPlayer.PlaySFX(SFXPlayer.SUCCESS);
                }
                break;
            case "upgrade_defense":
                if (selected != null)
                {
                    if (device.CanPlayerSee(1))
                    {
                        ShowErrorInConsole(user_operator.UpdateBIOS(selected, "defense"));
                    }
                }
                break;
            case "upgrade_security":
                if (selected != null)
                {
                    if (device.CanPlayerSee(1))
                    {
                        ShowErrorInConsole(user_operator.UpdateBIOS(selected, "security"));
                    }
                }
                break;
            case "upgrade_power":
                if (selected != null)
                {
                    if (device.CanPlayerSee(1))
                    {
                        ShowErrorInConsole(user_operator.UpdateBIOS(selected, "power"));
                    }
                }
                break;
            case "upgrade_firewall":
                if (selected != null)
                {
                    if (device.CanPlayerSee(1))
                    {
                        ShowErrorInConsole(user_operator.UpdateBIOS(selected, "firewall"));
                    }
                }
                break;
            case "return":
                CommandsController.commands.OpenMainCommands(true);
                break;
            default:
                Debug.LogError("Unknow command. Check commands!");
                break;
        }

    }
    public static void StartInstallTorrent(string torrent_id, int pcs)
    {
        ShowErrorInConsole(user_operator.InstallTorrent(torrent_id, pcs));
    }
    public static void InstallUpgrade(string product, int catcoin)
    {
        ShowErrorInConsole(user_operator.InstallUpgrade(product, catcoin));
    }
    public static void CancelCommand(string task_number) // cancel task (invoked by a taskbar button)
    {
        user_operator.USERCancelTask(int.Parse(task_number));
    }
    public static void GiveCommandTip(string command) // show command tip/info when mouse is over a command button in Logger
    {
        GameObject selected = MouseController.GetSelected();
        string address = "error";
        string output = "";
        DeviceScript target_device = null;
        if(selected == null)
        {
            address = "null";
        }
        else
        {
            address = selected.GetComponent<DeviceScript>().GetIP();
            target_device = selected.GetComponent<DeviceScript>();
        }
        switch (command)
        {
            case "scan":
                output += ">scan " + address + "\n\n";
                output += "Scan (S)\n";
                output += "Scan the selected device and reveal their attributes and its true identity.\n";
                output += "Codelevel: " + user_operator.scan_level;
                if(target_device != null)
                {
                    output += "\nEstimated process time: " +
                    (user_operator.GetTrueWork(CommandsDefinitions.scan) * user_operator.GetSlavePenalty()
                    / user_operator.GetSuperPower()
                    / user_operator.Technology.SmartGetPluginBonus(CommandsDefinitions.scan, target_device, true)
                    ).ToString("0.0") + "s";
                }
                if (user_operator.GetSlavePenalty() > 1.0f)
                    output += "\nSlave penalty: " + HelperClass.PrintSlavePenalty(user_operator);
                LoggerController.ShowQuickLog(output);
                break;
            case "tcpcatch":
                output += ">tcpcatch " + address + "\n\n";
                output += "Run TCPCatch.exe (Q)\n";
                output += "TCPCatch.exe tries to catch and decypher the TCP protocol flowing through a router" +
                    " and reveal devices connected to the router.\n";
                output += "Codelevel: " + user_operator.tcpcatch_level;
                if (target_device != null)
                {
                    if (target_device.CanPlayerSee(1, true))
                    {
                        output += "\nEstimated process time: " +
                        (user_operator.GetTrueWork(CommandsDefinitions.tcpcatch) * user_operator.GetSlavePenalty()
                        / user_operator.GetSuperPower()
                        / user_operator.Technology.SmartGetPluginBonus(CommandsDefinitions.tcpcatch, target_device, true)
                        ).ToString("0.0") + "s";
                    }
                }
                if (user_operator.GetSlavePenalty() > 1.0f)
                    output += "\nSlave penalty: " + HelperClass.PrintSlavePenalty(user_operator);
                LoggerController.ShowQuickLog(output);
                break;
            case "analyzetasks":
                output += ">help -analyze -tasks -" + address + "\n\n";
                output += "Analyze tasks (K)\n";
                output += "Show tasks running on the selected device.\n";
                output += "Has an immediate effect, does not require task space or minimal code level.";
                LoggerController.ShowQuickLog(output);
                break;
            case CommandsDefinitions.antivirus_scan:
                output += ">antivirus " + address + "\n\n";
                output += "Run Antivirus.exe (T)\n";
                output += "Search for hidden installed threats on the selected device\n";
                output += "Codelevel: " + user_operator.antivirus_level;
                if (target_device != null)
                {
                    output += "\nEstimated process time: " +
                    (user_operator.GetTrueWork(CommandsDefinitions.antivirus_scan) / user_operator.GetSuperPower()
                    / user_operator.Technology.SmartGetPluginBonus(CommandsDefinitions.antivirus_scan, target_device, true)
                    ).ToString("0.0") + "s";
                }
                LoggerController.ShowQuickLog(output);
                break;
            case CommandsDefinitions.cyberattack:
                output += ">cyberattack " + address + "\n\n";
                output += "Cyberattack (A)\n";
                output += "Cyberattack the selected device and decrease its integrity and take control over it.\n";
                output += "Codelevel: " + user_operator.cyberattack_level;
                if (target_device != null)
                {
                    if (!target_device.CanPlayerSee(1, true))
                    {
                        LoggerController.ShowQuickLog(output);
                        break;
                    }
                    output += "\nEstimated process time: " +
                    (target_device.GetTrueDefense() / user_operator.GetSuperPower()
                    / user_operator.Technology.SmartGetPluginBonus(CommandsDefinitions.cyberattack, target_device, true)
                    * target_device.GetTrueIntegrity()
                    ).ToString("0.0") + "s";
                }
                LoggerController.ShowQuickLog(output);
                break;
            case CommandsDefinitions.trojan:
                output += ">trojan " + address + "\n\n";
                output += "Trojan executable (T)\n";
                output += "Use Trojan horse program to stealthly attack the selected device and decrease its integrity and take control over it.\n";
                output += "Codelevel: " + user_operator.trojan_level;
                if (target_device != null)
                {
                    if (!target_device.CanPlayerSee(1, true))
                    {
                        LoggerController.ShowQuickLog(output);
                        break;
                    }
                    output += "\nEstimated process time: " +
                    (target_device.GetTrueDefense() / user_operator.GetSuperPower() / 0.5f
                    / user_operator.Technology.SmartGetPluginBonus(CommandsDefinitions.trojan, target_device, true)
                    * target_device.GetTrueIntegrity()
                    ).ToString("0.0") + "s";
                }
                LoggerController.ShowQuickLog(output);
                break;
            case CommandsDefinitions.break_firewall:
                output += ">breakfirewall " + address + "\n\n";
                output += "Break Firewall (T)\n";
                output += "Break router's Firewall to make it vulnerable to cyberattacks.\n";
                output += "Codelevel: " + user_operator.break_firewall_level;
                if (target_device != null)
                {
                    if (!target_device.CanPlayerSee(1, true) || !DeviceManagment.BelongsToCategory(selected, DeviceManagment.ROUTER))
                    {
                        LoggerController.ShowQuickLog(output);
                        break;
                    }
                    output += "\nEstimated process time: " +
                    ((target_device.GetTrueDefense() +
                    (target_device.GetTrueDefense() * target_device.GetComponent<RouterScript>().GetTrueFirewallLevel() * 0.1f))
                    / user_operator.GetSuperPower()
                    / user_operator.Technology.SmartGetPluginBonus(CommandsDefinitions.break_firewall, target_device, true)
                    * target_device.GetTrueIntegrity()
                    ).ToString("0.0") + "s";
                }
                LoggerController.ShowQuickLog(output);
                break;
            case CommandsDefinitions.inject_spyworm:
                output += ">spyworm " + address + "\n\n";
                output += "Inject spyworm (I)\n";
                output += "Inject spyworm on the selected device to see its activity\n";
                output += "Codelevel: " + user_operator.scan_level;
                if (target_device != null)
                {
                    if (!target_device.CanPlayerSee(1, true))
                    {
                        LoggerController.ShowQuickLog(output);
                        break;
                    }
                    output += "\nEstimated process time: " +
                    (target_device.GetTrueDefense() / user_operator.GetSuperPower() / 0.05f
                    / user_operator.Technology.SmartGetPluginBonus(CommandsDefinitions.trojan, target_device, true)
                    * target_device.GetTrueIntegrity()
                    ).ToString("0.0") + "s";
                }
                output += "\nPCS cost: " + user_operator.GetTrueCost(CommandsDefinitions.inject_spyworm) + "\n";
                LoggerController.ShowQuickLog(output);
                break;
            case "deletelog":
                output += ">deletelog " + address + "\n\n";
                output += "Delete Log (V)\n";
                output += "Delete log on the seected device to not prevent others from knowing who hacked this device.\n";
                if (target_device != null)
                {
                    output += "\nEstimated process time: " +
                    (user_operator.GetTrueWork(CommandsDefinitions.delete_log) * user_operator.GetSlavePenalty()
                    / user_operator.GetSuperPower()
                    / user_operator.Technology.SmartGetPluginBonus(CommandsDefinitions.delete_log, target_device, true)
                    ).ToString("0.0") + "s";
                }
                if (user_operator.GetSlavePenalty() > 1.0f)
                    output += "\nSlave penalty: " + HelperClass.PrintSlavePenalty(user_operator);
                LoggerController.ShowQuickLog(output);
                break;
            case "counterattack":
                output += ">counter " + address + "\n\n";
                output += "Counterattack (C)\n";
                output += "Counterattack the device attacking your device\n";
                output += "Estimated process time: " + "unknown";
                LoggerController.ShowQuickLog(output);
                break;
            case CommandsDefinitions.create_stasis:
                output += ">stasis " + address + "\n\n";
                output += "Create Stasis (C)\n";
                output += "Create stasis that blocks all incoming attacks\n";
                output += "Stasis duration: " + user_operator.stasis_duration;
                output += "Stasis charges: " + user_operator.stasis_charges.ToString() + "/"
                    + user_operator.stasis_charges_max.ToString() + 
                    "\nNext charge in: ";
                if(user_operator.stasis_charges == user_operator.stasis_charges_max)
                {
                    output += "null\n";
                }
                else
                {
                    output += user_operator.Stasis_recharge_timer + "s\n";
                }
                if (target_device != null)
                {
                    output += "\nEstimated process time: " +
                    (user_operator.GetTrueWork(CommandsDefinitions.create_stasis) / user_operator.GetSuperPower()
                    / user_operator.Technology.SmartGetPluginBonus(CommandsDefinitions.create_stasis, target_device, true)
                    ).ToString("0.0") + "s";
                }
                output += "\nPCS cost: " + user_operator.GetTrueCost(CommandsDefinitions.create_stasis) + "\n";
                LoggerController.ShowQuickLog(output);
                break;
            case "repair":
                output += ">repair " + address + "\n\n";
                output += "Repair (R)\n";
                output += "Repair device's integrity or firewall\n";
                output += "Estimated process time: " + "unknown\n";
                LoggerController.ShowQuickLog(output);
                break;
            case "createvpn":
                output += ">help -createvpn -" + address + "\n\n";
                output += "Estabilish VPN connection (V)\n";
                output += "Estabilish VPN connection with your own selected computer, which grant you changed IP address.\n";
                output += "Estimated process time: " + "unknown";
                LoggerController.ShowQuickLog(output);
                break;
            case "createslave":
                output += ">createslave " + address + "\n\n";
                output += "Create slave (S)\n";
                output += "Make your own computer into a slave and increase your operator's computing power\n";
                if (target_device != null)
                {
                    if (DeviceManagment.BelongsToCategory(target_device.gameObject, DeviceManagment.COMPUTER))
                        output += "\nEstimated process time: " +
                        (user_operator.GetTrueWork(CommandsDefinitions.create_slave, target_device)
                        / (user_operator.GetSuperPower() * 0.5f + target_device.GetComponent<ComputerScript>().GetTruePower())
                        / user_operator.Technology.SmartGetPluginBonus(CommandsDefinitions.create_slave, target_device, true)
                        ).ToString("0.0") + "s";
                }
                output += "\nPCS cost: " + user_operator.GetTrueCost(CommandsDefinitions.create_slave) + "\n";
                output += "Penalty per slave: 50%\n" + "\nSlave penalty: " + HelperClass.PrintSlavePenalty(user_operator);
                LoggerController.ShowQuickLog(output);
                break;
            case "generate_pcs":
                output += ">install pcs_generator " + address + "\n\n";
                output += "Install PCS generator (G)\n";
                output += "Install on the selected computer software that generates PCS (pre-compiled scripts)\n";
                if (target_device != null)
                {
                    if (DeviceManagment.BelongsToCategory(target_device.gameObject, DeviceManagment.COMPUTER))
                        output += "\nEstimated process time: " +
                        (user_operator.GetTrueWork(CommandsDefinitions.install_software, target_device)
                        / (user_operator.GetSuperPower() * 0.5f + target_device.GetComponent<ComputerScript>().GetTruePower())
                        / user_operator.Technology.SmartGetPluginBonus(CommandsDefinitions.install_software, target_device, true)
                        ).ToString("0.0") + "s";
                }
                output += "\nPCS cost: FREE\n";
                LoggerController.ShowQuickLog(output);
                break;
            case "minecatcoin":
                output += ">help -install -minecatcoin -" + address + "\n\n";
                output += "Install CatCoinMiner.exe (L)\n";
                output += "Install on the selected computer software that mines CatCoin\n";
                if (target_device != null)
                {
                    output += "\nEstimated process time: " +
                    (user_operator.GetTrueWork(CommandsDefinitions.install_software, target_device) / user_operator.GetSuperPower()
                    / user_operator.Technology.SmartGetPluginBonus(CommandsDefinitions.install_software, target_device, true)
                    ).ToString("0.0") + "s";
                }
                output += "\nPCS cost: " + user_operator.GetTrueCost(CommandsDefinitions.mine_catcoin, target_device) + "\n";
                LoggerController.ShowQuickLog(output);
                break;
            case "installantivirus":
                output += ">install antivirus -" + address + "\n\n";
                output += "Install Antivirus (L)\n";
                output += "Install on the selected computer antivirus\n";
                if (target_device != null)
                {
                    output += "\nEstimated process time: " +
                    (user_operator.GetTrueWork(CommandsDefinitions.install_software, target_device) / user_operator.GetSuperPower()
                    / user_operator.Technology.SmartGetPluginBonus(CommandsDefinitions.install_software, target_device, true)
                    ).ToString("0.0") + "s";
                }
                output += "\nPCS cost: " + user_operator.GetTrueCost(CommandsDefinitions.autoantivirus, target_device) + "\n";
                LoggerController.ShowQuickLog(output);
                break;
            case "installautorepair":
                output += ">help -install -autorepair -" + address + "\n\n";
                output += "Install Autorepair (L)\n";
                output += "Install autorepair\n";
                output += "Estimated process time: " + "unknown";
                LoggerController.ShowQuickLog(output);
                break;
            case "botnet":
                output += ">help -install -botnet -" + address + "\n\n";
                output += "Install BotNet.exe (L)\n";
                output += "Install on the selected computer botnet\n";
                output += "Estimated process time: " + "unknown";
                LoggerController.ShowQuickLog(output);
                break;
            case "uninstall":
                output += ">uninstall " + address + "\n\n";
                output += "Uninstall software from computer (U)\n";
                output += "Click to immediately uninstall software from the selected device and stop its current task";
                LoggerController.ShowQuickLog(output);
                break;
            case "openinstall":
                output += ">browse install\n\n";
                output += "Show available installs(L)\n";
                output += "Browse all available software to install";
                LoggerController.ShowQuickLog(output);
                break;
            case "openupdatebios":
                output += ">help -open_update_bios\n\n";
                output += "Show all available BIOS updates (L)\n";
                output += "-WIP-";
                LoggerController.ShowQuickLog(output);
                break;
            case "upgrade_defense":
                output += ">help -upgrade -defense\n\n";
                output += "Upgrade defense (Q)\n";
                output += "-WIP-";
                LoggerController.ShowQuickLog(output);
                break;
            case "upgrade_security":
                output += ">help -upgrade -security\n\n";
                output += "Upgrade security (Q)\n";
                output += "-WIP-";
                LoggerController.ShowQuickLog(output);
                break;
            case "upgrade_power":
                output += ">help -upgrade -power\n\n";
                output += "Upgrade power (Q)\n";
                output += "-WIP-";
                LoggerController.ShowQuickLog(output);
                break;
            case "upgrade_firewall":
                output += ">help -upgrade -firewall\n\n";
                output += "Upgrade firewall (Q)\n";
                output += "-WIP-";
                LoggerController.ShowQuickLog(output);
                break;
            case "downloaddata":
                output += ">help -download -" + address + "\n\n";
                output += "Download data (L)\n";
                output += "Download data from the selected device";
                LoggerController.ShowQuickLog(output);
                break;
            case "terminate":
                output += ">help -terminate -" + address + "\n\n";
                output += "Terminate device (R)\n";
                output += "TERMINATE";
                LoggerController.ShowQuickLog(output);
                break;
            case "ping":
                output += ">ping " + address+"\n\n";
                output += "Ping (P)\n";
                output += "Say hello to the selected device";
                LoggerController.ShowQuickLog(output);
                break;
            case "return":
                output += ">return\n\n";
                output += "Return to main commands (M)\n";
                output += "Close this window and show main commands";
                LoggerController.ShowQuickLog(output);
                break;
            case "canceltask":
                output += ">cancel {task #}\n\n";
                output += "Cancel Task (Shift + num)\n";
                output += "Click to cancel the current process.\nIf PCS were used, they won't be refunded.";
                LoggerController.ShowQuickLog(output);
                break;
            default:
                Debug.LogError("UserController.GiveCommandTip: Unknow command "+command+". Check commands!");
                break;
        }
    }
    public static void StopCommandTip() // stop showing command tip/info when mouse exits the space over the button
    {
        LoggerController.StopQuickLog();
    }

    private static void ShowErrorInConsole(string text)
    {
        if(text[0] == '-')
        {
            SFXPlayer.PlaySFX(SFXPlayer.ERROR);
            ConsoleController.ShowQuickText(text.Substring(1));
        }
        else
        {
            SFXPlayer.PlaySFX(SFXPlayer.OK);
        }
    }

    public static void ExecuteLevelCommand(string line)
    {
        string[] arguments = line.Split();
        switch (arguments[0])
        {
            case "reveal":
                DeviceScript device = DeviceManagment.GetDeviceByIP(arguments[1]).GetComponent<DeviceScript>();
                device.ChangeVisibility(INT.Parse(arguments[2]), true);
                break;
            default:
                Debug.LogError("Unknown level command: " + line);
                break;
        }
    }
    public static void ExecuteConsoleCommand(string line)
    {
        string[] arguments = line.Split();
        switch (arguments[0])
        {
            case "look":
                CameraController.LookAt(DeviceManagment.GetDeviceByIP(arguments[1]), true);
                return;
        }
        ConsoleController.ShowQuickText("Invalid command");
    }
    public static void KeybindsController()
    {
        /* ALL KEYBINDS - main menu
         * a - cyberAttack
         * b - update Bios
         * c - Counterattack
         * d - create stasis (Defend)
         * e - antivirus scan
         * f - break Firewall
         * g
         * h
         * i - Install software
         * j |
         * k |
         * l |
         * m |
         * n |
         * o |
         * p - Ping
         * q - tcp catch
         * r - Repair
         * s - Scan
         * t - Terminate
         * v - delete logs (Vanish)
         * u - Uninstall software
         * w - doWnload
         * x - trojan ?
         * y - spYworm
         * z
         * 
         * Keybinds Install software
         * g - pcs Generator
         * c - Catcoin miner
         * v - Vpn gate
         * s - Slave
         * a - Autoantivirus
         * e - autorEpair
         * b - Botnet
         * r - Return
         * 
         * Keybinds Update BIOS
         * d - Defense
         * s - Security level
         * a - power ?
         * f - Firewall level
         */
        GameObject selected = MouseController.GetSelected();
        switch (CommandsController.openned_menu)
        {
            case "MAIN":
                if (selected != null)
                {
                    if (Input.GetKeyDown("s") && CommandsController.commands.scan)
                    {
                        ShowErrorInConsole(user_operator.Scan(selected));
                    }
                    if (Input.GetKeyDown("q") && CommandsController.commands.tcp)
                    {
                        ShowErrorInConsole(user_operator.TCPCatch(selected));
                    }
                    if (Input.GetKeyDown("e") && CommandsController.commands.antivirus)
                    {
                        ShowErrorInConsole(user_operator.AntivirusScan(selected));
                    }
                    if (Input.GetKeyDown("a") && CommandsController.commands.cyberattack)
                    {
                        ShowErrorInConsole(user_operator.CyberAttack(selected));
                    }
                    if (Input.GetKeyDown("x") && CommandsController.commands.trojan)
                    {
                        ShowErrorInConsole(user_operator.TrojanProcess(selected));
                    }
                    if (Input.GetKeyDown("f") && CommandsController.commands.break_firewall)
                    {
                        ShowErrorInConsole(user_operator.BreakFirewall(selected));
                    }
                    if (Input.GetKeyDown("y") && CommandsController.commands.spyworm)
                    {
                        ShowErrorInConsole(user_operator.InjectSpyworm(selected));
                    }
                    if (Input.GetKeyDown("t") && CommandsController.commands.terminate)
                    {
                        ShowErrorInConsole(user_operator.TerminateDevice(selected));
                    }
                    if (Input.GetKeyDown("c") && CommandsController.commands.counterattack)
                    {
                        ShowErrorInConsole(user_operator.Counterattack(selected));
                    }
                    if (Input.GetKeyDown("r") && CommandsController.commands.repair)
                    {
                        ShowErrorInConsole(user_operator.Repair(selected));
                    }
                    if (Input.GetKeyDown("d") && CommandsController.commands.create_stasis)
                    {
                        ShowErrorInConsole(user_operator.CreateStasis(selected));
                    }
                    if (Input.GetKeyDown("v") && CommandsController.commands.delete_log)
                    {
                        ShowErrorInConsole(user_operator.DeleteLog(selected));
                    }
                    if (Input.GetKeyDown("u") && CommandsController.commands.uninstall)
                    {
                        ShowErrorInConsole(user_operator.UninstallComputer(selected));
                    }
                    if (Input.GetKeyDown("w") && CommandsController.commands.download)
                    {
                        ShowErrorInConsole(user_operator.DownloadData(selected));
                    }
                    if (Input.GetKeyDown("p") && CommandsController.commands.ping)
                    {
                        GiveCommand("ping");
                    }
                }
                if (Input.GetKeyDown("i") && CommandsController.commands.install)
                {
                    CommandsController.commands.OpenInstallCommands(true);
                }
                if (Input.GetKeyDown("b") && CommandsController.commands.update_bios)
                {
                    CommandsController.commands.OpenUpdateBIOSCommands(true);
                }
                break;
            case "INSTALL":
                if (selected != null)
                {
                    if (Input.GetKeyDown("g") && CommandsController.commands.generate_pcs)
                    {
                        ShowErrorInConsole(user_operator.InstallSoftware(selected, CommandsDefinitions.generate_pcs));
                    }
                    if (Input.GetKeyDown("c") && CommandsController.commands.mine_catcoin)
                    {
                        ShowErrorInConsole(user_operator.InstallSoftware(selected, CommandsDefinitions.mine_catcoin));
                    }
                    if (Input.GetKeyDown("v") && CommandsController.commands.vpn)
                    {
                        ShowErrorInConsole(user_operator.CreateVPN(selected));
                    }
                    if (Input.GetKeyDown("s") && CommandsController.commands.create_slave)
                    {
                        ShowErrorInConsole(user_operator.CreateSlave(selected));
                    }
                    if (Input.GetKeyDown("a") && CommandsController.commands.install_antivirus)
                    {
                        ShowErrorInConsole(user_operator.InstallSoftware(selected, CommandsDefinitions.autoantivirus));
                    }
                    if (Input.GetKeyDown("e") && CommandsController.commands.install_autorepair)
                    {
                        ShowErrorInConsole(user_operator.InstallSoftware(selected, CommandsDefinitions.autorepair));
                    }
                    if (Input.GetKeyDown("b") && CommandsController.commands.install_botnet)
                    {
                        ShowErrorInConsole(user_operator.InstallSoftware(selected, CommandsDefinitions.botnet));
                    }
                }
                if (Input.GetKeyDown("r"))
                {
                    CommandsController.commands.OpenMainCommands(true);
                }
                break;
            case "BIOS":
                if (selected != null)
                {
                    if (Input.GetKeyDown("d") && CommandsController.commands.upgrade_defense)
                    {
                        ShowErrorInConsole(user_operator.UpdateBIOS(selected, Upgrades.DEFENSE));
                    }
                    if (Input.GetKeyDown("s") && CommandsController.commands.upgrade_security)
                    {
                        ShowErrorInConsole(user_operator.UpdateBIOS(selected, Upgrades.SECURITY));
                    }
                    if (Input.GetKeyDown("a") && CommandsController.commands.upgrade_power)
                    {
                        ShowErrorInConsole(user_operator.UpdateBIOS(selected, Upgrades.POWER));
                    }
                    if (Input.GetKeyDown("f") && CommandsController.commands.upgrade_firewall)
                    {
                        ShowErrorInConsole(user_operator.UpdateBIOS(selected, Upgrades.FIREWALL));
                    }
                }
                if (Input.GetKeyDown("r"))
                {
                    CommandsController.commands.OpenMainCommands(true);
                }
                break;
            default:
                Debug.LogError("UserController: KeybindsController - invalid command menu key = " 
                    + CommandsController.openned_menu);
                break;
        }
        
    }
    public static GameObject FindDeviceByString(string key)
    {
        switch (key)
        {
            case "operator":
                return user_operator.gameObject;
            case "selected":
                return MouseController.GetSelected();
            default:
                return DeviceManagment.GetDeviceByIP(key);
        }
    }
}

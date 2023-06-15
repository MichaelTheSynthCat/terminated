using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericAI : MonoBehaviour
{
    public OperatorScript This_operator { get; private set; }
    public int difficulty = 10;
    // 10 - TERMINATOR, 7 - hard, 5 - normal, 2 - easy
    public string behaviour = "hacker";
    // hacker - attack everything
    // hunter - attack hackers
    // terminator - terminate foreign devices
    // IT - defend own devices
    public string attack_strategy = "smart";
    // smart - cyberattack enemies, trojan public
    // force - cyberattack only
    // stealth - trojan only, firewall breaking only with vpn
    // random - randomised
    public bool attack_routers = true; // break firewalls of routers
    public bool use_botnet = true; // install botnets on computers
    public string log_clearing = "normal";
    // normal - delete logs often
    // safe - always delete logs
    // rare - sometimes delete logs
    // none - don't delete logs
    public string defense_strategy = "smart";
    // smart - counterattack cyberattacks, search trojans, use stasis, use antivirus appropriately + priority behaviour
    // safe - defense is main priority, always counterattack, use antivirus very often, install local antiviruses, stasis very often
    // casual - sometimes counterattack, use antivirus rarely, stasis only in emergency + priority behaviour
    // priority - defend only high priority devices and main infrastructure + emergency behaviour
    // emergency - defend only operator and vpn device
    // unsafe - do nothing
    public bool local_antiviruses = true;
    public bool repair_damaged = true;
    public bool local_autorepair = true;
    public string spying_strategy = "normal";
    // normal - sometimes install spyware
    // high - install spyware very often
    // none - don't use spyware
    public bool use_vpn = true; // create vpn gates
    public int pcs_generators = 5;
    public string upgrades_strategy = "smart";
    // smart - smart upgrade selection and installation on high priority devices and infrastructure
    public List<GameObject> high_priority_devices;
    public List<GameObject> terminate_devices;
    private void Awake()
    {
        This_operator = GetComponent<OperatorScript>();
    }
    void Start()
    {
        
    }
    void Update()
    {
        if (Idle())
        {
            AttackSession();
        }
    }
    public bool Idle()
    {
        return This_operator.EmptyTaskSpace();
    }
    public void AttackSession()
    {
        List<GameObject> visible = DeviceManagment.VisibleDevicesForPlayer(This_operator.Player_operator, true);
        List<GameObject> enemies = new List<GameObject>();
        foreach(GameObject gobject in visible)
        {
            if(attack_routers && DeviceManagment.BelongsToCategory(gobject, "Router"))
            {
                continue;
            }
            if (!gobject.GetComponent<DeviceScript>().IsFriendlyWith(This_operator.Player_operator))
            {
                enemies.Add(gobject);
            }
        }
        List<GameObject> attackable_cyber = new List<GameObject>();
        List<GameObject> attackable_trojan = new List<GameObject>();
        List<GameObject> attackable_firewall = new List<GameObject>();
        foreach(GameObject enemy in enemies)
        {
            DeviceScript device = enemy.GetComponent<DeviceScript>();
            if(CommandPossible.IsPossible(CommandPossible.Cyberattack(device, This_operator)))
            {
                attackable_cyber.Add(enemy);
            }
            if(CommandPossible.IsPossible(CommandPossible.Trojan(device, This_operator)))
            {
                attackable_trojan.Add(enemy);
            }
            // add break firewall
        }
        switch (attack_strategy)
        {
            case "smart":
                GameObject target = SelectRandomDevice(attackable_cyber);
                if(target.GetComponent<DeviceScript>().player == 0 || !attackable_trojan.Contains(target))
                {
                    This_operator.CyberAttack(target);
                }
                else
                {
                    This_operator.TrojanProcess(target);
                }
                break;
            case "force":
                This_operator.CyberAttack(SelectRandomDevice(attackable_cyber));
                break;
            case "stealth":
                // code
                break;
            default:
                Debug.LogError("Invalid strategy " + attack_strategy);
                break;
        }
    }
    public void DefendSessionEmergency()
    {
        List<GameObject> own_devices = DeviceManagment.AdminDevicesForPlayer(This_operator.Player_operator);
        List<GameObject> in_danger = new List<GameObject>();
        foreach(GameObject gobject in own_devices)
        {
            if (!gobject.GetComponent<DeviceScript>().IsSafe())
            {
                in_danger.Add(gobject);
            }
        }
        switch (defense_strategy)
        {
            case "emergency":
                if (!This_operator.This_device.IsSafe())
                {
                    Defend(This_operator.gameObject);
                }
                break;
        }
    }
    public void DefendSessionPrevention()
    {

    }
    public void Defend(GameObject target, bool emergency=false)
    {
        DeviceScript device = target.GetComponent<DeviceScript>();
        if(device.GetTrueIntegrity() / device.GetTrueIntegrity(true) < 0.25)
        {
            if(This_operator.stasis_charges > 0)
            {
                This_operator.CreateStasis(target);
                if (!Idle())
                {
                    return;
                }
            }
        }
        if (device.IsUnderStealthAttack())
        {
            This_operator.AntivirusScan(target);
        }
    }
    public static GameObject SelectRandomDevice(List<GameObject> list)
    {
        return list[Random.Range(0, list.Count)];
    }
}

public class CommandPossible
{
    // class that controls if process/action is possible on a certain device
    public const string OK = "+ok";
    public static Dictionary<string, string> USER_START_ERROR_MESSAGES = new Dictionary<string, string>()
    {
        {"-not visible", "Device on the given IP address is unreachable" },
        {"-terminated", "Can't run processes on terminated devices" },
        {"-duplicate", "Task is already running" },
        {"-not admin", "Must have admin access to perform this operation" },
        {"-friendly", "Target foreign devices" },
        {"-unscanned", "Can't perform this operation without full scan" },
        {"-attacking", "Already attacking with another process" },
        {"-firewall", "Device is protected by active firewall" },
        {"-stasis", "Device is protected by active stasis" },
        {"-not computer", "Target must be a computer" },
        {"-operator selected", "Can't do this operation on operator" },
        {"-has task", "Computer already has a task, uninstall it first" },
        {"-install active", "Already installing software on this computer" },
        {"-not enough PCS", "You need more PCS" },
        {"-already scanned", "Device is already scanned" },
        {"-security", "Your code level must be higher than the security level" },
        {"-not router", "This process is determined for routers" },
        {"-firewall broken", "Firewall is already breached" },
        {"-firewall level", "Your code level must be higher than the firewall level" },
        {"-spyworm active", "Spyworm already injected" },
        {"-log empty", "Device's log is empty" },
        {"-no visible attack", "Device is not under visible attack" },
        {"-stasis already", "Device is already protected by stasis" },
        {"-no stasis charges", "Not enough stasis charges" },
        {"-danger", "Device is not in safe mode" },
        {"-full integrity", "Device is in full condition" },
        {"-vpn already", "Disconnect from active VPN gate" },
        {"-one task limit", "Only one task of this type can exist in one moment" },
        {"-slaves max", "Reached maximum slaves capacity"},
        {"-update active", "Already updating BIOS on this device" },
        {"-not computer or server", "Target must be a computer or server" },
        {"-upgrade max", "Reached maximal level of update" },
        {"-no files", "Nothing to download" },
        {"-self termination", "SELF-TERMINATION DENIED" },
        {"-idle", "No software to uninstall" },
    };
    public static Dictionary<string, string> USER_PROCESS_ERROR_MESSAGES = new Dictionary<string, string>()
    {
        {"-not visible", "lost connection" },
        {"-terminated", "device terminated" },
        {"-not admin", "lost admin access" },
        {"-friendly", "friendly device" },
        {"-stasis", "active stasis" },
        {"-security", "high security" },
        {"-danger", "device in danger" },
        {"-no visible attack", "not under attack" },
    };
    public static bool IsPossible(string message)
    {
        return message.StartsWith("+");
    }
    public static string GetUserStartErrorMessage(string detail, bool minus_included = true)
    {
        if (!USER_START_ERROR_MESSAGES.ContainsKey(detail))
        {
            Debug.LogError("USER START ERROR MESSAGE key error: " + detail);
            return "-unity error";
        }
        if (minus_included) return "-" + USER_START_ERROR_MESSAGES[detail];
        return USER_START_ERROR_MESSAGES[detail];
    }
    public static string GetUserProcessErrorMessage(string detail, bool minus_included = true)
    {
        if (!USER_PROCESS_ERROR_MESSAGES.ContainsKey(detail))
        {
            Debug.LogError("USER PROCESS ERROR MESSAGE key error: " + detail);
            return "-unity error";
        }
        if (minus_included) return "-" + USER_PROCESS_ERROR_MESSAGES[detail];
        return USER_PROCESS_ERROR_MESSAGES[detail];
    }
    public static string CommandOnDevice(DeviceScript device, OperatorScript source, string command)
    {
        // any process must be verified with this function
        if (!device.CanPlayerSee(source.Player_operator))
        {
            // player does not see the device (not visible)
            return "-not visible";
        }
        if (device.IsTerminated())
        {
            // device is terminated and no actions can be performed on terminated devices
            return "-terminated";
        }
        if (source.DuplicateTaskExists(command, device))
        {
            // process is already processed by the operator/source
            return "-duplicate";
        }
        return "+ok";
    }
    public static string CommandWithAdmin(DeviceScript device, OperatorScript source, string command)
    {
        // each process that requires admin access (ownership) must be verified with this function
        string possible = CommandOnDevice(device, source, command);
        if (!IsPossible(possible))
        {
            return possible;
        }
        if (device.player != source.Player_operator)
        {
            // player does not have admin access (does not own this device)
            return "-not admin";
        }
        return "+ok";
    }
    public static string CommandOnEnemy(DeviceScript device, OperatorScript source, string command, bool scan_required = true)
    {
        // process that are ran on foreign devices are verified with this function
        string possible = CommandOnDevice(device, source, command);
        if (!IsPossible(possible))
        {
            return possible;
        }
        if (device.IsFriendlyWith(source.Player_operator))
        {
            // device is friendly, target enemy's device
            return "-friendly";
        }
        if (!device.CanPlayerSee(source.Player_operator, true) && scan_required)
        {
            // device must be first scanned
            return "-unscanned";
        }
        return "+ok";
    }
    public static string AttackPossible(DeviceScript device, OperatorScript source, string command)
    {
        // related processes: Cyberattack, Trojan process, Firewall breaker
        string possible = CommandOnEnemy(device, source, command, true);
        if (!IsPossible(possible))
        {
            return possible;
        }
        if (source.AlreadyAttackingOn(device.gameObject))
        {
            // each device can be attacked only by one type of attack in one moment
            return "-attacking";
        }
        if (DeviceManagment.BelongsToCategory(device.gameObject, "Router") && command != CommandsDefinitions.break_firewall)
        {
            // firewall breaker's code level is checked in BreakFirewall function
            if (device.GetComponent<RouterScript>().IsFirewallActive())
            {
                // device is protected by firewall
                return "-firewall";
            }
        }
        if (device.HasStasis())
        {
            // device has active stasis
            return "-stasis";
        }
        return "+ok";
    }
    public static string InstallSoftware(DeviceScript device, OperatorScript source, string software)
    {
        if (source.AlreadyInstallingSoftwareOn(device.gameObject))
        {
            return "-install active";
        }
        string possible = CommandWithAdmin(device, source, CommandsDefinitions.install_software);
        if (!IsPossible(possible))
        {
            return possible;
        }
        if (!DeviceManagment.BelongsToCategory(device.gameObject, "Computer"))
        {
            // must be a computer
            return "-not computer";
        }
        if (!device.IsSafe())
        {
            return "-danger";
        }
        if (device.GetComponent<ComputerScript>().is_active_operator)
        {
            return "-operator selected";
        }
        if (device.GetComponent<ComputerScript>().GetOwnTask().type != CommandsDefinitions.idle)
        {
            return "-has task";
        }
        if (!HasEnoughPCS(software, source))
        {
            // not enough PCS
            return "-not enough PCS";
        }
        return "+ok";
    }
    public static bool HasEnoughPCS(string process, OperatorScript source, DeviceScript device = null)
    {
        int cost = source.GetTrueCost(process, device);
        int wallet = PlayerManager.GetPCS(source.Player_operator);
        return cost <= wallet;
    }
    public static string Scan(DeviceScript device, OperatorScript source)
    {
        string possible = CommandOnEnemy(device, source, CommandsDefinitions.scan, false);
        if (!IsPossible(possible))
        {
            return possible;
        }
        if (device.CanPlayerSee(source.Player_operator, true))
        {
            // already scanned
            return "-already scanned";
        }
        if (device.GetTrueSecurity() >= source.scan_level)
        {
            // code level not met
            return "-security";
        }
        return "+ok";
    }
    public static string TCPCatch(DeviceScript device, OperatorScript source)
    {
        if (!DeviceManagment.BelongsToCategory(device.gameObject, "Router"))
        {
            // must be a router
            return "-not router";
        }
        string possible = CommandOnEnemy(device, source, CommandsDefinitions.tcpcatch);
        if (!IsPossible(possible))
        {
            return possible;
        }
        if (device.GetTrueSecurity() >= source.tcpcatch_level)
        {
            // code level not met
            return "-security";
        }
        return "+ok";
    }
    public static string Antivirus(DeviceScript device, OperatorScript source)
    {
        string possible = CommandWithAdmin(device, source, CommandsDefinitions.antivirus_scan);
        if (!IsPossible(possible))
        {
            return possible;
        }
        return "+ok";
    }
    public static string Cyberattack(DeviceScript device, OperatorScript source)
    {
        string possible = AttackPossible(device, source, CommandsDefinitions.cyberattack);
        if (!IsPossible(possible))
        {
            return possible;
        }
        if (device.GetTrueSecurity() >= source.cyberattack_level)
        {
            // code level not met
            return "-security";
        }
        return "+ok";
    }
    public static string Trojan(DeviceScript device, OperatorScript source)
    {
        string possible = AttackPossible(device, source, CommandsDefinitions.trojan);
        if (!IsPossible(possible))
        {
            return possible;
        }
        if (device.GetTrueSecurity() >= source.trojan_level)
        {
            // code level not met
            return "-security";
        }
        return "+ok";
    }
    public static string BreakFirewall(DeviceScript device, OperatorScript source)
    {
        if (!DeviceManagment.BelongsToCategory(device.gameObject, "Router"))
        {
            return "-not router";
        }
        string possible = AttackPossible(device, source, CommandsDefinitions.break_firewall);
        if (!IsPossible(possible))
        {
            return possible;
        }
        if (!device.GetComponent<RouterScript>().IsFirewallActive())
        {
            // firewall already broken
            return "-firewall broken";
        }
        if (device.GetComponent<RouterScript>().GetTrueFirewallLevel() >= source.break_firewall_level)
        {
            // code level not met
            return "-firewall level";
        }
        return "+ok";
    }
    public static string Spyworm(DeviceScript device, OperatorScript source)
    {
        string possible = CommandOnEnemy(device, source, CommandsDefinitions.break_firewall);
        if (!IsPossible(possible))
        {
            return possible;
        }
        if (device.HasSpyworm(source.Player_operator))
        {
            return "-spyworm active";
        }
        if (device.GetTrueSecurity() >= source.spyworm_level)
        {
            // code level not met
            return "-security";
        }
        if (!HasEnoughPCS(CommandsDefinitions.inject_spyworm, source))
        {
            // not enough PCS
            return "-not enough PCS";
        }
        return "+ok";
    }
    public static string DeleteLog(DeviceScript device, OperatorScript source)
    {
        string possible = CommandWithAdmin(device, source, CommandsDefinitions.delete_log);
        if (!IsPossible(possible))
        {
            return possible;
        }
        if (device.Attack_log == null)
        {
            // no log present
            return "-log empty";
        }
        return "+ok";
    }
    public static string Counterattack(DeviceScript device, OperatorScript source)
    {
        string possible = CommandWithAdmin(device, source, CommandsDefinitions.counterattack);
        if (!IsPossible(possible))
        {
            return possible;
        }
        if (!device.IsUnderAttack())
        {
            return "-no visible attack";
        }
        return "+ok";
    }
    public static string CreateStasis(DeviceScript device, OperatorScript source)
    {
        string possible = CommandWithAdmin(device, source, CommandsDefinitions.create_stasis);
        if (!IsPossible(possible))
        {
            return possible;
        }
        if (device.HasStasis())
        {
            return "-stasis already";
        }
        if (source.stasis_charges == 0)
        {
            return "-no stasis charges";
        }
        if (HasEnoughPCS(CommandsDefinitions.create_stasis, source))
        {
            return "-not enough PCS";
        }
        return "+ok";
    }
    public static string Repair(DeviceScript device, OperatorScript source)
    {
        string possible = CommandWithAdmin(device, source, CommandsDefinitions.repair);
        if (!IsPossible(possible))
        {
            return possible;
        }
        if (!device.IsSafe())
        {
            return "-danger";
        }
        bool firewall_full = true;
        if (DeviceManagment.BelongsToCategory(device.gameObject, "Router"))
        {
            RouterScript router = device.GetComponent<RouterScript>();
            if (router.GetTrueFirewallHealth() < router.GetTrueFirewallHealth(true))
            {
                firewall_full = false;
            }
        }
        if (device.GetTrueIntegrity() == device.GetTrueIntegrity(true) && firewall_full)
        {
            return "-full integrity";
        }
        return "+ok";
    }
    public static string CreateVPN(DeviceScript device, OperatorScript source)
    {
        string possible = InstallSoftware(device, source, CommandsDefinitions.create_vpn);
        if (!IsPossible(possible))
        {
            return possible;
        }
        if (source.vpn_connection != null)
        {
            return "-vpn already";
        }
        else if (source.IsTaskTypePresent(CommandsDefinitions.create_vpn))
        {
            return "-one task limit";
        }
        return "+ok";
    }
    public static string CreateSlave(DeviceScript device, OperatorScript source)
    {
        string possible = InstallSoftware(device, source, CommandsDefinitions.create_slave);
        if (!IsPossible(possible))
        {
            return possible;
        }
        if (source.ReachedSlaveCapacity())
        {
            return "-slaves max";
        }
        return "+ok";
    }
    public static string UpdateBIOS(DeviceScript device, OperatorScript source, string update)
    {
        string possible = CommandWithAdmin(device, source, CommandsDefinitions.update_bios);
        if (!IsPossible(possible))
        {
            return possible;
        }
        if (!device.IsSafe())
        {
            return "-danger";
        }
        if (source.AlreadyUpdatingBIOSon(device.gameObject))
        {
            return "-update active";
        }
        switch (update)
        {
            case Upgrades.DEFENSE:
                if (device.upgrades.defense >= source.defense_upgrade_max) return "-upgrade max";
                break;
            case Upgrades.SECURITY:
                if (device.upgrades.security >= source.security_upgrade_max) return "-upgrade max";
                break;
            case Upgrades.POWER:
                if (!(DeviceManagment.BelongsToCategory(device.gameObject, "Computer") ||
                    (!DeviceManagment.BelongsToCategory(device.gameObject, "Server")))) return "-not computer or server";
                if (device.upgrades.power >= source.power_upgrade_max) return "-upgrade max";
                break;
            case Upgrades.FIREWALL:
                if (!DeviceManagment.BelongsToCategory(device.gameObject, "Router")) return "-not router";
                if (device.upgrades.firewall >= source.firewall_upgrade_max) return "-upgrade max";
                break;
            default:
                Debug.LogError("CommandPossible: UpdateBIOS update error");
                break;
        }
        if (!HasEnoughPCS(update + Upgrades.UPGRADE_SUFFIX, source, device))
        {
            return "-not enough PCS";
        }
        return "+ok";
    }
    public static string DownloadData(DeviceScript device, OperatorScript source)
    {
        string possible = CommandWithAdmin(device, source, CommandsDefinitions.download_data);
        if (!IsPossible(possible))
        {
            return possible;
        }
        if (!device.HasDownloadableContent())
        {
            return "-no files";
        }
        return "+ok";
    }
    public static string Terminate(DeviceScript device, OperatorScript source)
    {
        string possible = CommandWithAdmin(device, source, CommandsDefinitions.terminate);
        if (!IsPossible(possible))
        {
            return possible;
        }
        if (device.gameObject == source.gameObject)
        {
            return "-self termination";
        }
        return "+ok";
    }
    public static string Uninstall(DeviceScript device, OperatorScript source)
    {
        string possible = CommandWithAdmin(device, source, CommandsDefinitions.install_software);
        if (!IsPossible(possible))
        {
            return possible;
        }
        if (!DeviceManagment.BelongsToCategory(device.gameObject, "Computer"))
        {
            // must be a computer
            return "-not computer";
        }
        if (device.GetComponent<ComputerScript>().is_active_operator)
        {
            return "-operator selected";
        }
        if (device.GetComponent<ComputerScript>().GetOwnTask().type == CommandsDefinitions.idle)
        {
            return "-idle";
        }
        return "+ok";
    }
    public static string ProcessOnDevice(DeviceScript device, OperatorScript source)
    {
        // any process must be verified with this function
        if (!device.CanPlayerSee(source.Player_operator))
        {
            // player does not see the device (not visible)
            return "-not visible";
        }
        if (device.IsTerminated())
        {
            // device is terminated and no actions can be performed on terminated devices
            return "-terminated";
        }
        return OK;
    }
    public static string ProcessWithAdmin(DeviceScript device, OperatorScript source, bool safe_mode_required)
    {
        // each process that requires admin access (ownership) must be verified with this function
        string possible = ProcessOnDevice(device, source);
        if (!IsPossible(possible))
        {
            return possible;
        }
        if (device.player != source.Player_operator)
        {
            // player does not have admin access (does not own this device)
            return "-not admin";
        }
        if (safe_mode_required && !device.IsSafe())
        {
            return "-danger";
        }
        return "+ok";
    }
    public static string ProcessOnEnemy(DeviceScript device, OperatorScript source)
    {
        // process that are ran on foreign devices are verified with this function
        string possible = ProcessOnDevice(device, source);
        if (!IsPossible(possible))
        {
            return possible;
        }
        if (device.IsFriendlyWith(source.Player_operator))
        {
            // device is friendly, target enemy's device
            return "-friendly";
        }
        return "+ok";
    }
    public static string ProcessAttack(DeviceScript device, OperatorScript source, string process)
    {
        // related processes: Cyberattack, Trojan process, Firewall breaker
        string possible = ProcessOnEnemy(device, source);
        if (!IsPossible(possible))
        {
            return possible;
        }
        if (device.HasStasis())
        {
            // device has active stasis
            return "-stasis";
        }
        switch (process)
        {
            case CommandsDefinitions.cyberattack:
                if (device.GetTrueSecurity() >= source.cyberattack_level) return "-security";
                break;
            case CommandsDefinitions.trojan:
                if (device.GetTrueSecurity() >= source.trojan_level) return "-security";
                break;
            case CommandsDefinitions.break_firewall:
                if (device.GetTrueSecurity() >= source.break_firewall_level) return "-security";
                break;
        }
        return OK;
    }
    public static string ProcessScan(DeviceScript device, OperatorScript source)
    {
        string possible = ProcessOnEnemy(device, source);
        if (!IsPossible(possible))
        {
            return possible;
        }
        if (device.GetTrueSecurity() >= source.scan_level)
        {
            // code level not met
            return "-security";
        }
        return "+ok";
    }
    public static string ProcessTCPCatch(DeviceScript device, OperatorScript source)
    {
        string possible = ProcessOnEnemy(device, source);
        if (!IsPossible(possible))
        {
            return possible;
        }
        if (device.GetTrueSecurity() >= source.tcpcatch_level)
        {
            // code level not met
            return "-security";
        }
        return "+ok";
    }
    public static string ProcessSpyworm(DeviceScript device, OperatorScript source)
    {
        string possible = ProcessOnEnemy(device, source);
        if (!IsPossible(possible))
        {
            return possible;
        }
        if (device.GetTrueSecurity() >= source.spyworm_level)
        {
            // code level not met
            return "-security";
        }
        return "+ok";
    }
    public static string ProcessCounterAttack(DeviceScript device, OperatorScript source)
    {
        string possible = ProcessWithAdmin(device, source, false);
        if (!IsPossible(possible))
        {
            return possible;
        }
        if (!device.IsUnderAttack())
        {
            return "-no visible attack";
        }
        return "+ok";
    }
}
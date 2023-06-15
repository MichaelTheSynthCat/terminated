using INT = System.Int32;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// OperatorScript must be attached to every operator.
//  It controls all basic processes occuring in the operator.
//  It contains all bonuses and technology levels.
public class OperatorScript : MonoBehaviour
{
    public int Player_operator { get; private set; } // original player from the start of the game
    private bool defeated = false; // if was the operator defeated / overtaken by another player
    public bool dummy = false; // if the operator's only purpose is storing bonuses and technology levels (mostly accessed by public)
    public int task_capacity = 1; // number of tasks that can be processed in one moment
    public int slaves_capacity = 0; // how many slaves can an operator have in one moment
    public List<GameObject> slaves; // container for active slaves
    public GameObject vpn_connection = null; // stores device providing VPN connection

    // work amounts
    public Dictionary<string, float> work_amounts = new Dictionary<string, float>()
    {
        {CommandsDefinitions.scan, 300.0f },
        {CommandsDefinitions.tcpcatch, 500.0f },
        {CommandsDefinitions.antivirus_scan, 600.0f },
        {CommandsDefinitions.delete_log, 400.0f},
        {CommandsDefinitions.create_stasis, 1000.0f },
        {CommandsDefinitions.create_vpn, 1000.0f },
        {CommandsDefinitions.create_slave, 1000.0f },
        {CommandsDefinitions.install_software, 1000.0f },
        {Resources.PCS, 300.0f},
        {Resources.CATCOIN, 500.0f },
        {CommandsDefinitions.update_bios, 1000.0f },
        {CommandsDefinitions.extra_update_bios, 500.0f },
        {CommandsDefinitions.download_data, 1000.0f },
        {CommandsDefinitions.terminate, 1.0f },
    };

    // code levels + penalties
    public int scan_level = 10;
    public int antivirus_level = 8;
    public int cyberattack_level = 5;
    public int trojan_level = 4;
    public int break_firewall_level = 6;
    public int spyworm_level = 3;
    public float trojan_penalty = 0.5f;
    public int tcpcatch_level = 7;
    public int botnet_level = 4;
    public float termination_coefficient = 1.0f;

    // stasis
    public float stasis_duration = 10.0f;
    public float stasis_cooldown_charge = 30.0f;
    public int stasis_charges = 1;
    public int stasis_charges_max = 1;
    public float Stasis_recharge_timer { get; private set; }

    // repair
    public float repair_integrity_coefficient = 50.0f;
    public float repair_firewall_coefficient = 100.0f;

    // installed software related
    public Dictionary<string, int> upgrade_boost = new Dictionary<string, int>()
    {
        {Upgrades.DEFENSE, 10 },
        {Upgrades.SECURITY, 1 },
        {Upgrades.POWER, 10 },
        {Upgrades.FIREWALL, 1 },
    };
    public int defense_upgrade_max = 3;
    public int security_upgrade_max = 3;
    public int power_upgrade_max = 3;
    public int firewall_upgrade_max = 3;

    // pcs costs
    public Dictionary<string, int> pcs_cost = new Dictionary<string, int>()
    {
        {CommandsDefinitions.inject_spyworm, 5 },
        {CommandsDefinitions.create_stasis, 5 },
        {CommandsDefinitions.create_vpn, 10 },
        {CommandsDefinitions.create_slave, 10 },
        {CommandsDefinitions.extra_slave, 5 },
        {CommandsDefinitions.generate_pcs, 0 },
        {CommandsDefinitions.mine_catcoin, 5 },
        {CommandsDefinitions.autoantivirus, 5 },
        {CommandsDefinitions.autorepair, 5 },
        {CommandsDefinitions.botnet, 10 },

        {Upgrades.DEFENSE+Upgrades.UPGRADE_SUFFIX, 10 },
        {Upgrades.SECURITY+Upgrades.UPGRADE_SUFFIX, 10 },
        {Upgrades.POWER+Upgrades.UPGRADE_SUFFIX, 10 },
        {Upgrades.FIREWALL+Upgrades.UPGRADE_SUFFIX, 10 },

        {"defense_upgrade1", 10 },
        {"defense_upgrade2", 20 },
        {"defense_upgrade3", 40 },
        {"security_upgrade1", 10 },
        {"security_upgrade2", 20 },
        {"security_upgrade3", 40 },
        {"power_upgrade1", 10 },
        {"power_upgrade2", 20 },
        {"power_upgrade3", 40 },
        {"firewall_upgrade1", 10 },
        {"firewall_upgrade2", 20 },
        {"firewall_upgrade3", 40 },
    };
    
    // technologies and torrents
    public Technology Technology { get; private set; }
    public List<string> Installed_torrents { get; private set; }
    public List<string> Installed_upgrades { get; private set; }
    public List<string> editor_bonuses;

    // private code things
    private List<Task> tasks = new List<Task>();

    // other important scripts
    public DeviceScript This_device { get; private set; }
    public ComputerScript This_computer { get; private set; }
    public static List<string> attacking = new List<string>() 
    { 
        CommandsDefinitions.cyberattack,
        CommandsDefinitions.trojan, 
        CommandsDefinitions.break_firewall
    };
    public static List<string> software_installing = new List<string>() { 
        CommandsDefinitions.create_vpn,
        CommandsDefinitions.create_slave,
        CommandsDefinitions.create_stasis,
        CommandsDefinitions.install_software};
    public static List<string> updating_bios = new List<string>()
    {
        "update_bios",
        "defense_upgrade1", "defense_upgrade2", "defense_upgrade3",
        "security_upgrade1", "security_upgrade2", "security_upgrade3",
        "power_upgrade1", "power_upgrade2", "power_upgrade3",
        "firewall_upgrade1", "firewall_upgrade2", "firewall_upgrade3",
    };
    public const string START_OK = "+Starting process";
    public const string REACHED_TASK_LIMIT = "-Reached task limit";

    private void Awake()
    {
        Technology = new Technology(this);
        Installed_torrents = new List<string>();
        Installed_upgrades = new List<string>();
    }

    void Start()
    {
        This_device = GetComponent<DeviceScript>();
        Player_operator = This_device.player;
        This_computer = GetComponent<ComputerScript>();

        for(int i=0; i < task_capacity; i++)
        {
            tasks.Add(new Task() { id = i, type = "idle", target = null, progress = 1.0f,
                animated_rectangle = null });
        }

        Stasis_recharge_timer = stasis_cooldown_charge;

        
        foreach(string line in editor_bonuses)
        {
            string[] separated = line.Split();
            Technology.ApplySmartUpgrade(separated[0] + " " + separated[1],
                float.Parse(separated[2], CultureInfo.InvariantCulture));
        }
    }

    // Update is called once per frame
    void Update()
    {
        // do tasks
        foreach(Task task in tasks)
        {
            switch (task.type)
            {
                case CommandsDefinitions.idle:
                    if(Player_operator == 1)
                    {
                        TaskBarController.UpdateTaskBar(task.id, "Idle", 0.0f, 1.0f);
                    }
                    break;
                case CommandsDefinitions.scan:
                    DoScan(task);
                    break;
                case CommandsDefinitions.tcpcatch:
                    DoTCPCatch(task);
                    break;
                case CommandsDefinitions.antivirus_scan:
                    DoAntivirusScan(task);
                    break;
                case CommandsDefinitions.cyberattack:
                    DoCyberAttack(task);
                    break;
                case CommandsDefinitions.trojan:
                    DoTrojanProcess(task);
                    break;
                case CommandsDefinitions.break_firewall:
                    DoBreakFirewall(task);
                    break;
                case CommandsDefinitions.inject_spyworm:
                    DoInjectSpyworm(task);
                    break;
                case CommandsDefinitions.delete_log:
                    DoDeleteLogProcess(task);
                    break;
                case CommandsDefinitions.counterattack:
                    DoCounterattack(task);
                    break;
                case CommandsDefinitions.create_stasis:
                    DoCreateStasis(task);
                    break;
                case CommandsDefinitions.repair:
                    DoRepair(task);
                    break;
                case CommandsDefinitions.create_vpn:
                    DoCreateVPN(task);
                    break;
                case CommandsDefinitions.create_slave:
                    DoCreateSlave(task);
                    break;
                case CommandsDefinitions.install_software:
                    DoInstallSoftware(task);
                    break;
                case CommandsDefinitions.update_bios:
                    DoUpdateBIOS(task);
                    break;
                case CommandsDefinitions.download_data:
                    DoDownloadData(task);
                    break;
                case CommandsDefinitions.terminate:
                    DoTerminateDevice(task);
                    break;
                default:
                    Debug.LogError("OperatorScript: Task type error - " + task.type);
                    break;
            }
        }

        // stasis charges recharge
        if(stasis_charges_max > stasis_charges)
        {
            Stasis_recharge_timer -= Time.deltaTime;
            if (Stasis_recharge_timer <= 0.0f)
            {
                stasis_charges += 1;
                Stasis_recharge_timer = stasis_cooldown_charge;
            }
        }
    }

    public bool CreateNewTask(string type, GameObject target, string software="") // create new task
    {
        if (EmptyTaskSpace() && !defeated)
        {
            foreach(Task task in tasks)
            {
                if(task.type == CommandsDefinitions.idle)
                {
                    GameObject anim_rec = CreateAnimatedRectangle(target.transform.position); // creates rectangle showing progress
                    tasks[task.id] = new Task() { id = task.id, type = type, target = target, progress = 0.0f,
                    animated_rectangle = anim_rec, software = software};
                    if(Player_operator != 1) // animated rectangle visible only when created from user's operator
                    {
                        anim_rec.GetComponent<Image>().enabled = false;
                    }
                    return true;
                }
            }
            return false;
        }
        else
        {
            return false;
        }
    }
    public bool EmptyTaskSpace() // ask if there's an empty process space for another task
    {
        int idle_tasks = 0;
        foreach(Task task in tasks)
        {
            if(task.type == CommandsDefinitions.idle)
            {
                idle_tasks += 1;
            }
        }
        return idle_tasks > 0;

    }
    public void USERCancelTask(int task_id) // cancel a task by user
        // invoked by TaskBar buttons
    {
        MakeIdle(tasks[task_id]);
    }
    public bool IsTaskTypePresent(string task_type) // is the given type of the task present and in execution
        // example: task "create_vpn" can't be present two and more times
    {
        foreach(Task task in tasks)
        {
            if(task.type == task_type)
            {
                return true;
            }
        }
        return false;
    }
    public bool DuplicateTaskExists(string command, DeviceScript target_device) // is the task already being executed on the selected device
    {
        //print("Check duplicates");
        foreach(AttachedTask attachedTask in target_device.attachedTasks)
        {
            //print("This device? " + attachedTask.init_device + " == " + GetInternetDevice());
            if(attachedTask.init_device == GetInternetDevice())
            {
                //print("yes and the task? " + attachedTask.task_name + " == " + command);
                if(attachedTask.task_name == command)
                {
                    //print("don't do it");
                    return true;
                }
            }
        }
        return false;
    }
    public bool AlreadyInstallingSoftwareOn(GameObject target) // is operator already installing something on a selected device
    {
        foreach(Task task in tasks)
        {
            if (task.target == target && software_installing.Contains(task.type))
            {
                return true;
            }
        }
        return false;
    }
    public bool AlreadyUpdatingBIOSon(GameObject target) // is operator already updating BIOS on a selectde device
    {
        foreach (Task task in tasks)
        {
            if (task.target == target && updating_bios.Contains(task.type))
            {
                return true;
            }
        }
        return false;
    }
    public bool AlreadyAttackingOn(GameObject target) // is operator already a selected device
    {
        foreach (Task task in tasks)
        {
            if (task.target == target && attacking.Contains(task.type))
            {
                return true;
            }
        }
        return false;
    }
    public static GameObject CreateAnimation(GameObject prefab, Vector3 position) // create effect animation
    {
        return Instantiate(prefab, position, Quaternion.identity);
    }
    public static GameObject CreateAnimatedRectangle(Vector3 position) // create animated rectangle, visible only to player 1
    {
        GameObject rectangle = Instantiate(EffectsStash.stash.task_rectangle, WorldCanvasController.canvas.transform);
        rectangle.transform.position = position;
        rectangle.GetComponent<Image>().color = PlayerManager.GetPlayerColor(1);
        rectangle.GetComponent<Image>().fillAmount = 0.0f;
        return rectangle;
    }
    public float GetSuperPower() // get true power of this operator
    {
        float power = This_computer.GetTruePower();
        foreach(GameObject slave in slaves)
        {
            power += slave.GetComponent<ComputerScript>().GetTruePower();
        }
        return power;
    }
    public bool WasDefeated() // return true if the operator was defeated
    {
        return defeated;
    }
    public void MakeIdle(Task task) // destroys task, all animations, detaches the attached task from target and updates UI
    {
        if(task.type == CommandsDefinitions.idle) // if task is already idle, do nothing
        {
            return;
        }
        task.target.GetComponent<DeviceScript>().DetachTask(GetInternetDevice(), task.type); // detach task to stop basic animation
        task.type = CommandsDefinitions.idle;
        task.target = null;
        task.progress = 1.0f;
        Destroy(task.animated_rectangle);
        task.animated_rectangle = null;
        task.software = "";
        if (Player_operator == 1)
        {
            TaskBarController.UpdateTaskBar(task.id, "Idle", 0.0f, 1.0f);
        }
    }
    public string Scan(GameObject target)
    {
        // Scan a device (target)
        DeviceScript device = target.GetComponent<DeviceScript>();

        string result = CommandPossible.Scan(device, this);

        if (!CommandPossible.IsPossible(result))
        {
            return CommandPossible.GetUserStartErrorMessage(result);
        }
        else
        {
            if (CreateNewTask(CommandsDefinitions.scan, target))
            {
                target.GetComponent<DeviceScript>().AttachTask(CommandsDefinitions.scan, GetInternetDevice(), true,
                    CreateAnimation(EffectsStash.stash.scanning, target.transform.position));
                return START_OK;
            }
            else
            {
                return REACHED_TASK_LIMIT;
            }
        }
    }

    private void DoScan(Task task)
    {
        DeviceScript target_device = task.target.GetComponent<DeviceScript>();

        // possible errors in real-time
        string check_detail = CommandPossible.ProcessScan(target_device, this);
        if (!CommandPossible.IsPossible(check_detail))
        {
            if (Player_operator == 1)
            {
                ConsoleController.ShowUserProcessError("Scan", target_device, check_detail);
            }
            MakeIdle(task);
            return;
        }

        float speed = GetSuperPower() / (GetTrueWork(CommandsDefinitions.scan) * GetSlavePenalty())
            * Technology.SmartGetPluginBonus(CommandsDefinitions.scan, target_device.type, true);
        task.progress += (speed * Time.deltaTime);
        if(task.progress >= 1.0f) // scan completed
        {
            target_device.ChangeScanned(Player_operator, true);
            if (Player_operator == 1)
            {
                ConsoleController.ShowText("Completed - Scan {" + target_device.GetIP() + "}", "normal");
                SFXPlayer.PlaySFX(SFXPlayer.OK);
            }
            MakeIdle(task);
        }
        else
        {
            task.animated_rectangle.GetComponent<Image>().fillAmount = task.progress;
            if(Player_operator == 1) 
            {
                TaskBarController.UpdateTaskBar(task.id, "Scan " + target_device.GetIP(),
                    (1.0f - task.progress) / speed, task.progress);
            }
        }
    }

    public string TCPCatch(GameObject target)
    {
        DeviceScript device = target.GetComponent<DeviceScript>();
        string result = CommandPossible.TCPCatch(device, this);
        if (!CommandPossible.IsPossible(result))
        {
            return CommandPossible.GetUserStartErrorMessage(result);
        }
        else
        {
            if (CreateNewTask(CommandsDefinitions.tcpcatch, target))
            {
                target.GetComponent<DeviceScript>().AttachTask(CommandsDefinitions.tcpcatch, GetInternetDevice(), true,
                    CreateAnimation(EffectsStash.stash.tcpcatching, target.transform.position));
                return START_OK;
            }
            else
            {
                return REACHED_TASK_LIMIT;
            }
        }
    }

    private void DoTCPCatch(Task task)
    {
        DeviceScript target_device = task.target.GetComponent<DeviceScript>();
        RouterScript target_router = task.target.GetComponent<RouterScript>();

        // possible errors in real-time
        string check_detail = CommandPossible.ProcessTCPCatch(target_device, this);
        if (!CommandPossible.IsPossible(check_detail))
        {
            if (Player_operator == 1)
            {
                ConsoleController.ShowUserProcessError("TCP Catch", target_device, check_detail);
            }
            MakeIdle(task);
            return;
        }

        // TO DO: add firewall level bonus
        float speed = GetSuperPower() / (GetTrueWork(CommandsDefinitions.tcpcatch) * GetSlavePenalty())
            * Technology.SmartGetPluginBonus(CommandsDefinitions.tcpcatch, target_device.type, true);
        task.progress += (speed * Time.deltaTime);
        // TO DO: instant reveal when firewall is broken
        if (task.progress >= 1.0f)
        {
            // reveal one random device
            List<GameObject> hidden_devices = new List<GameObject>();
            foreach(GameObject device in target_router.local_connections)
            {
                if (!device.GetComponent<DeviceScript>().CanPlayerSee(Player_operator))
                {
                    if(DeviceManagment.BelongsToCategory(device, "Computer")) // don't reveal operators with VPN
                    {
                        if (device.GetComponent<ComputerScript>().is_active_operator)
                        {
                            if (device.GetComponent<OperatorScript>().vpn_connection != null)
                            {
                                continue;
                            }
                        }
                    }
                    hidden_devices.Add(device);
                }
            }
            if(hidden_devices.Count == 0) // no device to reveal
            {
                if (Player_operator == 1)
                {
                    ConsoleController.ShowText("TCPCatch found no other devices connected to {"
                        + target_device.GetIP() + "}", "warning");
                    SFXPlayer.PlaySFX(SFXPlayer.WARNING);
                }
            }
            else // reveal a random device
            {
                DeviceScript found_device = hidden_devices[Random.Range(0, hidden_devices.Count)].GetComponent<DeviceScript>();
                found_device.ChangeVisibility(Player_operator, true);
                if (Player_operator == 1)
                {
                    ConsoleController.ShowText("TCPCatch found " + found_device.GetIP(), "normal");
                    SFXPlayer.PlaySFX(SFXPlayer.SUCCESS);
                }
                target_router.ReloadRenderer();
            }
            MakeIdle(task);
        }
        else
        {
            task.animated_rectangle.GetComponent<Image>().fillAmount = task.progress;
            if (Player_operator == 1)
            {
                TaskBarController.UpdateTaskBar(task.id, "TCPCatch-ing " + target_device.GetIP(),
                    (1.0f - task.progress) / speed, task.progress);
            }
        }
    }
    public string AntivirusScan(GameObject target)
    {
        // Scan a device (target)
        DeviceScript device = target.GetComponent<DeviceScript>();
        string result = CommandPossible.Antivirus(device, this);
        if (!CommandPossible.IsPossible(result))
        {
            return CommandPossible.GetUserStartErrorMessage(result);
        }
        else
        {
            if (CreateNewTask(CommandsDefinitions.antivirus_scan, target))
            {
                target.GetComponent<DeviceScript>().AttachTask(CommandsDefinitions.antivirus_scan, GetInternetDevice(), true,
                    CreateAnimation(EffectsStash.stash.antivirus_scan, target.transform.position));
                return START_OK;
            }
            else
            {
                return REACHED_TASK_LIMIT;
            }
        }
    }
    private void DoAntivirusScan(Task task)
    {
        DeviceScript target_device = task.target.GetComponent<DeviceScript>();

        // possible errors in real-time
        string check_detail = CommandPossible.ProcessWithAdmin(target_device, this, false);
        if (!CommandPossible.IsPossible(check_detail))
        {
            if (Player_operator == 1)
            {
                ConsoleController.ShowUserProcessError("Antivirus.exe", target_device, check_detail);
            }
            MakeIdle(task);
            return;
        }

        float speed = GetSuperPower() / (GetTrueWork(CommandsDefinitions.antivirus_scan) * GetSlavePenalty())
            * Technology.SmartGetPluginBonus(CommandsDefinitions.antivirus_scan, target_device, true);
        task.progress += (speed * Time.deltaTime);
        if (task.progress >= 1.0f)
        {
            string removed_malware = target_device.RemoveMalware(antivirus_level);
            if (Player_operator == 1)
            {
                switch (removed_malware)
                {
                    case "spyworm":
                        ConsoleController.ShowText("Antivirus {" + target_device.GetIP() + "} removed spyware", "success");
                        SFXPlayer.PlaySFX(SFXPlayer.SUCCESS);
                        break;
                    case "spyworm+trojan":
                        ConsoleController.ShowText("Antivirus {" + target_device.GetIP() + "} found hidden threat and removed spyware", "warning");
                        break;
                    case "trojan":
                        ConsoleController.ShowText("Antivirus {" + target_device.GetIP() + "} found hidden threat", "warning");
                        break;
                    default:
                        ConsoleController.ShowText("Antivirus {" + target_device.GetIP() + "} found 0 threats", "normal");
                        break;
                }
            }
            MakeIdle(task);
        }
        else
        {
            task.animated_rectangle.GetComponent<Image>().fillAmount = task.progress;
            if (Player_operator == 1)
            {
                TaskBarController.UpdateTaskBar(task.id, "Antivirus " + target_device.GetIP(), 
                    (1.0f - task.progress) / speed, task.progress);
            }
        }
    }

    public string CyberAttack(GameObject target)
    {
        DeviceScript device = target.GetComponent<DeviceScript>();
        string result = CommandPossible.Cyberattack(device, this);

        if (!CommandPossible.IsPossible(result))
        {
            return CommandPossible.GetUserStartErrorMessage(result);
        }
        else
        {
            if (CreateNewTask(CommandsDefinitions.cyberattack, target))
            {
                target.GetComponent<DeviceScript>().AttachTask(CommandsDefinitions.cyberattack, GetInternetDevice(), false,
                    CreateAnimation(EffectsStash.stash.cyberattacking, target.transform.position));
                return START_OK;
            }
            else
            {
                return REACHED_TASK_LIMIT;
            }
        }
    }
    private void DoCyberAttack(Task task)
    {
        DeviceScript target_device = task.target.GetComponent<DeviceScript>();

        // possible errors in real-time
        string check_detail = CommandPossible.ProcessAttack(target_device, this, CommandsDefinitions.cyberattack);
        if (!CommandPossible.IsPossible(check_detail))
        {
            if (Player_operator == 1)
            {
                ConsoleController.ShowUserProcessError("Cyberattack", target_device, check_detail);
            }
            MakeIdle(task);
            return;
        }

        float speed = GetSuperPower() / target_device.GetTrueDefense()
            * Technology.SmartGetPluginBonus(CommandsDefinitions.cyberattack, target_device, true);
        target_device.ChangeIntegrity(-speed * Time.deltaTime);
        if (target_device.GetTrueIntegrity() <= 0.0f)
        {
            if(Player_operator == 1)
            {
                ConsoleController.ShowText("Cyberattack {" + target_device.GetIP() + "} successful", "success");
                SFXPlayer.PlaySFX(SFXPlayer.SUCCESS);
            }
            target_device.ChangeControl(Player_operator, true, GetInternetDevice());

            MakeIdle(task);
        }
        else
        {
            task.animated_rectangle.GetComponent<Image>().fillAmount = 
                1.0f - target_device.GetTrueIntegrity()/target_device.GetTrueIntegrity(true);
            if(Player_operator == 1)
            {
                TaskBarController.UpdateTaskBar(task.id, "Cyberattacking " + target_device.GetIP(),
                    target_device.GetTrueIntegrity() / speed, 
                    1.0f - target_device.GetTrueIntegrity() / target_device.GetTrueIntegrity(true));
            }
        }
    }
    public string TrojanProcess(GameObject target)
    {
        DeviceScript device = target.GetComponent<DeviceScript>();
        string result = CommandPossible.Trojan(device, this);

        if (!CommandPossible.IsPossible(result))
        {
            return CommandPossible.GetUserStartErrorMessage(result);
        }
        else
        {
            if (CreateNewTask(CommandsDefinitions.trojan, target))
            {
                target.GetComponent<DeviceScript>().AttachTask(CommandsDefinitions.trojan, GetInternetDevice(), true,
                    CreateAnimation(EffectsStash.stash.trojanprocess, target.transform.position));
                return START_OK;
            }
            else
            {
                return REACHED_TASK_LIMIT;
            }
        }
    }
    private void DoTrojanProcess(Task task)
    {
        DeviceScript target_device = task.target.GetComponent<DeviceScript>();

        // possible errors in real-time
        string check_detail = CommandPossible.ProcessAttack(target_device, this, CommandsDefinitions.trojan);
        if (!CommandPossible.IsPossible(check_detail))
        {
            if (Player_operator == 1)
            {
                ConsoleController.ShowUserProcessError("Trojan", target_device, check_detail);
            }
            MakeIdle(task);
            return;
        }

        float speed = GetSuperPower() * 0.5f / target_device.GetTrueDefense()
            * Technology.SmartGetPluginBonus(CommandsDefinitions.trojan, target_device, true);
        target_device.ChangeIntegrity(-speed * Time.deltaTime);
        if (target_device.GetTrueIntegrity() <= 0.0f)
        {
            if (Player_operator == 1)
            {
                ConsoleController.ShowText("Cyberattack {" + target_device.GetIP() + "} successful", "success");
                SFXPlayer.PlaySFX(SFXPlayer.SUCCESS);
            }
            target_device.ChangeControl(Player_operator, false, GetInternetDevice());

            MakeIdle(task);
        }
        else
        {
            task.animated_rectangle.GetComponent<Image>().fillAmount =
                1.0f - target_device.GetTrueIntegrity() / target_device.GetTrueIntegrity(true);
            if (Player_operator == 1)
            {
                TaskBarController.UpdateTaskBar(task.id, "Trojan.exe " + target_device.GetIP(),
                    (target_device.GetTrueIntegrity() / speed), 1.0f);
            }
        }
    }
    public string BreakFirewall(GameObject target)
    {
        DeviceScript device = target.GetComponent<DeviceScript>();
        string result = CommandPossible.BreakFirewall(device, this);

        if (!CommandPossible.IsPossible(result))
        {
            return CommandPossible.GetUserStartErrorMessage(result);
        }
        else
        {
            if (CreateNewTask(CommandsDefinitions.break_firewall, target))
            {
                target.GetComponent<DeviceScript>().AttachTask(CommandsDefinitions.break_firewall, GetInternetDevice(), false,
                    CreateAnimation(EffectsStash.stash.breaking_firewall, target.transform.position));
                return START_OK;
            }
            else
            {
                return REACHED_TASK_LIMIT;
            }
        }
    }
    private void DoBreakFirewall(Task task)
    {
        DeviceScript target_device = task.target.GetComponent<DeviceScript>();
        RouterScript target_router = task.target.GetComponent<RouterScript>();

        // possible errors in real-time
        string check_detail = CommandPossible.ProcessAttack(target_device, this, CommandsDefinitions.break_firewall);
        if (!CommandPossible.IsPossible(check_detail))
        {
            if (Player_operator == 1)
            {
                ConsoleController.ShowUserProcessError("Firewall Breaker", target_device, check_detail);
            }
            MakeIdle(task);
            return;
        }

        float speed = GetSuperPower() /
            (target_device.GetTrueDefense() + 
            (target_device.GetTrueDefense() * target_router.GetTrueFirewallLevel() * 0.1f))
            * Technology.SmartGetPluginBonus(CommandsDefinitions.break_firewall, target_device, true);
        target_router.ChangeFirewallHealth(-speed * Time.deltaTime);
        if (target_router.GetTrueFirewallHealth() <= 0.0f)
        {
            if (Player_operator == 1)
            {
                ConsoleController.ShowText("Broke Firewall on " + target_device.GetIP(), "success");
                SFXPlayer.PlaySFX(SFXPlayer.SUCCESS);
            }

            MakeIdle(task);
        }
        else
        {
            task.animated_rectangle.GetComponent<Image>().fillAmount =
                1.0f - target_router.GetTrueFirewallHealth() / target_router.GetTrueFirewallHealth(true);
            if (Player_operator == 1)
            {
                TaskBarController.UpdateTaskBar(task.id, "Break Firewall " + target_device.GetIP(),
                    (target_router.GetTrueFirewallHealth() / speed), 1.0f);
            }
        }
    }

    public string InjectSpyworm(GameObject target)
    {
        DeviceScript device = target.GetComponent<DeviceScript>();
        string result = CommandPossible.Spyworm(device, this);

        if (!CommandPossible.IsPossible(result))
        {
            return CommandPossible.GetUserStartErrorMessage(result);
        }
        else
        {
            if (CreateNewTask(CommandsDefinitions.inject_spyworm, target))
            {
                PlayerManager.RemovePCS(Player_operator, GetTrueCost(CommandsDefinitions.inject_spyworm));
                target.GetComponent<DeviceScript>().AttachTask(CommandsDefinitions.inject_spyworm, GetInternetDevice(), true,
                    CreateAnimation(EffectsStash.stash.spyworm_install, target.transform.position));
                return START_OK;
            }
            else
            {
                return REACHED_TASK_LIMIT;
            }
        }
    }
    private void DoInjectSpyworm(Task task)
    {
        DeviceScript target_device = task.target.GetComponent<DeviceScript>();

        // possible errors in real-time
        string check_detail = CommandPossible.ProcessSpyworm(target_device, this);
        if (!CommandPossible.IsPossible(check_detail))
        {
            if (Player_operator == 1)
            {
                ConsoleController.ShowUserProcessError("Inject spyworm", target_device, check_detail);
            }
            MakeIdle(task);
            return;
        }

        float speed = GetSuperPower() * 0.05f / target_device.GetTrueDefense()
            * Technology.SmartGetPluginBonus(CommandsDefinitions.inject_spyworm, target_device, true);
        task.progress += (speed * Time.deltaTime);
        if (task.progress >= 1.0f)
        {
            target_device.InstallSpyworm(Player_operator, true);
            if (Player_operator == 1)
            {
                ConsoleController.ShowText("Spyworm injected in " + target_device.GetIP(), "success");
                SFXPlayer.PlaySFX(SFXPlayer.SUCCESS);
            }
            MakeIdle(task);
        }
        else
        {
            task.animated_rectangle.GetComponent<Image>().fillAmount = task.progress;
            if (Player_operator == 1)
            {
                TaskBarController.UpdateTaskBar(task.id, "Inject spyworm " + target_device.GetIP(),
                    (1.0f - task.progress) / speed, task.progress);
            }
        }
    }
    public string DeleteLog(GameObject target)
    {
        DeviceScript device = target.GetComponent<DeviceScript>();
        string result = CommandPossible.DeleteLog(device, this);
        if (!CommandPossible.IsPossible(result))
        {
            return CommandPossible.GetUserStartErrorMessage(result);
        }
        else
        {
            if (CreateNewTask(CommandsDefinitions.delete_log, target))
            {
                target.GetComponent<DeviceScript>().AttachTask(CommandsDefinitions.delete_log, GetInternetDevice(), true,
                    CreateAnimation(EffectsStash.stash.deleting_log, target.transform.position));
                return START_OK;
            }
            else
            {
                return REACHED_TASK_LIMIT;
            }
        }
    }
    private void DoDeleteLogProcess(Task task)
    {
        DeviceScript target_device = task.target.GetComponent<DeviceScript>();

        // possible errors in real-time
        string check_detail = CommandPossible.ProcessWithAdmin(target_device, this, false);
        if (!CommandPossible.IsPossible(check_detail))
        {
            if (Player_operator == 1)
            {
                ConsoleController.ShowUserProcessError("Delete log", target_device, check_detail);
            }
            MakeIdle(task);
            return;
        }

        float speed = GetSuperPower() / (GetTrueWork(CommandsDefinitions.delete_log) * GetSlavePenalty())
            * Technology.SmartGetPluginBonus(CommandsDefinitions.delete_log, target_device, true);
        task.progress += (speed * Time.deltaTime);
        if (task.progress >= 1.0f)
        {
            target_device.DeleteLog();
            if (Player_operator == 1)
            {
                ConsoleController.ShowText("Log on " + target_device.GetIP() + " has been deleted.", "normal");
                SFXPlayer.PlaySFX(SFXPlayer.SUCCESS);
            }
            MakeIdle(task);
        }
        else
        {
            task.animated_rectangle.GetComponent<Image>().fillAmount = task.progress;
            if (Player_operator == 1)
            {
                TaskBarController.UpdateTaskBar(task.id, "Deleting log " + target_device.GetIP(),
                    (1.0f - task.progress) / speed, task.progress);
            }
        }
    }
    public string Counterattack(GameObject target)
    {
        DeviceScript device = target.GetComponent<DeviceScript>();
        string result = CommandPossible.Counterattack(device, this);
        if (!CommandPossible.IsPossible(result))
        {
            return CommandPossible.GetUserStartErrorMessage(result);
        }
        else
        {
            if (CreateNewTask(CommandsDefinitions.counterattack, target))
            {
                target.GetComponent<DeviceScript>().AttachTask(CommandsDefinitions.counterattack, GetInternetDevice(), true,
                    CreateAnimation(EffectsStash.stash.counterattacking, target.transform.position));
                return START_OK;
            }
            else
            {
                return REACHED_TASK_LIMIT;
            }
        }
    }
    private void DoCounterattack(Task task)
    {
        // TO DO: redesign counterattack mechanic 
        DeviceScript target_device = task.target.GetComponent<DeviceScript>();

        string check_detail = CommandPossible.ProcessCounterAttack(target_device, this);
        if (!CommandPossible.IsPossible(check_detail))
        {
            if (Player_operator == 1)
            {
                // TO DO: add successful defense if attack is stopped
                ConsoleController.ShowUserProcessError("Counterattack", target_device, check_detail);
            }
            MakeIdle(task);
            return;
        }

        DeviceScript counter_device = null;
        foreach(AttachedTask attachedTask in target_device.attachedTasks)
        {
            if(attachedTask.task_name == CommandsDefinitions.cyberattack)
            {
                counter_device = attachedTask.init_device.GetComponent<DeviceScript>();
                break;
            }
            // TO DO: rebuild counterattacking trojan
            else if(attachedTask.task_name == CommandsDefinitions.trojan && !attachedTask.stealth)
            {
                counter_device = attachedTask.init_device.GetComponent<DeviceScript>();
                break;
            }
            else if(attachedTask.task_name == CommandsDefinitions.break_firewall)
            {
                counter_device = attachedTask.init_device.GetComponent<DeviceScript>();
                break;
            }
            else if (attachedTask.task_name == CommandsDefinitions.botnet_attack)
            {
                counter_device = attachedTask.init_device.GetComponent<DeviceScript>();
                break;
            }
        }
        if(counter_device == null)
        {
            Debug.LogError("DoCounterattack - something went wrong");
        }
        float speed = GetSuperPower() / counter_device.GetTrueDefense()
            * Technology.SmartGetPluginBonus(CommandsDefinitions.counterattack, target_device, true);
        counter_device.ChangeIntegrity(-speed * Time.deltaTime);
        if (counter_device.GetTrueIntegrity() <= 0.0f)
        {
            if (Player_operator == 1)
            {
                ConsoleController.ShowText("Device " + counter_device.GetIP() + " is now under your control", "success");
                SFXPlayer.PlaySFX(SFXPlayer.SUCCESS);
            }
            counter_device.ChangeControl(Player_operator, true, GetInternetDevice());
            MakeIdle(task);
        }
        else
        {
            task.animated_rectangle.GetComponent<Image>().fillAmount = 0;
            if (Player_operator == 1)
            {
                TaskBarController.UpdateTaskBar(task.id, "Counterattacking from" + target_device.GetIP(),
                    0.0f , target_device.GetTrueIntegrity() / target_device.GetTrueIntegrity(true));
            }
        }
    }
    public string CreateStasis(GameObject target)
    {
        DeviceScript device = target.GetComponent<DeviceScript>();
        string result = CommandPossible.CreateStasis(device, this);
        if (!CommandPossible.IsPossible(result))
        {
            return CommandPossible.GetUserStartErrorMessage(result);
        }
        else
        {
            if (CreateNewTask(CommandsDefinitions.create_stasis, target))
            {
                PlayerManager.RemovePCS(Player_operator, GetTrueCost(CommandsDefinitions.create_stasis));
                target.GetComponent<DeviceScript>().AttachTask(CommandsDefinitions.create_stasis, GetInternetDevice(), false,
                    CreateAnimation(EffectsStash.stash.creating_stasis, target.transform.position));
                stasis_charges -= 1;
                return START_OK;
            }
            else
            {
                return REACHED_TASK_LIMIT;
            }
        }
    }
    private void DoCreateStasis(Task task)
    {
        DeviceScript target_device = task.target.GetComponent<DeviceScript>();

        // possible errors in real-time
        string check_detail = CommandPossible.ProcessWithAdmin(target_device, this, false);
        if (!CommandPossible.IsPossible(check_detail))
        {
            if (Player_operator == 1)
            {
                ConsoleController.ShowUserProcessError("Stasis creation", target_device, check_detail);
            }
            MakeIdle(task);
            return;
        }

        float speed = GetSuperPower() / GetTrueWork(CommandsDefinitions.create_stasis)
            * Technology.SmartGetPluginBonus(CommandsDefinitions.create_stasis, target_device, true);
        task.progress += (speed * Time.deltaTime);
        if (task.progress >= 1.0f)
        {
            target_device.ChangeScanned(Player_operator, true);
            if (Player_operator == 1)
            {
                ConsoleController.ShowText("Stasis created on " + target_device.GetIP(), "normal");
                SFXPlayer.PlaySFX(SFXPlayer.SUCCESS);
            }
            target_device.ActivateStasis(stasis_duration);
            MakeIdle(task);
        }
        else
        {
            task.animated_rectangle.GetComponent<Image>().fillAmount = task.progress;
            if (Player_operator == 1)
            {
                TaskBarController.UpdateTaskBar(task.id, "Stasis " + target_device.GetIP(),
                    (1.0f - task.progress) / speed, task.progress);
            }
        }
    }
    public string Repair(GameObject target)
    {
        // Scan a device (target)
        DeviceScript device = target.GetComponent<DeviceScript>();
        string result = CommandPossible.Repair(device, this);

        if (!CommandPossible.IsPossible(result))
        {
            return CommandPossible.GetUserStartErrorMessage(result);
        }
        else
        {
            if (CreateNewTask(CommandsDefinitions.repair, target))
            {
                target.GetComponent<DeviceScript>().AttachTask(CommandsDefinitions.repair, GetInternetDevice(), true,
                    CreateAnimation(EffectsStash.stash.repairing, target.transform.position));
                return START_OK;
            }
            else
            {
                return REACHED_TASK_LIMIT;
            }
        }
    }
    private void DoRepair(Task task)
    {
        DeviceScript target_device = task.target.GetComponent<DeviceScript>();
        RouterScript target_router = null;
        if(DeviceManagment.BelongsToCategory(task.target, "Router"))
        {
            target_router =  task.target.GetComponent<RouterScript>();
        }

        // possible errors in real-time
        string check_detail = CommandPossible.ProcessWithAdmin(target_device, this, true);
        if (!CommandPossible.IsPossible(check_detail))
        {
            if (Player_operator == 1)
            {
                ConsoleController.ShowUserProcessError("Repair", target_device, check_detail);
            }
            MakeIdle(task);
            return;
        }

        float speed = GetSuperPower() / repair_integrity_coefficient
            * Technology.SmartGetPluginBonus(CommandsDefinitions.repair, target_device, true);
        target_device.ChangeIntegrity(speed * Time.deltaTime);

        bool firewall_full = true;
        if(target_router != null)
        {
            if(target_router.GetTrueFirewallHealth() < target_router.GetTrueFirewallHealth(true))
            {
                float firewall_speed = GetSuperPower() / repair_firewall_coefficient
                    * Technology.SmartGetPluginBonus(CommandsDefinitions.repair, target_device, true);
                target_router.ChangeFirewallHealth(firewall_speed * Time.deltaTime);
                firewall_full = false;
            }
            task.progress = (target_device.GetTrueIntegrity() + target_router.GetTrueFirewallHealth()) /
                (target_device.GetTrueIntegrity(true)+target_router.GetTrueFirewallHealth(true));
        }
        else
        {
            task.progress = target_device.GetTrueIntegrity() / target_device.GetTrueIntegrity(true);
        }

        task.animated_rectangle.GetComponent<Image>().fillAmount = task.progress;
        if (Player_operator == 1)
        {
            float time = Mathf.Max(target_device.GetTrueIntegrity(true) - target_device.GetTrueIntegrity(true) / speed,
                target_router.GetTrueFirewallHealth(true) - target_router.GetTrueFirewallHealth() / speed);
            TaskBarController.UpdateTaskBar(task.id, "Repair " + target_device.GetIP(), Mathf.Round(time), task.progress);
        }

        if (target_device.GetTrueIntegrity() == target_device.GetTrueIntegrity(true) && firewall_full)
        {
            MakeIdle(task);
            if (Player_operator == 1)
            {
                ConsoleController.ShowText("Successful repair " + target_device.GetIP(), "normal");
                SFXPlayer.PlaySFX(SFXPlayer.SUCCESS);
            }
        }
    }
    public string CreateVPN(GameObject target)
    {
        DeviceScript device = target.GetComponent<DeviceScript>();
        string result = CommandPossible.CreateVPN(device, this);

        if (!CommandPossible.IsPossible(result))
        {
            return CommandPossible.GetUserStartErrorMessage(result);
        }
        else
        {
            if (CreateNewTask(CommandsDefinitions.create_vpn, target))
            {
                PlayerManager.RemovePCS(Player_operator, GetTrueCost(CommandsDefinitions.create_vpn));
                target.GetComponent<DeviceScript>().AttachTask(CommandsDefinitions.create_vpn, GetInternetDevice(), false,
                    CreateAnimation(EffectsStash.stash.creatingvpn, target.transform.position));
                return START_OK;
            }
            else
            {
                return REACHED_TASK_LIMIT;
            }
        }
    }
    private void DoCreateVPN(Task task)
    {
        DeviceScript target_device = task.target.GetComponent<DeviceScript>();
        ComputerScript target_computer = task.target.GetComponent<ComputerScript>();

        // possible errors in real-time
        string check_detail = CommandPossible.ProcessWithAdmin(target_device, this, true);
        if (!CommandPossible.IsPossible(check_detail))
        {
            if (Player_operator == 1)
            {
                ConsoleController.ShowUserProcessError("Estabilish VPN", target_device, check_detail);
            }
            MakeIdle(task);
            return;
        }

        float speed = (GetSuperPower()*0.5f + target_computer.GetTruePower()) / GetTrueWork(CommandsDefinitions.create_vpn)
            * Technology.SmartGetPluginBonus(CommandsDefinitions.create_vpn, target_device, true);
        task.progress += (speed * Time.deltaTime);
        if (task.progress >= 1.0f) // progress = 100%
        {
            GameObject last_vpn = gameObject; // remember itself for later reference
            MakeIdle(task);

            // create VPN connection link
            vpn_connection = target_device.gameObject; // store VPN gate machine 
            target_computer.GiveOrder("vpn_connection", gameObject); // give the VPN gate machine order to sustain VPN (animations...)


            if (Player_operator == 1)
            {
                ConsoleController.ShowText("Created VPN connection with " + target_device.GetIP(), "success");
                SFXPlayer.PlaySFX(SFXPlayer.SUCCESS);
            }

            foreach (Task t in tasks) // Change init_device in every Attached task
            {
                if (t.type != CommandsDefinitions.idle)
                {
                    t.target.GetComponent<DeviceScript>().ChangeInitDeviceForAttachedTask(last_vpn, t.type, GetInternetDevice());
                }
            }

            // make invisible
            This_device.FullyDisappear();
        }
        else
        {
            task.animated_rectangle.GetComponent<Image>().fillAmount = task.progress;
            if (Player_operator == 1)
            {
                TaskBarController.UpdateTaskBar(task.id, "VPN connecting " + target_device.GetIP(), (1.0f - task.progress) / speed, task.progress);
            }
        }
    }

    public string CreateSlave(GameObject target)
    {
        DeviceScript device = target.GetComponent<DeviceScript>();
        string result = CommandPossible.CreateSlave(device, this);

        if (!CommandPossible.IsPossible(result))
        {
            return CommandPossible.GetUserStartErrorMessage(result);
        }
        else
        {
            if (CreateNewTask(CommandsDefinitions.create_slave, target))
            {
                // TO DO: add higher PCS cost when more slaves present
                PlayerManager.RemovePCS(Player_operator, GetTrueCost(CommandsDefinitions.create_slave, task_created: true));
                target.GetComponent<DeviceScript>().AttachTask(CommandsDefinitions.create_slave, GetInternetDevice(), true,
                    CreateAnimation(EffectsStash.stash.creatingslave, target.transform.position));
                return START_OK;
            }
            else
            {
                return REACHED_TASK_LIMIT;
            }
        }
    }
    private void DoCreateSlave(Task task)
    {
        DeviceScript target_device = task.target.GetComponent<DeviceScript>();
        ComputerScript target_computer = task.target.GetComponent<ComputerScript>();

        // possible errors in real-time
        string check_detail = CommandPossible.ProcessWithAdmin(target_device, this, true);
        if (!CommandPossible.IsPossible(check_detail))
        {
            if (Player_operator == 1)
            {
                ConsoleController.ShowUserProcessError("Slave creation", target_device, check_detail);
            }
            MakeIdle(task);
            return;
        }

        float speed = (GetSuperPower() * 0.5f + target_computer.GetTruePower()) / GetTrueWork(CommandsDefinitions.create_slave)
            * Technology.SmartGetPluginBonus(CommandsDefinitions.create_slave, target_device, true);
        task.progress += (speed * Time.deltaTime);
        if (task.progress >= 1.0f) // progress = 100%
        {
            MakeIdle(task);

            slaves.Add(target_device.gameObject); 
            target_computer.GiveOrder("slave", gameObject);


            if (Player_operator == 1)
            {
                ConsoleController.ShowText("Slave created " + target_device.GetIP(), "normal");
                SFXPlayer.PlaySFX(SFXPlayer.SUCCESS);
            }

        }
        else
        {
            task.animated_rectangle.GetComponent<Image>().fillAmount = task.progress;
            if (Player_operator == 1)
            {
                TaskBarController.UpdateTaskBar(task.id, "New slave " + target_device.GetIP(), (1.0f - task.progress) / speed, task.progress);
            }
        }
    }
    public string InstallSoftware(GameObject target, string software)
    {
        DeviceScript device = target.GetComponent<DeviceScript>();
        string result = CommandPossible.InstallSoftware(device, this, software);

        if (!CommandPossible.IsPossible(result))
        {
            return CommandPossible.GetUserStartErrorMessage(result);
        }
        else
        {
            if (CreateNewTask(CommandsDefinitions.install_software, target, software))
            {
                PlayerManager.RemovePCS(Player_operator, GetTrueCost(software));
                target.GetComponent<DeviceScript>().AttachTask(CommandsDefinitions.install_software, GetInternetDevice(), false,
                    CreateAnimation(EffectsStash.stash.installing_software, target.transform.position));
                return START_OK;
            }
            else
            {
                return REACHED_TASK_LIMIT;
            }
        }
    }
    private void DoInstallSoftware(Task task)
    {
        DeviceScript target_device = task.target.GetComponent<DeviceScript>();
        ComputerScript target_computer = task.target.GetComponent<ComputerScript>();

        // possible errors in real-time
        string check_detail = CommandPossible.ProcessWithAdmin(target_device, this, true);
        if (!CommandPossible.IsPossible(check_detail))
        {
            if (Player_operator == 1)
            {
                ConsoleController.ShowUserProcessError("Installation", target_device, check_detail);
            }
            MakeIdle(task);
            return;
        }

        float speed = (GetSuperPower() * 0.5f + target_computer.GetTruePower()) 
            / GetTrueWork(CommandsDefinitions.install_software)
            * Technology.SmartGetPluginBonus(CommandsDefinitions.install_software, target_device, true);
        task.progress += (speed * Time.deltaTime);
        if (task.progress >= 1.0f) // progress = 100%
        {
            target_computer.GiveOrder(task.software, gameObject);
            if (Player_operator == 1)
            {
                ConsoleController.ShowText("Software "+task.software+" installed on " + target_device.GetIP(), "success");
                SFXPlayer.PlaySFX(SFXPlayer.SUCCESS);
            }
            MakeIdle(task);
        }
        else
        {
            task.animated_rectangle.GetComponent<Image>().fillAmount = task.progress;
            if (Player_operator == 1)
            {
                TaskBarController.UpdateTaskBar(task.id, 
                    "Install " + CommandsDefinitions.GetInstallName(task.software) + " " + target_device.GetIP(),
                    (1.0f - task.progress) / speed, task.progress);
            }
        }
    }
    public string UpdateBIOS(GameObject target, string update)
    {
        DeviceScript device = target.GetComponent<DeviceScript>();
        string result = CommandPossible.UpdateBIOS(device, this, update);

        if (!CommandPossible.IsPossible(result))
        {
            return CommandPossible.GetUserStartErrorMessage(result);
        }
        else
        {
            if (CreateNewTask(CommandsDefinitions.update_bios, target, update))
            {
                PlayerManager.RemovePCS(Player_operator, GetTrueCost(update+Upgrades.UPGRADE_SUFFIX, device));
                target.GetComponent<DeviceScript>().AttachTask(CommandsDefinitions.update_bios, GetInternetDevice(), false,
                    CreateAnimation(EffectsStash.stash.updating_bios, target.transform.position));
                return START_OK;
            }
            else
            {
                return REACHED_TASK_LIMIT;
            }
        }
    }
    private void DoUpdateBIOS(Task task)
    {
        DeviceScript target_device = task.target.GetComponent<DeviceScript>();

        // possible errors in real-time
        string check_detail = CommandPossible.ProcessWithAdmin(target_device, this, true);
        if (!CommandPossible.IsPossible(check_detail))
        {
            if (Player_operator == 1)
            {
                ConsoleController.ShowUserProcessError("BIOS update", target_device, check_detail);
            }
            MakeIdle(task);
            return;
        }

        float speed = GetSuperPower() / GetTrueWork(CommandsDefinitions.download_data, target_device)
            * Technology.SmartGetPluginBonus(CommandsDefinitions.update_bios, target_device, true);
        task.progress += (speed * Time.deltaTime);
        if (task.progress >= 1.0f) // progress = 100%
        {
            target_device.UpdateBIOS(task.software.Split('_')[0]);
            if (Player_operator == 1)
            {
                ConsoleController.ShowText("BIOS update " + task.software + " installed on " + target_device.GetIP(), "success");
                SFXPlayer.PlaySFX(SFXPlayer.SUCCESS);
            }
            MakeIdle(task);
        }
        else
        {
            task.animated_rectangle.GetComponent<Image>().fillAmount = task.progress;
            if (Player_operator == 1)
            {
                TaskBarController.UpdateTaskBar(task.id, "Update " + task.software + " " + target_device.GetIP(), (1.0f - task.progress) / speed, task.progress);
            }
        }
    }
    public string DownloadData(GameObject target)
    {
        // Scan a device (target)
        DeviceScript device = target.GetComponent<DeviceScript>();
        string result = CommandPossible.DownloadData(device, this);

        if (!CommandPossible.IsPossible(result))
        {
            return CommandPossible.GetUserStartErrorMessage(result);
        }
        else
        {
            if (CreateNewTask(CommandsDefinitions.download_data, target))
            {
                target.GetComponent<DeviceScript>().AttachTask(CommandsDefinitions.download_data, GetInternetDevice(), true,
                    CreateAnimation(EffectsStash.stash.downloading_data, target.transform.position));
                return START_OK;
            }
            else
            {
                return REACHED_TASK_LIMIT;
            }
        }
    }
    private void DoDownloadData(Task task)
    {
        DeviceScript target_device = task.target.GetComponent<DeviceScript>();

        // possible errors in real-time
        string check_detail = CommandPossible.ProcessWithAdmin(target_device, this, false);
        if (!CommandPossible.IsPossible(check_detail))
        {
            if (Player_operator == 1)
            {
                ConsoleController.ShowUserProcessError("Download files", target_device, check_detail);
            }
            MakeIdle(task);
            return;
        }

        float speed = GetSuperPower() / GetTrueWork(CommandsDefinitions.download_data)
            * Technology.SmartGetPluginBonus(CommandsDefinitions.download_data, target_device, true);
        task.progress += (speed * Time.deltaTime);
        if (task.progress >= 1.0f) // scan completed
        {
            string downloaded = target_device.DownloadContent();
            MakeIdle(task);
            if (Player_operator == 1)
            {
                ConsoleController.ShowText("Downloaded " + downloaded, "normal");
                DownloadedFiles.SaveFile(downloaded);
                SFXPlayer.PlaySFX(SFXPlayer.SUCCESS);
            }
        }
        else
        {
            task.animated_rectangle.GetComponent<Image>().fillAmount = task.progress;
            if (Player_operator == 1)
            {
                TaskBarController.UpdateTaskBar(task.id, "Download " + target_device.GetIP(), (1.0f - task.progress) / speed, task.progress);
            }

        }
    }

    public string TerminateDevice(GameObject target)
    {
        DeviceScript device = target.GetComponent<DeviceScript>();
        string result = CommandPossible.Terminate(device, this);

        if (!CommandPossible.IsPossible(result))
        {
            return CommandPossible.GetUserStartErrorMessage(result);
        }
        else
        {
            if (CreateNewTask(CommandsDefinitions.terminate, target))
            {
                target.GetComponent<DeviceScript>().AttachTask(CommandsDefinitions.terminate, GetInternetDevice(), false,
                    CreateAnimation(EffectsStash.stash.terminating, target.transform.position));
                return START_OK;
            }
            else
            {
                return REACHED_TASK_LIMIT;
            }
        }
    }
    private void DoTerminateDevice(Task task)
    {
        DeviceScript target_device = task.target.GetComponent<DeviceScript>();

        // possible errors in real-time
        string check_detail = CommandPossible.ProcessWithAdmin(target_device, this, false);
        if (!CommandPossible.IsPossible(check_detail))
        {
            if (Player_operator == 1)
            {
                ConsoleController.ShowUserProcessError("Termination", target_device, check_detail);
            }
            MakeIdle(task);
            return;
        }

        float speed = GetSuperPower() / target_device.GetTrueDefense() * 2.0f
            * Technology.SmartGetPluginBonus(CommandsDefinitions.terminate, target_device, true);
        task.progress += (speed * Time.deltaTime);
        if (task.progress >= target_device.GetTrueIntegrity(true)) // scan completed
        {
            target_device.Terminate();
            if (Player_operator == 1)
            {
                ConsoleController.ShowText("Terminated " + target_device.GetIP(), "normal");
            }
            MakeIdle(task);
        }
        else
        {
            task.animated_rectangle.GetComponent<Image>().fillAmount = task.progress;
            if (Player_operator == 1)
            {
                TaskBarController.UpdateTaskBar(task.id, "Terminate " + target_device.GetIP(),
                    (1.0f - task.progress / target_device.GetTrueIntegrity(true)) / speed,
                    task.progress / target_device.GetTrueIntegrity(true));
            }

        }
    }

    public string InstallTorrent(string torrent_id, int pcs)
    {
        if (Installed_torrents.Contains(torrent_id))
        {
            return "-Torrent already installed";
        }
        else if (pcs > PlayerManager.GetPCS(Player_operator))
        {
            return "-Not enough PCS";
        }
        else
        {
            PlayerManager.RemovePCS(Player_operator, pcs);
            Installed_torrents.Add(torrent_id);

            TorrentButton torrent = TorrentButton.torrents[torrent_id];
            Technology.ApplySmartUpgrade(torrent.bonus, torrent.value);
            return "+Successful torrent installation";
        }
    }

    public string InstallUpgrade(string product, int catcoin)
    {
        if (Installed_upgrades.Contains(product))
        {
            return "-Upgrade already installed";
        }
        else if (catcoin > PlayerManager.GetCatCoin(Player_operator))
        {
            return "-Not enough Catcoin";
        }
        else
        {
            PlayerManager.RemoveCatCoin(Player_operator, catcoin);
            Installed_upgrades.Add(product);

            OfferController offer = OfferController.offers[product];
            Technology.ApplySmartUpgrade(offer.bonus, offer.value);
            return "+Successful upgrade installation";
        }
    }
    public float GetTrueWork(string key, DeviceScript device = null)
    {
        if (key.EndsWith(Upgrades.UPGRADE_SUFFIX))
        {
            if (device == null) Debug.LogError("OperatorScript: GetTrueWork no device given");
            string upgrade = key.Substring(0, key.IndexOf("_"));
            int lvl = 0;
            switch (upgrade)
            {
                case Upgrades.DEFENSE:
                    lvl = device.upgrades.defense;
                    break;
                case Upgrades.SECURITY:
                    lvl = device.upgrades.security;
                    break;
                case Upgrades.POWER:
                    lvl = device.upgrades.security;
                    break;
                case Upgrades.FIREWALL:
                    lvl = device.upgrades.firewall;
                    break;
                default:
                    Debug.LogError("OperatorScript: GetTrueWork upgrade error");
                    break;
            }
            return work_amounts[CommandsDefinitions.update_bios] + lvl * work_amounts[CommandsDefinitions.extra_update_bios];
        }
        return work_amounts[key];
    }
    public float GetSlavePenalty(bool coefficient_formatted = true)
    {
        if (coefficient_formatted) return 1.0f + 0.5f * slaves.Count;
        return 0.5f * slaves.Count;
    }
    public int GetTrueCost(string key, DeviceScript device = null, bool task_created = false)
    {
        if (key == CommandsDefinitions.create_slave || key == CommandsDefinitions.slave)
        {
            int creating_slaves = 0;
            if (task_created) creating_slaves--;
            foreach (Task task in tasks)
            {
                if (task.type == CommandsDefinitions.create_slave)
                {
                    creating_slaves++;
                }
            }
            return Mathf.RoundToInt((pcs_cost[CommandsDefinitions.extra_slave] * (slaves.Count + creating_slaves - 1)
                + pcs_cost[CommandsDefinitions.create_slave]) * Technology.GetPCSCostModifier(key, true));
        }
        if (key.EndsWith(Upgrades.UPGRADE_SUFFIX))
        {
            if (device == null) Debug.LogError("OperatorScript: GetTrueCost no device given");
            string upgrade = key.Substring(0, key.IndexOf("_"));
            int lvl = 0;
            switch (upgrade)
            {
                case Upgrades.DEFENSE:
                    lvl = device.upgrades.defense;
                    break;
                case Upgrades.SECURITY:
                    lvl = device.upgrades.security;
                    break;
                case Upgrades.POWER:
                    lvl = device.upgrades.security;
                    break;
                case Upgrades.FIREWALL:
                    lvl = device.upgrades.firewall;
                    break;
                default:
                    Debug.LogError("OperatorScript: GetTrueCost upgrade error");
                    break;
            }
            return Mathf.CeilToInt(pcs_cost[key] * Mathf.Pow(2.0f, lvl)
                * Technology.GetPCSCostModifier(key, true));
        }
        return Mathf.CeilToInt(pcs_cost[key] * Technology.GetPCSCostModifier(key, true));
    }
    public bool ReachedSlaveCapacity()
    {
        int creating_slaves = 0;
        foreach (Task task in tasks)
        {
            if (task.type == CommandsDefinitions.create_slave)
            {
                creating_slaves++;
            }
        }
        return slaves.Count + creating_slaves >= slaves_capacity;
    }

    public void ExtendTaskCapacity(int extend)
    {
        for(int i=task_capacity; i<task_capacity+extend; i++)
        {
            tasks.Add(new Task()
            {
                id = i,
                type = "idle",
                command_code = 0,
                target = null,
                progress = 1.0f,
                animated_rectangle = null
            });
            if(Player_operator == 1) TaskBarController.ActivateTaskbar(i, true);
        }
        task_capacity += extend;
    }

    public GameObject GetInternetDevice() // get internet IP address, returns vpn_connection if VPN was estabilished
    {
        // if operator has a VPN connection --> return VPN gate machine
        // if it doesn't have --> return itself
        if (vpn_connection == null) 
        {
            return gameObject;
        }
        else 
        {
            return vpn_connection;
        }
    }
    public string UninstallComputer(GameObject target)
    {
        DeviceScript device = target.GetComponent<DeviceScript>();
        ComputerScript computer = target.GetComponent<ComputerScript>();

        string result = CommandPossible.Uninstall(device, this);

        if (!CommandPossible.IsPossible(result))
        {
            return CommandPossible.GetUserStartErrorMessage(result);
        }
        else
        {
            //Debug.Log("operator func, aT = "+device.attachedTasks.Count.ToString());
            computer.GiveOrder(CommandsDefinitions.uninstall, null);
            return START_OK;
        }
    }

    public void LoseVPN() // lose VPN connection
        // used when device which is a VPN gate is lost
    {
        GameObject last_vpn = GetInternetDevice();
        vpn_connection = null;
        foreach (Task t in tasks)
        {
            if (t.type != "idle")
            {
                t.target.GetComponent<DeviceScript>().ChangeInitDeviceForAttachedTask(last_vpn, t.type, GetInternetDevice());
            }
        }
    }

    public void LoseSlave(GameObject target_slave)
    {
        foreach(GameObject slave in slaves)
        {
            if(slave == target_slave)
            {
                slaves.Remove(slave);
                return;
            }
        }
    }

    public void Defeat()
    {
        foreach (Task task in tasks)
        {
            MakeIdle(task);
        }
        if (vpn_connection != null)
        {
            vpn_connection.GetComponent<ComputerScript>().MakeIdle();
            LoseVPN();
        }
        foreach(GameObject slave in slaves)
        {
            slave.GetComponent<ComputerScript>().MakeIdle();
            LoseSlave(slave);
        }
        defeated = true;
        This_computer.is_active_operator = false;
        PlayerManager.DefeatPlayer(Player_operator);
        foreach (Transform child in transform)
        {
            GameObject g = child.gameObject;
            if (g.name == "Operator monitor")
            {
                Destroy(g);
            }
        }
    }
}
public class Task // task class
{
    public int id; // id of the task, important for ui
    public string type; // examples: cyberattack, scan...
    public int command_code; // id of the command
    public GameObject target; // task is done on a device/target
    public float progress;
    public GameObject animated_rectangle;
    public string software;
    public AttachedTask attachedTask;
}

public class Technology
{
    private OperatorScript belonging_operator;
    public Dictionary<string, float> plugin_bonuses = new Dictionary<string, float>()
    {
        {CommandsDefinitions.scan, 0.0f},
        {CommandsDefinitions.tcpcatch, 0.0f},
        {CommandsDefinitions.antivirus_scan, 0.0f},
        {CommandsDefinitions.cyberattack, 0.0f},
        {CommandsDefinitions.trojan, 0.0f},
        {CommandsDefinitions.break_firewall, 0.0f},
        {CommandsDefinitions.inject_spyworm, 0.0f},
        {CommandsDefinitions.delete_log, 0.0f},
        {CommandsDefinitions.counterattack, 0.0f},
        {CommandsDefinitions.create_stasis, 0.0f},
        {CommandsDefinitions.repair, 0.0f},
        {CommandsDefinitions.create_vpn, 0.0f},
        {CommandsDefinitions.create_slave, 0.0f},
        {CommandsDefinitions.install_software, 0.0f},
        {CommandsDefinitions.update_bios, 0.0f},
        {CommandsDefinitions.download_data, 0.0f},
        {CommandsDefinitions.terminate, 0.0f},
    };
    public Dictionary<string, float> install_bonuses = new Dictionary<string, float>()
    {
        {CommandsDefinitions.autoantivirus, 0.0f},
        {CommandsDefinitions.autorepair, 0.0f},
        {CommandsDefinitions.botnet, 0.0f},
    };
    public Dictionary<string, float> device_type_bonus = new Dictionary<string, float>()
    {
        {"Computer", 0.0f},
        {"Router", 0.0f},
        {"Server", 0.0f},
    };
    public Dictionary<string, float> cost_bonuses = new Dictionary<string, float>()
    {
        {CommandsDefinitions.inject_spyworm, 0.0f },
        {CommandsDefinitions.create_stasis, 0.0f },
        {CommandsDefinitions.create_vpn, 0.0f },
        {CommandsDefinitions.create_slave, 0.0f },
        {CommandsDefinitions.extra_slave, 0.0f },
        {CommandsDefinitions.generate_pcs, 0.0f },
        {CommandsDefinitions.mine_catcoin, 0.0f },
        {CommandsDefinitions.autoantivirus, 0.0f },
        {CommandsDefinitions.autorepair, 0.0f },
        {CommandsDefinitions.botnet, 0.0f },

        {Upgrades.DEFENSE+Upgrades.UPGRADE_SUFFIX, 0.0f },
        {Upgrades.SECURITY+Upgrades.UPGRADE_SUFFIX, 0.0f },
        {Upgrades.POWER+Upgrades.UPGRADE_SUFFIX, 0.0f },
        {Upgrades.FIREWALL+Upgrades.UPGRADE_SUFFIX, 0.0f },
    };
    public Dictionary<string, float> resources_bonuses = new Dictionary<string, float>()
    {
        {Resources.PCS, 0.0f },
        {Resources.CATCOIN, 0.0f },
        {Resources.STOLEN_DATA, 0.0f },
    };
    public Technology(OperatorScript input_operator)
    {
        belonging_operator = input_operator;
    }
    public float SmartGetPluginBonus(string key, DeviceScript device, bool coefficient_formatted)
    {
        return SmartGetPluginBonus(key, device.type, coefficient_formatted);
    }
    public float SmartGetPluginBonus(string key, GameObject target_device, bool coefficient_formatted)
    {
        return SmartGetPluginBonus(key, target_device.GetComponent<DeviceScript>().type, coefficient_formatted);
    }
    public float SmartGetPluginBonus(string key, string device_type, bool coefficient_formatted)
    {
        float bonus = plugin_bonuses[key];
        foreach(string bonus_type in device_type_bonus.Keys)
        {
            if(DeviceManagment.BelongsToCategory(device_type, bonus_type))
            {
                bonus += device_type_bonus[bonus_type];
            }
        }
        if (coefficient_formatted) return bonus + 1.0f;
        else return bonus;
    }
    public float SmartGetInstallBonus(string key, string device_type, bool coefficient_formatted)
    {
        float bonus = install_bonuses[key];
        foreach (string bonus_type in device_type_bonus.Keys)
        {
            if (DeviceManagment.BelongsToCategory(device_type, bonus_type))
            {
                bonus += device_type_bonus[bonus_type];
            }
        }
        if (coefficient_formatted) return bonus + 1.0f;
        else return bonus;
    }
    public float GetPluginBonus(string key, bool coefficient_formatted)
    {
        if (coefficient_formatted) return plugin_bonuses[key] + 1.0f;
        else return plugin_bonuses[key];
    }
    public float GetPCSCostModifier(string key, bool coefficient_formatted)
    {
        if (coefficient_formatted) return 1.0f - cost_bonuses[key];
        else return -cost_bonuses[key];
    }
    public float GetResourceGenerationModifier(string key, bool coefficient_formatted)
    {
        if (coefficient_formatted) return resources_bonuses[key] + 1.0f;
        else return resources_bonuses[key];
    }
    public void ApplySmartUpgrade(string bonus, float value)
    {
        /* Enter positive values to give bonuses
         * Enter negative values to give disadvantages
         * 
         * Upgrade syntax:
         * category type
         * 
         * Categories:
         *  plugin - boosts for plugins (plugin scan)
         *  install - boosts for installed software (install botnet)
         *  device - boosts on device types (device computer)
         *  cost - PCS discounts (cost autorepair)
         *  operator - upgrades for operator (operator task)
         */
        string[] arguments = bonus.Split();
        switch (arguments[0])
        {
            case "plugin":
                plugin_bonuses[arguments[1]] += value;
                break;
            case "install":
                install_bonuses[arguments[1]] += value;
                break;
            case "device":
                device_type_bonus[arguments[1]] += value;
                break;
            case "cost":
                cost_bonuses[arguments[1]] += value;
                break;
            case "operator":
                break;
        }
    }
}
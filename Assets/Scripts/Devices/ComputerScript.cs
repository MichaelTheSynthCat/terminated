using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// ComputerScript class is attached to every computer(device which should act like a computer, like desktops, notebooks, servers)
// - has power attribute
// - can do tasks like operator (but only one at a time)

public class ComputerScript : MonoBehaviour
{
    public float base_power = 10.0f;
    public bool is_active_operator = false;
    private Task own_task = new Task() {id = 0, type = CommandsDefinitions.idle, command_code = 0,
        target = null, progress = 1.0f};
    private Task foreign_task = new Task() { id = 0, type = CommandsDefinitions.idle, target = null };

    public DeviceScript This_device { get; private set; }
    private float accumulated_power = 0.0f; // accumulated power during an active task

    private TextMeshPro powergui;

    private void Awake()
    {
        This_device = GetComponent<DeviceScript>();
        foreach (Transform child in transform)
        {
            string child_name = child.name;
            if (child_name == "Power NUM")
            {
                powergui = child.GetComponent<TextMeshPro>();
            }
        }
    }

    void Start()
    {
        ReloadRenderer();
    }

    // Update is called once per frame
    void Update()
    {
        switch (own_task.command_code)
        {
            case CommandCodes.idle:
                break;
            case CommandCodes.vpn_connection:
                break;
            case CommandCodes.slave:
                break;
            case CommandCodes.generate_pcs:
                GeneratePCS();
                break;
            case CommandCodes.mine_catcoin:
                GenerateCatCoin();
                break;
            case CommandCodes.autoantivirus:
                AntivirusEngineLoop();
                break;
            case CommandCodes.autorepair:
                AutoRepairLoop();
                break;
            case CommandCodes.botnet:
                BotnetLoop();
                break;
            default:
                Debug.LogError("ComputerScript.Update(): unknown task.command_code - " + own_task.command_code.ToString());
                break;
        }
    }

    public void GiveOrder(string order, GameObject target) // give order to do a new task
    {
        if(order == CommandsDefinitions.uninstall)
        {
            if (GetOwnTask().type == CommandsDefinitions.vpn_connection)
            {
                DeviceManagment.GetOperator(This_device.player).GetComponent<OperatorScript>().LoseVPN();
            }
            if (GetOwnTask().type == CommandsDefinitions.slave)
            {
                DeviceManagment.GetOperator(This_device.player).GetComponent<OperatorScript>().LoseSlave(gameObject);
            }
            //Debug.Log("computer func, aT = "+this_device.attachedTasks.Count.ToString());
            MakeIdle(own_task);
            return;
        }
        switch (order)
        {
            case CommandsDefinitions.vpn_connection:
                own_task = new Task() { id = 0, type = CommandsDefinitions.vpn_connection,
                    command_code = CommandCodes.vpn_connection, target = gameObject };
                own_task.attachedTask = This_device.AttachTask(CommandsDefinitions.vpn_connection, gameObject, true,
                    CreateAnimation(EffectsStash.stash.vpn_connection, transform.position));
                break;
            case CommandsDefinitions.slave:
                own_task = new Task() { id = 0, type = CommandsDefinitions.slave,
                    command_code = CommandCodes.slave,
                    target = gameObject };
                own_task.attachedTask = This_device.AttachTask(CommandsDefinitions.slave, gameObject, true,
                    CreateAnimation(EffectsStash.stash.slave_process, transform.position));
                break;
            case CommandsDefinitions.generate_pcs:
                own_task = new Task() { id = 0, type = CommandsDefinitions.generate_pcs,
                    command_code = CommandCodes.generate_pcs,
                    target = gameObject };
                own_task.attachedTask = This_device.AttachTask(CommandsDefinitions.generate_pcs, gameObject, false,
                    CreateAnimation(EffectsStash.stash.generating_pcs, transform.position));
                break;
            case CommandsDefinitions.mine_catcoin:
                own_task = new Task() { id = 0, type = CommandsDefinitions.mine_catcoin,
                    command_code = CommandCodes.mine_catcoin,
                    target = gameObject };
                own_task.attachedTask = This_device.AttachTask(CommandsDefinitions.mine_catcoin, gameObject, false,
                    CreateAnimation(EffectsStash.stash.mining_catcoin, transform.position));
                break;
            case CommandsDefinitions.autoantivirus:
                own_task = new Task { id = 0, type = CommandsDefinitions.autoantivirus,
                    command_code = CommandCodes.autoantivirus,
                    target = gameObject };
                own_task.attachedTask = This_device.AttachTask(CommandsDefinitions.autoantivirus, gameObject, false,
                     CreateAnimation(EffectsStash.stash.antivirus_engine, transform.position));
                break;
            case CommandsDefinitions.autorepair:
                own_task = new Task { id = 0, type = CommandsDefinitions.autorepair,
                    command_code = CommandCodes.autorepair,
                    target = gameObject };
                own_task.attachedTask = This_device.AttachTask(CommandsDefinitions.autorepair, gameObject, false,
                     CreateAnimation(EffectsStash.stash.autorepair, transform.position));
                break;
            case CommandsDefinitions.botnet:
                own_task = new Task { id = 0, type = CommandsDefinitions.botnet,
                    command_code = CommandCodes.botnet,
                    target = gameObject };
                own_task.attachedTask = This_device.AttachTask(CommandsDefinitions.botnet, gameObject, false,
                     CreateAnimation(EffectsStash.stash.botnet_source, transform.position));
                break;
            default:
                Debug.LogError("Unknown order " + order + ". Check ComputerScript.GiveOrder().");
                break;
        }
    }
    public void MakeIdle() // make own_task idle, but without a parameter
    {
        MakeIdle(own_task);
    }
    public void MakeIdle(Task task) // make task idle
    {
        EndForeignTask();
        if(task.type != CommandsDefinitions.idle)
        {
            task.target.GetComponent<DeviceScript>().DetachTask(gameObject, task.type);
            task.type = CommandsDefinitions.idle;
            task.command_code = 0;
            task.target = null;
            task.progress = 1.0f;
            accumulated_power = 0.0f;
        }
    }
    public void EndForeignTask() // end foreign task / animation
    {
        if (foreign_task.type != CommandsDefinitions.idle)
        {
            foreign_task.target.GetComponent<DeviceScript>().DetachTask(gameObject, foreign_task.type);
            foreign_task.type = CommandsDefinitions.idle;
            foreign_task.target = null;
            foreign_task.progress = 1.0f;
        }
    }
    private void GeneratePCS() // generate PCS
    {
        OperatorScript my_operator = DeviceManagment.GetOperator(This_device.player).GetComponent<OperatorScript>();
        accumulated_power += GetTruePower() * Time.deltaTime
            * my_operator.Technology.GetResourceGenerationModifier(Resources.PCS, true);
        if(accumulated_power >= my_operator.work_amounts[Resources.PCS])
        {
            float generated = Mathf.Floor(accumulated_power / my_operator.work_amounts[Resources.PCS]);
            accumulated_power -= generated * my_operator.work_amounts[Resources.PCS];
            PlayerManager.AddPCS(This_device.player, (int)generated);
        }
    }
    private void GenerateCatCoin() // generate CatCoin
    {
        OperatorScript my_operator = DeviceManagment.GetOperator(This_device.player).GetComponent<OperatorScript>();
        accumulated_power += GetTruePower() * Time.deltaTime 
            * my_operator.Technology.GetResourceGenerationModifier(Resources.CATCOIN, true);
        if (accumulated_power >= my_operator.work_amounts[Resources.CATCOIN])
        {
            float generated = Mathf.Floor(accumulated_power / my_operator.work_amounts[Resources.CATCOIN]);
            accumulated_power -= generated * my_operator.work_amounts[Resources.CATCOIN];
            PlayerManager.AddCatCoin(This_device.player, (int)generated);
        }
    }
    private void BotnetLoop() // botnet's lifecycle
    {
        OperatorScript my_operator = DeviceManagment.GetOperator(This_device.player).GetComponent<OperatorScript>();
        if(foreign_task.target == null) // no active target
        {
            // find all possible targets
            List<GameObject> target_devices = new List<GameObject>();
            foreach(GameObject device_object in This_device.GetMasterRouter().GetComponent<RouterScript>().local_connections)
            {
                DeviceScript device = device_object.GetComponent<DeviceScript>();
                if (!device.CanPlayerSee(This_device.player, true))
                {
                    continue;
                }
                else if (device.IsTerminated())
                {
                    continue;
                }
                else if (device.IsFriendlyWith(This_device.player))
                {
                    continue;
                }
                else if (DeviceManagment.BelongsToCategory(device_object, "Router"))
                {
                    continue;
                }
                else if(device.GetTrueSecurity() >= my_operator.botnet_level)
                {
                    continue;
                }
                else if (device.HasStasis())
                {
                    continue;
                }
                else
                {
                    target_devices.Add(device_object);
                }
            }
            if(target_devices.Count > 0)
            {
                // choose randomly a target device
                foreign_task.target = target_devices[Random.Range(0, target_devices.Count)];
                foreign_task.type = "botnet_attack";
                foreign_task.target.GetComponent<DeviceScript>().AttachTask("botnet_attack", gameObject, false,
                     CreateAnimation(EffectsStash.stash.botnet_attacking, foreign_task.target.transform.position));
            }
        }
        else
        {
            DeviceScript device = foreign_task.target.GetComponent<DeviceScript>();
            bool end_now = false;
            if (!device.CanPlayerSee(This_device.player))
            {
                end_now = true;
            }
            else if (device.IsTerminated())
            {
                end_now = true;
            }
            else if (device.IsFriendlyWith(This_device.player))
            {
                end_now = true;
            }
            else if (device.GetTrueSecurity() >= my_operator.botnet_level)
            {
                end_now = true;
            }
            else if (device.HasStasis())
            {
                end_now = true;
            }

            if (end_now)
            {
                EndForeignTask();
            }
            else
            {
                float speed = GetTruePower() * 0.5f / device.GetTrueDefense()
                    * my_operator.Technology.SmartGetInstallBonus(CommandsDefinitions.botnet, device.type, true);
                device.ChangeIntegrity(-speed * Time.deltaTime);
                if (device.GetTrueIntegrity() <= 0.0f)
                {
                    if (This_device.player == 1)
                    {
                        ConsoleController.ShowText("Device " + device.GetIP() + 
                            " captured by bot {" + This_device.GetIP() + "}", "success");
                    }
                    device.ChangeControl(This_device.player, true, gameObject);
                    EndForeignTask();
                }
            }
        }
    }
    private void AntivirusEngineLoop() // auto-antivirus lifecycle
    {
        OperatorScript my_operator = DeviceManagment.GetOperator(This_device.player).GetComponent<OperatorScript>();
        if(foreign_task.target == null) // no active target
        {
            foreign_task.target = AntivirusEngineAutoChoose(); // choosing mechanism
            foreign_task.type = CommandsDefinitions.antivirus_scan;
            foreign_task.progress = 0.0f;
            if (foreign_task.target != null)
            {
                foreign_task.target.GetComponent<DeviceScript>().AttachTask(CommandsDefinitions.antivirus_scan, gameObject, true,
                        CreateAnimation(EffectsStash.stash.antivirus_scan, foreign_task.target.transform.position));
            }
        }
        else
        {
            DeviceScript device = foreign_task.target.GetComponent<DeviceScript>();
            bool end_now = false;
            if (!device.CanPlayerSee(This_device.player, true))
            {
                end_now = true;
            }
            else if (device.IsTerminated())
            {
                end_now = true;
            }
            else if (device.player != This_device.player)
            {
                end_now = true;
            }

            if (end_now)
            {
                EndForeignTask();
            }
            else
            {
                float speed = GetTruePower() /  my_operator.work_amounts[CommandsDefinitions.antivirus_scan];
                foreign_task.progress += speed * Time.deltaTime;
                if (foreign_task.progress >= 1.0f) // antivirus completed
                {
                    device.RemoveMalware(my_operator.antivirus_level);
                    GameObject last = foreign_task.target;
                    EndForeignTask();

                    // choose next device
                    foreign_task.target = AntivirusEngineAutoChoose(last);
                    foreign_task.type = CommandsDefinitions.antivirus_scan;
                    foreign_task.progress = 0.0f;
                    if (foreign_task.target != null)
                    {
                        foreign_task.target.GetComponent<DeviceScript>().AttachTask(CommandsDefinitions.antivirus_scan,
                            gameObject, true,
                            CreateAnimation(EffectsStash.stash.antivirus_scan, foreign_task.target.transform.position));
                    }
                }
            }
        }

    }
    private GameObject AntivirusEngineAutoChoose(GameObject last = null) // mechanism that orderly chooses target devices which should be antivirus-scanned
    {
        List<GameObject> target_devices = new List<GameObject>();
        bool return_next = false;
        foreach (GameObject device_object in This_device.GetMasterRouter().GetComponent<RouterScript>().local_connections)
        {
            DeviceScript device = device_object.GetComponent<DeviceScript>();
            if (!device.CanPlayerSee(This_device.player, true))
            {
                continue;
            }
            else if (device.IsTerminated())
            {
                continue;
            }
            else if (device.player != This_device.player)
            {
                continue;
            }
            else if(device_object == gameObject)
            {
                continue;
            }

            bool already_scanning = false;
            foreach(AttachedTask attachedTask in device.attachedTasks)
            {
                if (attachedTask.task_name == "antivirusscan")
                {
                    already_scanning = true;
                    break;
                }
            }
            if (already_scanning) { continue; }

            target_devices.Add(device_object);
            if (return_next)
            {
                return device_object;
            }
            if (device_object == last)
            {
                return_next = true;
            }
        }
        if(target_devices.Count == 0)
        {
            return null;
        }
        return target_devices[0];
    }

    private void AutoRepairLoop() // auto-repair lifecycle
    {
        OperatorScript my_operator = DeviceManagment.GetOperator(This_device.player).GetComponent<OperatorScript>();
        if (foreign_task.target == null)
        {
            foreach (GameObject device_object in This_device.GetMasterRouter().GetComponent<RouterScript>().local_connections)
            {
                DeviceScript device = device_object.GetComponent<DeviceScript>();
                if (!device.CanPlayerSee(This_device.player, true))
                {
                    continue;
                }
                else if (device.player != This_device.player)
                {
                    continue;
                }
                else if (!device.IsSafe())
                {
                    continue;
                }
                else if (device.GetTrueIntegrity() >= device.GetTrueIntegrity(true))
                {
                    continue;
                }
                else if (device_object == gameObject)
                {
                    continue;
                }
                foreign_task.target = device_object;
                foreign_task.type = CommandsDefinitions.autorepair;
                foreign_task.target.GetComponent<DeviceScript>().AttachTask(CommandsDefinitions.autorepair, gameObject, false,
                        CreateAnimation(EffectsStash.stash.repairing, foreign_task.target.transform.position));
                break;
            }
        }
        else
        {
            DeviceScript device = foreign_task.target.GetComponent<DeviceScript>();
            bool end_now = false;
            if (!device.CanPlayerSee(This_device.player, true))
            {
                end_now = true;
            }
            else if (device.player != This_device.player)
            {
                end_now = true;
            }
            else if (!device.IsSafe())
            {
                end_now = true;
            }
            else if (device.GetTrueIntegrity() >= device.GetTrueIntegrity(true))
            {
                end_now = true;
            }

            if (end_now)
            {
                EndForeignTask();
            }
            else
            {
                float speed = GetTruePower() / my_operator.repair_integrity_coefficient;
                device.ChangeIntegrity(speed * Time.deltaTime);
                if (device.GetTrueIntegrity() >= device.GetTrueIntegrity(true))
                {
                    EndForeignTask();
                }
            }
        }
    }
    public static GameObject CreateAnimation(GameObject prefab, Vector3 position) // create effect animation
    {
        return Instantiate(prefab, position, Quaternion.identity);
    }

    public Task GetOwnTask()
    {
        return own_task;
    }
    public int GetInstalledSoftwareCode()
    {
        return own_task.command_code;
    }
    public string GetInstalledSoftware()
    {
        return own_task.type;
    }
    public GameObject GetForeignTaskTarget()
    {
        return foreign_task.target;
    }
    public float GetTruePower()
    {
        float power = base_power;
        OperatorScript my_operator = DeviceManagment.GetOperator(This_device.player).GetComponent<OperatorScript>();
        if (This_device.upgrades.power > 0)
        {
            power += my_operator.upgrade_boost[Upgrades.POWER] * This_device.upgrades.power;
        }
        return power;
    }

    private void ShowPower(float value)
    {
        powergui.text = ((int)value).ToString();
        powergui.color = EffectsStash.stash.power_upgrades[This_device.upgrades.power];
    }
    public void ReloadRenderer()
    {
        ShowPower(GetTruePower());
    }
}

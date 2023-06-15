using INT = System.Int32;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


// DeviceScript must be attached to every interactable object/unit/device (desktops, routers, servers, etc.).

public class DeviceScript : MonoBehaviour
{
    // main vars
    public int Id { get; private set; } // global id of the device, don't change, given when the scene is played
    public string local_ip = "x"; // local ip address, use capitalized letters for master routers and numbers for slaves
    public string type; // game type of the device, examples: router, desktop, server
    private GameObject master_router = null; // contains master router
    private bool terminated = false; // device terminated

    // relations with players
    public int player; // the id of the player who controls the device (has full access)
    public List<int> visible; // contains ids of players who can see this computer (not stats)
    public bool stealth_identity; // set true if device's identity is hidden from other players, show identity if device is scanned by a player
    public List<int> scanned; // contains ids of players who can see stats of this computer
    public GameObject Attack_log  { get; private set; } // log that contains the ip of a device that took control over this device last time, can be removed (null)


    // attached tasks and processes
    public List<AttachedTask> attachedTasks { get; private set; }

    // upgrades and effects
    public List<int> spyworm = new List<int>(); // contains ids of players that installed spyworm on this device

    // stats of the device
    public float max_integrity; // maximal possible integrity level
    public float integrity; // current integrity level
    public float regeneration; // regeneration rate when the device is idle
    public float base_defense; // starting defense level, without bonuses and upgrades
    public int base_security; // starting security level, -||-

    // vars for checking safe_mode (device is not under violent attack or negative effect and can regenerate itself)
    private const float safe_mode_deadline = 3.0f;
    private float safe_mode_timer = 0.0f; // if lower or equals 0 than the device is in safe mode
    private float active_stasis = 0.0f; // how long will the stasis remain active

    // upgrades
    public Upgrades upgrades = new Upgrades(); // class for installed upgrades

    // downloads
    public string download_file = ""; // file that can be downloaded, "" for nothing

    // name random generation
    public string auto_name = "";
    // start commands
    public List<string> startup_commands;


    // textmesh vars
    private TextMeshPro ipgui, defensegui, securitygui;

    // integrity mask script
    private IntegrityMask integrity_bar;

    // effect icons
    private GameObject logpresent_icon, visible_icon, danger_icon, spyworm_icon, stasis_icon, download_icon;

    // static values
    private static List<string> scan_required = new List<string>() {
        "Defense NUM", "Defense icon", "Security icon", "Security NUM", "Power icon", "Power NUM",
    "Firewall icon", "Firewall NUM", };
    private static List<string> spyworm_required = new List<string>() { "Operator monitor", };
    private static List<string> admin_required = new List<string>() {"Effect LogPresent", "Effect Visible", 
        "Effect Danger", "Effect Download"};
    private void Awake()
    {
        Id = DeviceManagment.Add_device(gameObject); // add to all devices list
        attachedTasks = new List<AttachedTask>();// get text components TextMeshPro (not gui) and IntegrityScript
        foreach (Transform child in transform)
        {
            string child_name = child.name;
            if (child_name == "IP Address")
            {
                ipgui = child.GetComponent<TextMeshPro>();
            }
            if (child_name == "Integrity Mask")
            {
                integrity_bar = child.GetComponent<IntegrityMask>();
            }
            if (child_name == "Defense NUM")
            {
                defensegui = child.GetComponent<TextMeshPro>();
            }
            if (child_name == "Security NUM")
            {
                securitygui = child.GetComponent<TextMeshPro>();
            }
            if (child_name == "Effect LogPresent")
            {
                logpresent_icon = child.gameObject;
            }
            if (child_name == "Effect Visible")
            {
                visible_icon = child.gameObject;
            }
            if (child_name == "Effect Danger")
            {
                danger_icon = child.gameObject;
            }
            if (child_name == "Effect Spyworm")
            {
                spyworm_icon = child.gameObject;
                spyworm_icon.SetActive(false);
            }
            if (child_name == "Effect Stasis")
            {
                stasis_icon = child.gameObject;
            }
            if (child_name == "Effect Download")
            {
                download_icon = child.gameObject;
            }
        }
    }
    void Start()
    {
        // DEBUG
        if(local_ip == "x")
        {
            if(GetComponent<OperatorScript>() == null)
            {
                Debug.LogWarning("Enter valid IP address for " + gameObject.name);
            }
            else if (!GetComponent<OperatorScript>().dummy)
            {
                Debug.LogWarning("Enter valid IP address for " + gameObject.name);
            }
        }
        if(master_router == null && !DeviceManagment.BelongsToCategory(gameObject, "Router"))
        {
            if(GetComponent<OperatorScript>() == null)
            {
                Debug.LogWarning("No connection to internet for " + gameObject.name);
            }
            else
            {
                if (!GetComponent<OperatorScript>().dummy)
                {
                    Debug.LogWarning("No connection to internet for " + gameObject.name);
                }
            }
        }
        if(player > PlayerManager.GetPlayerCount())
        {
            Debug.LogWarning("Player " + player + " is not present in the game!");
        }

        // show ip address
        ipgui.SetText(GetIP(true));

        // show integrity, defense
        integrity_bar.Show(GetTrueIntegrity() / GetTrueIntegrity(true));
        ShowDefense(GetTrueDefense());
        ShowSecurity(GetTrueSecurity());


        if(auto_name != "")
        {
            if(auto_name[0] == '?')
            {
                gameObject.name = NameGenerator.GenerateName(auto_name.Substring(1).Split());
            }
            if (auto_name[0] == '+')
            {
                gameObject.name += " " + NameGenerator.GenerateName(auto_name.Substring(1).Split());
            }
        }
        foreach(string line in startup_commands)
        {
            ExecuteDeviceCommand(line.Split());
        }

        // visible to player 1
        GameReloadRenderer();
    }
    void Update()
    {
        // show or hide attached tasks
        foreach (AttachedTask attachedTask in attachedTasks) // show attached task's animation to player 1 (user)
        {
            // user must see the device and have installed spyworms
            attachedTask.ShowAnimation(CanPlayerSeeAttachedTask(1, attachedTask) && CanPlayerSee(1));
        }

        // effect icons
        if (player == 1 && !terminated) // shows various icons
        {
            logpresent_icon.SetActive(Attack_log != null);

            
            visible_icon.SetActive(IsVisibleToEnemies());

            danger_icon.SetActive(!IsSafe());

            download_icon.SetActive(HasDownloadableContent());
        }
        else
        {
            logpresent_icon.SetActive(false);
            visible_icon.SetActive(false);
            danger_icon.SetActive(false);
            download_icon.SetActive(false);
        }

        spyworm_icon.SetActive(player != 1 && CanPlayerSee(1) && HasSpyworm(1) && !terminated); // show spyworm icon
        if(HasStasis()) // stasis life cycle
        {
            active_stasis -= Time.deltaTime;
        }
        stasis_icon.SetActive(HasStasis() && !terminated); // show stasis animation

        if(safe_mode_timer >= 0.0f) // safe mode cycle
        {
            safe_mode_timer -= Time.deltaTime;
        }
        RegenerateIntegrity();
    }
    public bool IsType(string ask_type) // check if the device is of the given type
    {
        if(type == ask_type)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool IsFriendlyWith(int ask_player) // check if this device is friendly to the given player id
    {
        if(ask_player == player) // check if ask_player is admin (this.player)
        {
            return true;
        }
        else // check teams
        {
            if(PlayerManager.GetPlayerTeam(ask_player) != 0 && // if the ask_player is 0 (not in a team), he has no friendly players
                PlayerManager.GetPlayerTeam(ask_player) == PlayerManager.GetPlayerTeam(player))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public bool CanPlayerSee(int ask_player, bool stats=false) // return of player x can see this computer/stats
    {
        if(player == ask_player) // player can see it and everything about it if it's under his control
        {
            return true;
        }
        else
        {
            if (stats) // can see stats
            {
                return scanned.Contains(ask_player);
            }
            else
            {
                return visible.Contains(ask_player);
            }
        }
    }

    public void ChangeVisibility(int player_id, bool add) // add or delete a player from the "visible" list
    {
        if(add && !visible.Contains(player_id)){
            visible.Add(player_id);
            if(master_router != null)
            {
                master_router.GetComponent<DeviceScript>().ChangeVisibility(player_id, true);
            }
        }
        else
        {
            if (!add && visible.Contains(player_id))
            {
                visible.Remove(player_id);
                ChangeScanned(player_id, false);
            }
        }
        GameReloadRenderer();
    }

    public void ChangeScanned(int player_id, bool add) // add or delete a player from the "scanned" list
    {
        if (add && !scanned.Contains(player_id))
        {
            scanned.Add(player_id);
        }
        else
        {
            if (!add && scanned.Contains(player_id))
            {
                scanned.Remove(player_id);
            }
        }
        GameReloadRenderer();
    }

    public bool CanPlayerSeeActivity(int ask_player) // has this device a spyworm from player(id), can player see all tasks made by this device
        // returns true if ask_player can see all tasks in this device
        // can track vpn-ed operators
    {
        // ??? ask_player == player <- or maybe ? -> PlayerManager.ArePlayersFriendly(ask_player, player)
        if (ask_player == player || spyworm.Contains(ask_player))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool HasSpyworm(int ask_player)
    {
        return spyworm.Contains(ask_player);
    }

    public bool CanPlayerSeeAttachedTask(int ask_player, AttachedTask attachedTask) // can player see an attached task
    {
        if(attachedTask.task_name == CommandsDefinitions.botnet)
        {
            GameObject attacked = attachedTask.init_device.GetComponent<ComputerScript>().GetForeignTaskTarget();
            if(attacked != null)
            {
                if (attacked.GetComponent<DeviceScript>().player == 1) return true;
            }
        }
        if (CanPlayerSeeActivity(ask_player) && !attachedTask.stealth) // exmaple: other player who spywormed this device can see 'public' tasks like cyberattack
        {
            return true;
        }
        else // player can see tasks from other spywormed devices
        {
            if (attachedTask.init_device.GetComponent<DeviceScript>().CanPlayerSeeActivity(ask_player))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public Task GetOwnTask(int ask_player) // return active task on this computer
    {
        if(DeviceManagment.BelongsToCategory(gameObject, "Computer") && CanPlayerSeeActivity(ask_player))
        {
            ComputerScript computer = GetComponent<ComputerScript>();
            return computer.GetOwnTask();
        }
        return null;
    }

    public AttachedTask AttachTask(string task_name, GameObject init_device, bool stealth, GameObject animation) // attach new task
    {
        //print("device: " + gameObject + " attachedtask : " + task_name + " initdev: " + init_device);
        AttachedTask new_task = new AttachedTask()
        {
            task_name = task_name,
            init_device = init_device,
            stealth = stealth,
            animation = animation
        };
        attachedTasks.Add(new_task);

        CheckAttachedTaskDeviceVisibility(new_task);
        return new_task;
    }
    
    private void CheckAttachedTaskDeviceVisibility(AttachedTask task)
    {
        for (int i = 0; i <= PlayerManager.GetPlayerCount(); i++)
        {
            // make device, which is doing an unstealthed task on this device if it's spywormed, visible
            if (CanPlayerSeeActivity(i))
            {
                foreach (AttachedTask t in attachedTasks)
                {
                    if (!t.stealth)
                    {
                        t.init_device.GetComponent<DeviceScript>().ChangeVisibility(i, true);
                    }
                }
            }
        }

        // if the task is processed by a spywormed computer, make this device visible
        for (int i = 0; i <= PlayerManager.GetPlayerCount(); i++)
        {
            if (task.init_device.GetComponent<DeviceScript>().CanPlayerSeeActivity(i))
            {
                ChangeVisibility(i, true);
            }
        }
    }

    public void ChangeInitDeviceForAttachedTask(GameObject init_device, string task_name, GameObject new_init_device) // change init_device for a specific attached task
        // used when operator which is doing a task on this device change's its ip address through vpn
    {
        foreach(AttachedTask task in attachedTasks)
        {
            if(task.init_device == init_device && task.task_name == task_name)
            {
                task.init_device = new_init_device;
                CheckAttachedTaskDeviceVisibility(task);
            }
            return;
        }
        Debug.LogError("ChangeInitDeviceForAttachedTask ERROR: init_dev-" + init_device.name + 
            ", task name-" + task_name +
            ", new_init_dev-" + new_init_device.name);
    }

    public void DetachTask(GameObject init_device, string task_name) // detach/destroy task, enter the processing device
    {
        // first check if you are debugging the right device, you fucking cunt
        //print("device func, aT = " + attachedTasks.Count.ToString() + " ; device_name = "+gameObject.name);
        for(int i=0; i<attachedTasks.Count; i++)
        {
            //print("Is " + attachedTasks[i].task_name + " == " + task_name);
            if(attachedTasks[i].init_device == init_device && attachedTasks[i].task_name == task_name)
            {
                //print("destroying animation");
                Destroy(attachedTasks[i].animation);
                attachedTasks.RemoveAt(i);
                return;
            }
        }
        if(attachedTasks.Count == 0)
        {
            Debug.LogError("DetachTask ERROR: " + gameObject.name + " has no attached tasks to detach!");
        }
        else
        {
            Debug.LogError("DetachTask ERROR: Couldn't find the wanted task on " + gameObject.name + ". INFO: init_device - " + init_device.name + ", task_name - " + task_name);
        }
    }

    private void SetIntegrity(float value) // set integrity to value
    {
        integrity = value;
        if (GetTrueIntegrity() > GetTrueIntegrity(true))
        {
            integrity = GetTrueIntegrity(true);
        }
        integrity_bar.Show(GetTrueIntegrity() / GetTrueIntegrity(true));
    }
    public void ChangeIntegrity(float value) // change integrity -> +increase -decrease
    {
        if(value < 0.0f) // turn off safe mode if losing integrity
        {
            TurnDangerOn();
        }
        integrity = GetTrueIntegrity() + value;
        if(GetTrueIntegrity() > GetTrueIntegrity(true))
        {
            integrity = GetTrueIntegrity(true);
        }
        else if(GetTrueIntegrity() < 0.0f){
            integrity = 0.0f;
        }
        integrity_bar.Show(GetTrueIntegrity()/GetTrueIntegrity(true));
    }

    private void RegenerateIntegrity() // regenerate integrity
    {
        if(safe_mode_timer <= 0.0f && !terminated)
        {
            ChangeIntegrity(regeneration * Time.deltaTime);
        }
    }
    public void TurnDangerOn()
    {
        safe_mode_timer = safe_mode_deadline;
    }

    public void ChangeControl(int overtaking_player, bool violent, GameObject overtaking_device = null) // change controlling player
    {
        if (HasSpyworm(overtaking_player)) InstallSpyworm(overtaking_player, false);

        int last_player = player;
        if(Attack_log != null) // reveal last device from Attack_log
        {
            Attack_log.GetComponent<DeviceScript>().ChangeVisibility(overtaking_player, true);
            Attack_log = null;
        }

        if (violent) // write Attack_log
        {
            Attack_log = overtaking_device;
        }
        player = overtaking_player;
        SetIntegrity(0.5f * GetTrueIntegrity(true));

        // last player can see this device and has it scanned
        ChangeVisibility(last_player, true);
        ChangeScanned(last_player, true);


        if (DeviceManagment.BelongsToCategory(gameObject, "Computer"))
        {
            ComputerScript computer = GetComponent<ComputerScript>();
            if (computer.is_active_operator) // defeat operator
            {
                if (overtaking_player == 1)
                {
                    ConsoleController.ShowText("Operator of "+PlayerManager.GetPlayerName(last_player)+" was destroyed!", "success");
                }
                computer.GetComponent<OperatorScript>().Defeat();
            }
            else
            {
                // end active computer tasks
                if (GetOwnTask(player).type == "vpn_connection")
                {
                    GameObject found_operator = DeviceManagment.GetOperator(last_player);
                    found_operator.GetComponent<OperatorScript>().LoseVPN();
                    found_operator.GetComponent<DeviceScript>().ChangeVisibility(player, true);
                }
                else if (GetOwnTask(player).type == "slave")
                {
                    GameObject found_operator = DeviceManagment.GetOperator(last_player);
                    found_operator.GetComponent<OperatorScript>().LoseSlave(gameObject);
                }
                computer.MakeIdle();
            }
        }

        if(DeviceManagment.BelongsToCategory(gameObject, "Router"))
        {
            RouterScript router = GetComponent<RouterScript>();
            router.ChangeFirewallHealth(router.GetTrueFirewallHealth() * 0.2f);
            foreach(GameObject connected_device in router.local_connections)
            {
                connected_device.GetComponent<DeviceScript>().ChangeVisibility(overtaking_player, true);
            }
        }

        GameReloadRenderer();
    }
    public void FullyDisappear() // become invisible to all players
    {
        for (int i = 0; i <= PlayerManager.GetPlayerCount(); i++)
        {
            if (!CanPlayerSeeActivity(i))
            {
                ChangeVisibility(i, false);
            }
        }
    }
    public void DeleteLog()
    {
        Attack_log = null;
    }
    public string RemoveMalware(int antivirus_level) // remove malware from this device
    {
        string removed_malware = "";
        List<int> delete_spyworms = new List<int>();
        foreach(int worm in spyworm)
        {
            if(DeviceManagment.GetOperator(worm).GetComponent<OperatorScript>().spyworm_level <= antivirus_level)
            {
                delete_spyworms.Add(worm);
                removed_malware = "spyworm";
            }
        }
        foreach(int worm in delete_spyworms)
        {
            spyworm.Remove(worm);
        }

        // !!! add trojan find
        foreach(AttachedTask task in attachedTasks)
        {
            if(task.task_name == "trojanprocess")
            {
                DeviceScript trojan_device = task.init_device.GetComponent<DeviceScript>();
                if(DeviceManagment.GetOperator(trojan_device.player).GetComponent<OperatorScript>().trojan_level <= antivirus_level)
                {
                    trojan_device.ChangeVisibility(player, true);
                    task.stealth = false;
                    if (removed_malware == "spyworm") removed_malware += "+trojan";
                    else removed_malware = "trojan";
                }
            }
        }
        return removed_malware;
    }

    public void ActivateStasis(float duration) // activate stasis for given time in seconds
    {
        active_stasis = duration;
    }

    public void InstallSpyworm(int from_player, bool install) // install/inject spyworm
    {
        if (install)
        {
            if (!spyworm.Contains(from_player))
            {
                spyworm.Add(from_player);
            }
            foreach (AttachedTask task in attachedTasks)
            {
                CheckAttachedTaskDeviceVisibility(task); // check revealing
            }
        }
        else
        {
            if (spyworm.Contains(from_player))
            {
                spyworm.Remove(from_player);
            }
        }
        GameReloadRenderer();
    }
    public void UpdateBIOS(string upgrade) // install upgrade
    {
        switch (upgrade)
        {
            case "defense":
                upgrades.defense += 1;
                ShowDefense(GetTrueDefense());
                break;
            case "security":
                upgrades.security += 1;
                ShowSecurity(GetTrueSecurity());
                break;
            case "power":
                upgrades.power += 1;
                GetComponent<ComputerScript>().ReloadRenderer();
                break;
            case "firewall":
                upgrades.firewall += 1;
                GetComponent<RouterScript>().ReloadRenderer();
                break;
            default:
                Debug.LogError("Unknown upgrade: " + upgrade);
                break;
        }
    }
    public string DownloadContent() // download content
    {
        string output;
        if (DeviceManagment.BelongsToCategory(gameObject, "Server"))
        {
            output = GetComponent<ServerScript>().DownloadContent();
        }
        else
        {
            output = download_file.Substring(0);
            download_file = "";
        }
        return output;
    }
    public void Terminate() // terminate device immediately
    {
        if(DeviceManagment.BelongsToCategory(gameObject, "Computer") ||
            DeviceManagment.BelongsToCategory(gameObject, "Server"))
        {
            GetComponent<ComputerScript>().GiveOrder("uninstall", null);
        }
        spyworm.Clear();
        SetIntegrity(0.0f);
        terminated = true;
        GameReloadRenderer();
    }
    public void GameReloadRenderer() // check in-game visibility to player 1
    {
        if (CanPlayerSee(1)){
            MakeGameVisible(true);
            if(CanPlayerSee(1, stats: true))
            {
                MakeGameStatsVisible(true);
                MakeGameIdentityVisible(true);
            }
            else
            {
                MakeGameStatsVisible(false);
                if (stealth_identity)
                {
                    MakeGameIdentityVisible(false);
                }
                else
                {
                    MakeGameIdentityVisible(true);
                }
            }
            if (CanPlayerSeeActivity(1))
            {
                MakeGameSpywormThingsVisible(true);
            }
            else
            {
                MakeGameSpywormThingsVisible(false);
            }
        }
        else
        {
            MakeGameVisible(false);
            MakeGameStatsVisible(false);
            MakeGameSpywormThingsVisible(false);
        }
        MakeGameAdminEffectsVisible(player == 1);

        if (DeviceManagment.BelongsToCategory(gameObject, "Computer"))
        {
            GetComponent<ComputerScript>().ReloadRenderer();
        }
        if (DeviceManagment.BelongsToCategory(gameObject, "Router"))
        {
            GetComponent<RouterScript>().ReloadRenderer();
        }
        integrity_bar.Show(GetTrueIntegrity() / GetTrueIntegrity(true));
        ShowDefense(GetTrueDefense());
        ShowSecurity(GetTrueSecurity());
    }

    private void MakeGameVisible(bool yes) // make device in-game visible
    {
        // enable/disable renderer-components and box collider
        GetComponent<SpriteRenderer>().enabled = yes;
        GetComponent<BoxCollider2D>().enabled = yes;
        foreach(Transform child in transform)
        {
            if(child.GetComponent<MeshRenderer>() != null){
                child.GetComponent<MeshRenderer>().enabled = yes;
            }
            if (child.GetComponent<SpriteRenderer>() != null)
            {
                child.GetComponent<SpriteRenderer>().enabled = yes;
            }
            if (child.GetComponent<SpriteMask>() != null)
            {
                child.GetComponent<SpriteMask>().enabled = yes;
            }
        }
    }

    private void MakeGameStatsVisible(bool yes) // make in-game device's stats visible
    {
        foreach (Transform child in transform)
        {
            if (scan_required.Contains(child.name))
            {
                if (child.GetComponent<MeshRenderer>() != null)
                {
                    child.GetComponent<MeshRenderer>().enabled = yes;
                }
                if (child.GetComponent<SpriteRenderer>() != null)
                {
                    child.GetComponent<SpriteRenderer>().enabled = yes;
                }
                if (child.GetComponent<SpriteMask>() != null)
                {
                    child.GetComponent<SpriteMask>().enabled = yes;
                }
            }
        }
    }

    private void MakeGameIdentityVisible(bool yes) // show device's true identity/color
    {
        if (terminated)
        {
            GetComponent<SpriteRenderer>().color = PlayerManager.GetPlayerColor(-2);
            return;
        }
        if (yes)
        {
            GetComponent<SpriteRenderer>().color = PlayerManager.GetPlayerColor(player);
        }
        else
        {
            GetComponent<SpriteRenderer>().color = PlayerManager.GetPlayerColor(-1);
        }
    }
    private void MakeGameSpywormThingsVisible(bool yes) // make in-game device's stats visible
    {
        foreach (Transform child in transform)
        {
            if (spyworm_required.Contains(child.name))
            {
                if (child.GetComponent<MeshRenderer>() != null)
                {
                    child.GetComponent<MeshRenderer>().enabled = yes;
                }
                if (child.GetComponent<SpriteRenderer>() != null)
                {
                    child.GetComponent<SpriteRenderer>().enabled = yes;
                }
                if (child.GetComponent<SpriteMask>() != null)
                {
                    child.GetComponent<SpriteMask>().enabled = yes;
                }
            }
        }
    }

    private void MakeGameAdminEffectsVisible(bool yes)
    {
        foreach (Transform child in transform)
        {
            if (admin_required.Contains(child.name))
            {
                if (child.GetComponent<MeshRenderer>() != null)
                {
                    child.GetComponent<MeshRenderer>().enabled = yes;
                }
                if (child.GetComponent<SpriteRenderer>() != null)
                {
                    child.GetComponent<SpriteRenderer>().enabled = yes;
                }
                if (child.GetComponent<SpriteMask>() != null)
                {
                    child.GetComponent<SpriteMask>().enabled = yes;
                }
            }
        }
    }

    public string GetIP(bool end = true) // get ip address of this device
    {
        if(master_router == null)
        {
            return local_ip + ":";
        }
        else
        {
            DeviceScript devscript = master_router.GetComponent<DeviceScript>();
            if (end)
            {
                return devscript.GetIP(false) + local_ip;
            }
            else
            {
                return devscript.GetIP(false) + local_ip + ".";
            }
        }
    }

    public bool IsSafe()
    {
        return safe_mode_timer <= 0.0f;
    }
    public bool IsTerminated()
    {
        return terminated;
    }
    public bool IsVisibleToEnemies() // is device visible to enemies
    {
        bool visible_enemy = false;
        foreach (int p in visible)
        {
            if (p == 0 || !PlayerManager.IsPlayerPlaying(p))
            {
                continue;
            }
            if (!IsFriendlyWith(p) && CanPlayerSee(p))
            {
                visible_enemy = true;
            }
        }
        return visible_enemy;
    }
    public bool IsUnderAttack() // is device under visible attack
    {
        bool underattack = false;
        foreach(AttachedTask attachedTask in attachedTasks)
        {
            if(attachedTask.task_name == "cyberattack")
            {
                underattack = true;
                return true;
            }
            else if(attachedTask.task_name == "trojanprocess" && !attachedTask.stealth)
            {
                underattack = true;
                return true;
            }
            else if(DeviceManagment.BelongsToCategory(gameObject, "Router") && attachedTask.task_name == "breakfirewall")
            {
                underattack = true;
                return underattack;
            }
            else if (attachedTask.task_name == "botnet_attack")
            {
                underattack = true;
                return true;
            }
        }
        return underattack;
    }
    public bool IsUnderStealthAttack() // is device under invisible attack
    {
        foreach(AttachedTask attachedTask in attachedTasks)
        {
            if(attachedTask.task_name == "trojanprocess" && attachedTask.stealth)
            {
                return true;
            }
        }
        return false;
    }
    public bool HasStasis() // has device active stasis
    {
        return active_stasis > 0.0f;
    }
    public bool HasDownloadableContent() // has device downloadable content
    {
        if(DeviceManagment.BelongsToCategory(gameObject, "Server"))
        {
            return GetComponent<ServerScript>().HasDownloadableContent();
        }
        else
        {
            return download_file != "";
        }
    }
    public float GetTrueIntegrity(bool return_max=false) // get true integrity level, with all the bonuses and upgrades
    {
        if (return_max)
        {
            return max_integrity;
        }
        else
        {
            return integrity;
        }
    }
    public float GetTrueDefense()
    {
        float defense = base_defense;
        OperatorScript my_operator = DeviceManagment.GetOperator(player).GetComponent<OperatorScript>();
        if(upgrades.defense > 0)
        {
            defense += my_operator.upgrade_boost["defense"] * upgrades.defense;
        }
        return defense;
    }
    public int GetTrueSecurity()
    {
        int security = base_security;
        OperatorScript my_operator = DeviceManagment.GetOperator(player).GetComponent<OperatorScript>();
        if (upgrades.security > 0)
        {
            security += my_operator.upgrade_boost["security"] * upgrades.security;
        }
        return security;
    }
    public GameObject GetMasterRouter()
    {
        return master_router;
    }

    public void ConnectRouter(GameObject router) // invoke only on the start of the scene
    {
        master_router = router;
    }

    private void ShowDefense(float value) // update defense GUI
    {
        defensegui.text = ((int) value).ToString();
        defensegui.color = EffectsStash.stash.defense_upgrades[upgrades.defense];
    }
    private void ShowSecurity(int value) // update security level GUI
    {
        securitygui.text = value.ToString();
        securitygui.color = EffectsStash.stash.security_upgrades[upgrades.security];
    }
    public void ExecuteDeviceCommand(string[] arguments)
    {
        if(arguments[0] == "random")
        {
            float value = Random.Range(INT.Parse(arguments[2]), INT.Parse(arguments[3]) + 1);
            switch (arguments[1])
            {
                case Upgrades.DEFENSE:
                    base_defense = value;
                    break;
                case Upgrades.SECURITY:
                    base_security = (int)value;
                    break;
                case Upgrades.POWER:
                    GetComponent<ComputerScript>().base_power = value;
                    break;
            }
            GameReloadRenderer();
        }
        if(arguments[0] == "install")
        {
            GetComponent<ComputerScript>().GiveOrder(arguments[1], DeviceManagment.GetOperator(player));
        }
    }
}

public class AttachedTask // attached task class
{
    public string task_name; 
    public GameObject init_device; // device doing the task
    public bool stealth;
    public GameObject animation; // animated GameObject

    public void ShowAnimation(bool show) // show animated GameObject
    {
        animation.GetComponent<SpriteRenderer>().enabled = show;
    }

    public string AnalyzeInfo(bool own=false)
    {
        string output = "";
        output += task_name;
        
        if (own)
        {
            output += " on ";
        }
        else
        {
            output += " from ";
        }

        if(task_name == "vpn_connection")
        {
            output += DeviceManagment.GetOperator(init_device.GetComponent<DeviceScript>().player)
                .GetComponent<DeviceScript>().GetIP();
        }
        else
        {
            output += init_device.GetComponent<DeviceScript>().GetIP();
        }
        return output;
    }
}

public class Upgrades
{
    public int defense = 0;
    public int security = 0;
    public int power = 0;
    public int firewall = 0;

    public const string DEFENSE = "defense";
    public const string SECURITY = "security";
    public const string POWER = "power";
    public const string FIREWALL = "firewall";
    public const string UPGRADE_SUFFIX = "_upgrade";
}
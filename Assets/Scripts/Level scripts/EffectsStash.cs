using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// EffectsStash class contains all needed prefabs which can be created in real-time.
// - access any effects from EffectsStash.stash

public class EffectsStash : MonoBehaviour
{
    public static EffectsStash stash;

    // operator effects
    public GameObject task_rectangle;
    public GameObject scanning;
    public GameObject antivirus_scan;
    
    public GameObject cyberattacking;
    public GameObject trojanprocess;
    public GameObject breaking_firewall;
    public GameObject spyworm_install;
    public GameObject deleting_log;
    public GameObject tcpcatching;

    public GameObject counterattacking;
    public GameObject creating_stasis;
    public GameObject repairing;

    public GameObject creatingvpn;
    public GameObject creatingslave;

    public GameObject installing_software;
    public GameObject updating_bios;

    public GameObject downloading_data;

    public GameObject terminating;

    // computer effects
    public GameObject vpn_connection;
    public GameObject slave_process;
    public GameObject generating_pcs;
    public GameObject mining_catcoin;
    public GameObject botnet_source;
    public GameObject botnet_attacking;
    public GameObject antivirus_engine;
    public GameObject autorepair;

    // BIOS upgrades text colors
    public List<Color> defense_upgrades;
    public List<Color> security_upgrades;
    public List<Color> power_upgrades;
    public List<Color> firewall_upgrades;

    private void Awake()
    {
        stash = this;
    }
}

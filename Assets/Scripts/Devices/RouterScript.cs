using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


// RouterScript must be attached to every router.
//  - creates lines beetween local computers and global devices

public class RouterScript : MonoBehaviour
{
    private GameObject www_server; // global connection with the global net
    public List<GameObject> local_connections; // all devices connected to this router
    private LineRenderer linerender; // component for rendering lines

    public int firewall_level = 5; // level of firewall
    public float firewall_max_health = 100.0f; // firewall's maximal health
    public float firewall_health = 100.0f;

    private IntegrityMask firewall_bar; // firewall's bar
    private TextMeshPro firewallgui; // text displaying firewall level

    public DeviceScript This_device { get; private set; }

    private void Awake()
    {
        This_device = GetComponent<DeviceScript>();
        // create connections with connected devices
        DeviceScript devscript;
        foreach (GameObject local in local_connections)
        {
            devscript = local.GetComponent<DeviceScript>();
            devscript.ConnectRouter(this.gameObject);
        }

        foreach (Transform child in transform)
        {
            string child_name = child.name;
            if (child_name == "Firewall NUM")
            {
                firewallgui = child.GetComponent<TextMeshPro>();
            }
            if (child_name == "Firewall Mask")
            {
                firewall_bar = child.GetComponent<IntegrityMask>();
            }
        }
    }
    void Start()
    {
        // reveal devices for router owner
        foreach(GameObject local in local_connections)
        {
            local.GetComponent<DeviceScript>().ChangeVisibility(This_device.player, true);
        }

        // render things
        ShowFirewall(firewall_level);
        firewall_bar.Show(GetTrueFirewallHealth() / GetTrueFirewallHealth(true));
        RenderLines();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool IsFirewallActive() // is firewall active
    {
        return GetTrueFirewallHealth() > 0.0f;
    }
    public void ChangeFirewallHealth(float value) // change firewall's health/integrity
    {
        if(value < 0.0f) // if decreasing, turn on danger mode
        {
            This_device.TurnDangerOn();
        }
        firewall_health = GetTrueFirewallHealth() + value;
        if (GetTrueFirewallHealth() > GetTrueFirewallHealth(true))
        {
            firewall_health = GetTrueFirewallHealth(true);
        }
        else if (GetTrueFirewallHealth() < 0.0f)
        {
            firewall_health = 0.0f;
        }
        firewall_bar.Show(GetTrueFirewallHealth() / GetTrueFirewallHealth(true));
    }
    public float GetTrueFirewallHealth(bool max=false)
    {
        if (max)
        {
            return firewall_max_health;
        }
        else
        {
            return firewall_health;
        }
    }
    public int GetTrueFirewallLevel()
    {
        int truefirewall = firewall_level;
        OperatorScript my_operator = DeviceManagment.GetOperator(This_device.player).GetComponent<OperatorScript>();
        if (This_device.upgrades.firewall > 0)
        {
            truefirewall += my_operator.upgrade_boost["firewall"] * This_device.upgrades.firewall;
        }
        return truefirewall;
    }
    public int GetConnectedCount(bool ignore_vpn=true) // returns count of connected devices, if ignore_vpn is true, count devices that have VPN
    {
        if (ignore_vpn)
        {
            int connections = 0;
            foreach (GameObject local in local_connections)
            {
                if(DeviceManagment.BelongsToCategory(local, "Computer"))
                {
                    if (local.GetComponent<ComputerScript>().is_active_operator)
                    {
                        if (local.GetComponent<OperatorScript>().vpn_connection != null)
                        {
                            connections--;
                        }
                    }
                }
            }
            return connections;
        }
        return local_connections.Count;
    }
    public void ShowFirewall(int value)
    {
        firewallgui.text = value.ToString();
        firewallgui.color = EffectsStash.stash.firewall_upgrades[This_device.upgrades.firewall];
    }
    public void RenderLines() // render lines between visible connected devices
    {
        linerender = GetComponent<LineRenderer>();
        int max_vertex = 0;
        foreach (GameObject local in local_connections)
        {
            if (local.GetComponent<DeviceScript>().CanPlayerSee(1))
            {
                max_vertex += 2;
            }
        }
        if (max_vertex > 0)
        {
            linerender.positionCount = max_vertex;
            int vertex = 0;

            foreach (GameObject local in local_connections)
            {
                if (local.GetComponent<DeviceScript>().CanPlayerSee(1))
                {
                    linerender.SetPosition(vertex, gameObject.transform.position);
                    vertex++;
                    linerender.SetPosition(vertex, local.transform.position);
                    vertex++;
                }
            }
        }

        Color color;
        if(!(This_device.stealth_identity && !This_device.CanPlayerSee(1, true)))
        {
            color = PlayerManager.GetPlayerColor(This_device.player);
            color.a = 0.5f;
            linerender.startColor = color;
            linerender.endColor = color;
        }
        else
        {
            color = PlayerManager.GetPlayerColor(-1);
            color.a = 0.5f;
            linerender.startColor = color;
            linerender.endColor = color;
        }
    }
    public void ReloadRenderer()
    {
        RenderLines();
        ShowFirewall(GetTrueFirewallLevel());
        firewall_bar.Show(GetTrueFirewallHealth() / GetTrueFirewallHealth(true));
    }
    public void ConnectToInternet(GameObject server) // invoke only on start of the scene
    {
        www_server = server;
    }
}

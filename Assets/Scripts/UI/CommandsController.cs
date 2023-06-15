using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandsController : MonoBehaviour
{
    public bool console_input_only = false;
    public bool scan = true;
    public bool tcp = true;
    public bool analyze_tasks = true;
    public bool antivirus = true;
    public bool cyberattack = true;
    public bool trojan = true;
    public bool break_firewall = true;
    public bool spyworm = true;
    public bool delete_log = true;
    public bool counterattack = true;
    public bool create_stasis = true;
    public bool repair = true;
    public bool uninstall = true;
    public bool install = true;
    public bool update_bios = true;
    public bool download = true;
    public bool terminate = true;
    public bool ping = true;

    public bool generate_pcs = true;
    public bool mine_catcoin = true;
    public bool vpn = true;
    public bool create_slave = true;
    public bool install_antivirus = true;
    public bool install_autorepair = true;
    public bool install_botnet = true;

    public bool upgrade_defense = true;
    public bool upgrade_security = true;
    public bool upgrade_power = true;
    public bool upgrade_firewall = true;

    public static CommandsController commands;
    public static string openned_menu { get; private set; }

    private GameObject scan_button = null;
    private GameObject tcp_button = null;
    private GameObject analyze_tasks_button = null;
    private GameObject antivirus_button = null;
    private GameObject cyberattack_button = null;
    private GameObject trojan_button = null;
    private GameObject break_firewall_button = null;
    private GameObject spyworm_button = null;
    private GameObject deletelog_button = null;
    private GameObject counterattack_button = null;
    private GameObject create_stasis_button = null;
    private GameObject repair_button = null;
    private GameObject uninstall_button = null;
    private GameObject install_button = null;
    private GameObject update_bios_button = null;
    private GameObject download_button = null;
    private GameObject terminate_button = null;
    private GameObject ping_button = null;

    private GameObject return_button = null;

    private GameObject generate_pcs_button = null;
    private GameObject mine_catcoin_button = null;
    private GameObject vpn_button = null;
    private GameObject create_slave_button = null;
    private GameObject install_antivirus_button = null;
    private GameObject install_autorepair_button = null;
    private GameObject install_botnet_button = null;

    private GameObject upgrade_defense_button = null;
    private GameObject upgrade_security_button = null;
    private GameObject upgrade_power_button = null;
    private GameObject upgrade_firewall_button = null;
    void Start()
    {
        commands = this;
        foreach (Transform child in transform)
        {
            GameObject button_gameObject = child.gameObject;
            switch (button_gameObject.name)
            {
                case "Scan":
                    scan_button = button_gameObject;
                    break;
                case "TCP Catch":
                    tcp_button = button_gameObject;
                    break;
                case "Analyze Tasks":
                    analyze_tasks_button = button_gameObject;
                    break;
                case "Antivirus Scan":
                    antivirus_button = button_gameObject;
                    break;
                case "Cyberattack":
                    cyberattack_button = button_gameObject;
                    break;
                case "Trojan":
                    trojan_button = button_gameObject;
                    break;
                case "Break Firewall":
                    break_firewall_button = button_gameObject;
                    break;
                case "Inject Spyworm":
                    spyworm_button = button_gameObject;
                    break;
                case "Delete Log":
                    deletelog_button = button_gameObject;
                    break;
                case "Counterattack":
                    counterattack_button = button_gameObject;
                    break;
                case "Create Stasis":
                    create_stasis_button = button_gameObject;
                    break;
                case "Repair":
                    repair_button = button_gameObject;
                    break;
                case "Create VPN":
                    vpn_button = button_gameObject;
                    break;
                case "Create Slave":
                    create_slave_button = button_gameObject;
                    break;
                case "Uninstall Software":
                    uninstall_button = button_gameObject;
                    break;
                case "Install Software":
                    install_button = button_gameObject;
                    break;
                case "Update BIOS":
                    update_bios_button = button_gameObject;
                    break;
                case "Download":
                    download_button = button_gameObject;
                    break;
                case "Terminate":
                    terminate_button = button_gameObject;
                    break;
                case "Ping":
                    ping_button = button_gameObject;
                    break;
                case "Return":
                    return_button = button_gameObject;
                    break;
                case "Generate PCS":
                    generate_pcs_button = button_gameObject;
                    break;
                case "Mine CatCoin":
                    mine_catcoin_button = button_gameObject;
                    break;
                case "Install Antivirus":
                    install_antivirus_button = button_gameObject;
                    break;
                case "Install Autorepair":
                    install_autorepair_button = button_gameObject;
                    break;
                case "Install BotNet":
                    install_botnet_button = button_gameObject;
                    break;
                case "Upgrade Defense":
                    upgrade_defense_button = button_gameObject;
                    break;
                case "Upgrade Security":
                    upgrade_security_button = button_gameObject;
                    break;
                case "Upgrade Power":
                    upgrade_power_button = button_gameObject;
                    break;
                case "Upgrade Firewall":
                    upgrade_firewall_button = button_gameObject;
                    break;
                default:
                    Debug.LogError("Couldn't find button with name " + button_gameObject.name);
                    break;
            }
        }

        OpenMainCommands(!console_input_only);
        OpenInstallCommands(false);
    }
    public void OpenMainCommands(bool show)
    {
        ShowCommand(scan_button, show && scan);
        ShowCommand(tcp_button, show && tcp);
        ShowCommand(analyze_tasks_button, show && analyze_tasks);
        ShowCommand(antivirus_button, show && antivirus);
        ShowCommand(cyberattack_button, show && cyberattack);
        ShowCommand(trojan_button, show && trojan);
        ShowCommand(break_firewall_button, show && break_firewall);
        ShowCommand(spyworm_button, show && spyworm);
        ShowCommand(deletelog_button, show && delete_log);
        ShowCommand(counterattack_button, show && counterattack);
        ShowCommand(create_stasis_button, show && create_stasis);
        ShowCommand(repair_button, show && repair);
        ShowCommand(uninstall_button, show && uninstall);
        ShowCommand(install_button, show && install);
        ShowCommand(update_bios_button, show && update_bios);
        ShowCommand(download_button, show && download);
        ShowCommand(terminate_button, show && terminate);
        ShowCommand(ping_button, show && ping);

        if (show)
        {
            openned_menu = "MAIN";
            OpenInstallCommands(false);
            OpenUpdateBIOSCommands(false);
        }
    }

    public void OpenInstallCommands(bool show)
    {
        ShowCommand(return_button, show);
        ShowCommand(generate_pcs_button, show && generate_pcs);
        ShowCommand(mine_catcoin_button, show && mine_catcoin);
        ShowCommand(vpn_button, show && vpn);
        ShowCommand(create_slave_button, show && create_slave);
        ShowCommand(install_antivirus_button, show && install_antivirus);
        ShowCommand(install_autorepair_button, show && install_autorepair);
        ShowCommand(install_botnet_button, show && install_botnet);

        if (show)
        {
            openned_menu = "INSTALL";
            OpenMainCommands(false);
            ShowCommand(uninstall_button, show && uninstall);
        }
    }
    public void OpenUpdateBIOSCommands(bool show)
    {
        ShowCommand(return_button, show);
        ShowCommand(upgrade_defense_button, show && upgrade_defense);
        ShowCommand(upgrade_security_button, show && upgrade_security);
        ShowCommand(upgrade_power_button, show && upgrade_power);
        ShowCommand(upgrade_firewall_button, show && upgrade_firewall);

        if (show)
        {
            openned_menu = "BIOS";
            OpenMainCommands(false);
        }
    }
    private void ShowCommand(GameObject button, bool enable_condition)
    {
        button.SetActive(enable_condition);
    }
}
